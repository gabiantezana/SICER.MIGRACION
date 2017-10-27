using SICER.MIGRACION.Connections;
using SICER.MIGRACION.Documents;
using SICER.MIGRACION.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SICER.MIGRACION.TEST
{
    class Program
    {
        static void Main(string[] args)
        {
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

                    WebInvoices webInv = new WebInvoices();
                    webInv.migrate(Company);
                }
                con.Dispose();
            }
            catch (Exception ex)
            {
                con.Dispose();
                ExceptionHelper.LogException(ex);

            }
            finally
            {
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
    }
}
