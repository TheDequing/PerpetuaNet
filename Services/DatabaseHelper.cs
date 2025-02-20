using System;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;

namespace PerpetuaNet.Services
{
    public class DatabaseHelper
    {
        private static readonly string DbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "perpetuanet.db");
        private const string EncryptionKey = "a8B!cD3eFgH4iJ5kLmN6oP7qRsT8uV9w"; // Substitua por uma chave mais segura

        public DatabaseHelper()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            // Se o arquivo existe, tente abrir o banco para verificar se ele é válido.
            if (File.Exists(DbPath))
            {
                try
                {
                    using (var connection = new SqliteConnection(GetConnectionString()))
                    {
                        connection.Open();
                    }
                }
                catch (SqliteException ex)
                {
                    Console.WriteLine($"[DEBUG] Banco de dados inválido: {ex.Message}. Excluindo arquivo...");
                    File.Delete(DbPath);
                }
            }

            // Cria o banco de dados se não existir (ou se foi excluído)
            using (var connection = new SqliteConnection(GetConnectionString()))
            {
                connection.Open();
                string createTablesCommand = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT UNIQUE NOT NULL,
                Password TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS DataSync (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Data TEXT NOT NULL
            );
        ";

                using (var command = new SqliteCommand(createTablesCommand, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }


        private string GetConnectionString()
        {
            // A chave de criptografia é passada para o SQLCipher através da propriedade Password.
            return $"Data Source={DbPath};Password={EncryptionKey}";
        }

        public bool AddUser(string username, string password)
        {
            try
            {
                using (var connection = new SqliteConnection(GetConnectionString()))
                {
                    connection.Open();
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                    string insertCommand = "INSERT INTO Users (Username, Password) VALUES (@username, @password)";

                    using (var command = new SqliteCommand(insertCommand, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", hashedPassword);
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar usuário: {ex.Message}");
                return false;
            }
        }

        public bool ValidateUser(string username, string password)
        {
            Console.WriteLine($"[DEBUG] Buscando usuário no banco: {username}");

            using (var connection = new SqliteConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT Password FROM Users WHERE Username = @username";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    var storedPassword = command.ExecuteScalar()?.ToString();

                    Console.WriteLine($"[DEBUG] Senha armazenada no banco: {storedPassword}");

                    bool isValid = storedPassword != null && BCrypt.Net.BCrypt.Verify(password, storedPassword);
                    Console.WriteLine($"[DEBUG] Resultado da validação: {isValid}");

                    return isValid;
                }
            }
        }

        public void DebugListUsers()
        {
            using (var connection = new SqliteConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT Username FROM Users";

                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"[DEBUG] Usuário no banco: {reader.GetString(0)}");
                        }
                    }
                }
            }
        }

        public void StoreData(string data)
        {
            string encryptedData = EncryptData(data);
            using (var connection = new SqliteConnection(GetConnectionString()))
            {
                connection.Open();
                string insertCommand = "INSERT INTO DataSync (Data) VALUES (@data)";

                using (var command = new SqliteCommand(insertCommand, connection))
                {
                    command.Parameters.AddWithValue("@data", encryptedData);
                    command.ExecuteNonQuery();
                }
            }
        }

        public string RetrieveData()
        {
            using (var connection = new SqliteConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT Data FROM DataSync ORDER BY Id DESC LIMIT 1";

                using (var command = new SqliteCommand(query, connection))
                {
                    var result = command.ExecuteScalar()?.ToString();
                    return result != null ? DecryptData(result) : string.Empty;
                }
            }
        }

        public string EncryptData(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32));
                aesAlg.IV = new byte[16]; // Vetor de inicialização zerado

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public string DecryptData(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32));
                aesAlg.IV = new byte[16];

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}
