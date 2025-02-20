using System;
using System.IO;

namespace PerpetuaNet.Services
{
    public static class Logger
    {
        private static readonly string LogDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PerpetuaNetLogs");
        private static readonly string LogFilePath = Path.Combine(LogDirectory, "logs.txt");

        public static void Log(string message)
        {
            try
            {
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }

                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
                Console.WriteLine(logMessage);
                File.AppendAllText(LogFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO NO LOGGER] Não foi possível escrever no log: {ex.Message}");
            }
        }
    }
}
