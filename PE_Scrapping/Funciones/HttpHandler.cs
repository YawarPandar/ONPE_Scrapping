using System;
using System.IO;
using System.Net;
using OpenQA.Selenium;
using System.Threading;
using OpenQA.Selenium.Chrome;
using YPandar.Common.Functional;

namespace PE_Scrapping.Funciones
{
    public static class HttpHandler
    {
        static IWebDriver _driver;
        static string error_root = string.Empty;
        public static void StartWebDriver(Action action, string ChromeDriverPath, int MilisecondsWait)
        {
            FunctionalHandler.WriteLines(new string[] {
                Messages.DOUBLE_LINE(),
                Messages.INITIALIZING_DRIVER,
                Messages.DOUBLE_LINE(),
            });
            ChromeOptions options = new ChromeOptions { Proxy = null };
            _driver = new ChromeDriver(ChromeDriverPath, options);
            _driver.Manage().Timeouts().PageLoad = TimeSpan.FromMilliseconds(MilisecondsWait);
            _driver.Manage().Window.Minimize();
            FunctionalHandler.WriteLines(new string[]
            {
                Messages.DOUBLE_LINE(),
                Messages.DRIVER_INITIATED,
                Messages.DOUBLE_LINE(),
            });
            Thread.Sleep(5000);
            action();
            _driver.Close();
        }
        public static string SendApiRequest(string url, string tag)
        {
            bool success = false;
            string json = string.Empty;
            while (!success)
            {
                try
                {
                    _driver.Navigate().GoToUrl(url);
                    json = _driver.FindElement(By.TagName(tag)).Text;
                    success = true;
                }
                catch { }
            }
            return json;
        }
        public static void DownloadFile(string url_file, string save_file, string path, string folder)
        {
            bool result = Uri.TryCreate(url_file, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (result)
            {
                string full_path = Path.Combine(string.Format(path, "ACTAS"), folder);
                if (!Directory.Exists(full_path))
                {
                    Directory.CreateDirectory(full_path);
                }
                full_path = Path.Combine(full_path, save_file);
                using (var client = new WebClient())
                {
                    bool success = false;
                    int intento = 0;
                    while (!success && intento <= 5)
                    {
                        try
                        {
                            client.DownloadFile(url_file, full_path);
                            success = true;
                        }
                        catch (Exception ex)
                        {
                            ErrorLog(string.Concat("Error descargando acta.: ", full_path), path);
                            ErrorLog(ex.Message, path);
                            Console.WriteLine("Error de conexión al intentar descargar acta. Reintentando...");
                            intento++;
                            if (intento < 5)
                            {
                                Console.WriteLine("Reintentando...");
                            }
                            else
                            {
                                ErrorLog("No se pudo descargar acta luego de 5 intentos.", path);
                                Console.WriteLine("No se pudo descargar acta luego de 5 intentos.");
                                success = true;
                            }
                        }
                    }
                }
            }
        }
        private static void ErrorLog(string mensaje, string path)
        {
            error_root = string.IsNullOrEmpty(error_root) ? Guid.NewGuid().ToString() : error_root;
            var ruta_guardar = string.Format(path, "LOG");
            if (!Directory.Exists(ruta_guardar)) Directory.CreateDirectory(ruta_guardar);
            string[] mensajes = { mensaje };
            File.AppendAllLines(Path.Combine(ruta_guardar, string.Concat("errors_", error_root, ".log")), mensajes);
        }
    }
}