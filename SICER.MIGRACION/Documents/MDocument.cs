using SICER.MIGRACION.Connections;
using SICER.MIGRACION.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SICER.MIGRACION.Documents
{
    abstract class MDocument : IDisposable
    {
        protected readonly string migrationSP;
        protected readonly string keyField;
        protected ADODB.Recordset updateRS;

        protected MDocument(SAPbobsCOM.Company Company, string migrationStoredProcedure, string KeyField)
        {
            updateRS = new ADODB.Recordset();
            migrationSP = migrationStoredProcedure;
            keyField = KeyField;
        }

        public void migrateBP(SAPbobsCOM.Company Company)
        {
            ADODB.Recordset migrationRS = new SQLConnection().DoQuery(migrationSP);
            while (!migrationRS.EOF)
            {
                Company.StartTransaction();
                string currentDocEntry = migrationRS.Fields.Item(keyField).Value;
                string code = migrationRS.Fields.Item("Code").Value;
                try
                {
                    if (migrateDocuments(Company, migrationRS))
                    {
                        update(Company, true, currentDocEntry.ToString(), code);
                        Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                    }
                    else
                    {
                        Company.GetLastErrorDescription();
                        if (Company.InTransaction) { Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack); }
                        update(Company, false, currentDocEntry.ToString(), code);
                    }

                }
                catch (Exception)
                {
                    if (Company.InTransaction) { Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack); }
                    update(Company, false, currentDocEntry, code);
                }
                migrationRS.MoveNext();
            }
            if (Company.InTransaction) { Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack); }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(migrationRS);
            migrationRS = null;
        }

        public void migrate(SAPbobsCOM.Company Company)
        {
            ADODB.Recordset migrationRS = new SQLConnection().DoQuery(migrationSP);

            while (!migrationRS.EOF)
            {
                Company.StartTransaction();
                int currentDocEntry = migrationRS.Fields.Item(keyField).Value;
                string Code = migrationRS.Fields.Item("Code").Value;
                try
                {
                    if (migrateDocuments(Company, migrationRS))
                    {
                        update(Company, true, currentDocEntry.ToString(), Code);
                        Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                    }
                    else
                    {
                        Company.GetLastErrorDescription();
                        if (Company.InTransaction) { Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack); }
                        update(Company, false, currentDocEntry.ToString(), Code);
                    }

                }
                catch (Exception)
                {
                    if (Company.InTransaction) { Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack); }
                    update(Company, false, currentDocEntry.ToString(), Code);
                }
                migrationRS.MoveNext();
            }
            if (Company.InTransaction) { Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack); }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(migrationRS);
            migrationRS = null;
        }

        protected abstract void update(SAPbobsCOM.Company Company, bool successful, string id, string Code);

        protected abstract bool migrateDocuments(SAPbobsCOM.Company Company, ADODB.Recordset migrationRS);

        public void Dispose()
        {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(updateRS);
            updateRS = null;
        }
    }
}
