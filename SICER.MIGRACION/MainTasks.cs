using SICER.MIGRACION.Connections;
using SICER.MIGRACION.Documents;
using SICER.MIGRACION.Documents.Structs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace SICER.MIGRACION
{
    partial class MainTasks : ServiceBase
    {
        private const double INITIAL_TIME = 5000.0d;
        private const double CYCLE_INTERVAL = 5000.0d;
        private Timer trigger;

        public MainTasks()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            trigger = new Timer(INITIAL_TIME);
            trigger.Interval = CYCLE_INTERVAL;
            trigger.AutoReset = false;
            trigger.Elapsed += new System.Timers.ElapsedEventHandler(Tasks);
            trigger.Enabled = true;
        }

        protected override void OnStop()
        {
            trigger.Dispose();
        }

        private void Tasks(object sender, ElapsedEventArgs e)
        {
            trigger.Enabled = false;
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
                trigger.Enabled = true;
            }
        }
    }
}
