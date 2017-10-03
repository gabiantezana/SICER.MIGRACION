﻿using SICER.MIGRACION.Connections;
using SICER.MIGRACION.Documents;
using SICER.MIGRACION.Documents.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace SICER.MIGRACION
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        {
            try
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] { new MainTasks() };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception e)
            {
                //System.IO.File.WriteAllLines(@"D:\log.txt", new string[] { e.ToString() });
            }
        }
    }
}
