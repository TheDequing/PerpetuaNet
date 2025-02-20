using System;
using System.Threading;
using Avalonia;
using PerpetuaNet.Services;

namespace PerpetuaNet
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                Logger.Log("[DEBUG] Inicializando PerpetuaNet...");
                Thread.Sleep(2000);

                var dbHelper = new DatabaseHelper();
                // Evitar duplicação: verifica se o usuário já existe
                if (!dbHelper.ValidateUser("admin", "12345"))
                {
                    dbHelper.AddUser("admin", "12345");
                    Logger.Log("[DEBUG] Usuário de teste criado.");
                }
                else
                {
                    Logger.Log("[DEBUG] Usuário de teste já existe.");
                }

                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
                Logger.Log("[DEBUG] PerpetuaNet carregado com sucesso.");
            }
            catch (Exception ex)
            {
                Logger.Log($"[ERRO CRÍTICO] O programa fechou inesperadamente: {ex.Message}");
            }
            // Não utilizamos Console.ReadKey() para evitar exceção em aplicativos GUI
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
