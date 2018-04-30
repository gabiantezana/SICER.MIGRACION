using System;
using System.Configuration;
using System.Reflection;
using System.ServiceProcess;
using System.Timers;
using SICER.MIGRACION.Connections;
using SICER.MIGRACION.Documents;
using SICER.MIGRACION.Helper;

namespace SICER.MIGRACION
{
    internal partial class MainTasks : ServiceBase
    {
        private const double INITIAL_TIME = 5000.0d;
        private const double CYCLE_INTERVAL = 5000.0d;
        private Timer trigger;


        public MainTasks()
        {
            InitializeComponent();
            this.ServiceName = ConfigurationManager.AppSettings.Get("ServiceName");
        }

     
        protected override void OnStart(string[] args)
        {
            var ex = new Exception("Service has been started");
            ExceptionHelper.LogException(ex);

            trigger = new Timer(INITIAL_TIME);
            trigger.Interval = CYCLE_INTERVAL;
            trigger.AutoReset = false;
            trigger.Elapsed += Tasks;
            trigger.Enabled = true;
        }

        protected override void OnStop()
        {
            var ex = new Exception("Service has been stopped");
            ExceptionHelper.LogException(ex);
            trigger.Dispose();
        }

        private void Tasks(object sender, ElapsedEventArgs e)
        {
            trigger.Enabled = false;
            var con = new Connection();
            try
            {
                con.initializeConnections();
                var companies = con.companiesConnected();
                foreach (var company in companies)
                {
                    var Company = con.getCompany(company);

                    var bp = new BusinessPartners(Company);
                    bp.migrateBP(Company);

                    //JournalEntries je = new JournalEntries(Company);
                    //je.migrate(Company);

                    //Invoices rInv = new Invoices(Company);
                    //rInv.migrate(Company);

                    var webInv = new WebInvoices();
                    webInv.migrate(Company);

                    //SalesInvoices salesInv = new SalesInvoices(Company);
                    //salesInv.migrate(Company);
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
                trigger.Enabled = true;
            }
        }
    }
}