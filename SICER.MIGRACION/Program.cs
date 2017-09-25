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
                SAPbobsCOM.Company Company = null;

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
            catch (Exception e)
            {
                System.IO.File.WriteAllLines(@"C:\lel.txt", new string[] { e.ToString() });
            }
        }
    }
}
