using System;
using System.IO;
using System.Reflection;

namespace SICER.MIGRACION.Helper
{
    public sealed class ExceptionHelper
    {
        private ExceptionHelper()
        {
        }

        public static void LogException(Exception exc)
        {
            var route = @"C:\LOG\" + GetProjectName();
            var fileName = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            var logFile = route + @"\" + fileName;

            Directory.CreateDirectory(route);

            if (!File.Exists(logFile))
                File.Create(logFile).Close();

            var sw = new StreamWriter(logFile, true);
            sw.WriteLine(" * ********* {0} **********", DateTime.Now);

            sw.Write("Exception Type: ");
            sw.WriteLine(exc.GetType().ToString());
            sw.WriteLine("Exception: " + exc.Message);
            sw.WriteLine("Stack Trace: ");
            if (exc.InnerException != null)
            {
                sw.Write("Inner Exception Type: ");
                sw.WriteLine(exc.InnerException.GetType().ToString());
                sw.Write("Inner Exception: ");
                sw.WriteLine(exc.InnerException.Message);
                sw.Write("Inner Source: ");
                sw.WriteLine(exc.InnerException.Source);
                if (exc.InnerException.StackTrace != null)
                {
                    sw.WriteLine("Inner Stack Trace: ");
                    sw.WriteLine(exc.InnerException.StackTrace);
                }
            }

            if (exc.StackTrace != null)
            {
                sw.WriteLine(exc.StackTrace);
                sw.WriteLine();
            }
            sw.Close();
        }

        private static string GetProjectName()
        {
            try
            {
                return Assembly.GetCallingAssembly().GetName().Name;
            }
            catch (Exception)
            {
                return "UNDEFINED";
            }
        }
    }
}