using SICER.MIGRACION.Connections;
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
                /*
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] { new MainTasks() };
                ServiceBase.Run(ServicesToRun);*/

                Connection con = new Connection();
                try
                {
                    con.initializeConnections();
                    HashSet<string> companies = con.companiesConnected();
                    foreach (string company in companies)
                    {
                        SAPbobsCOM.Company Company = con.getCompany(company);

                        BusinessPartners bp = new BusinessPartners(Company);
                        bp.migrateBP(Company);

                        JournalEntries je = new JournalEntries(Company);
                        je.migrate(Company);

                        Invoices rInv = new Invoices(Company);
                        rInv.migrate(Company);

                        WebInvoices webInv = new WebInvoices();
                        webInv.migrate(Company);

                        SalesInvoices salesInv = new SalesInvoices(Company);
                        salesInv.migrate(Company);

                    }
                    con.Dispose();
                }
                catch (Exception ex)
                {
                    con.Dispose();
                }
                finally
                {
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
            }
            catch (Exception e)
            {
                System.IO.File.WriteAllLines(@"C:\lel.txt", new string[] { e.ToString() });
            }
        }
    }
}
