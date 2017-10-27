using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SICER.MIGRACION.Helper
{
    public sealed class ExceptionHelper
    {
        private ExceptionHelper() { }

        public static void LogException(Exception exc)
        {

            String route = @"C:\LOG\" + GetProjectName();
            String fileName = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            String logFile = route + @"\" + fileName;

            System.IO.Directory.CreateDirectory(route);

            if (!System.IO.File.Exists(logFile))
                System.IO.File.Create(logFile).Close();

            System.IO.StreamWriter sw = new System.IO.StreamWriter(logFile, true);
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
