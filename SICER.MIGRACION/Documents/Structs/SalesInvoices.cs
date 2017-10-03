using SICER.MIGRACION.Connections;
using SICER.MIGRACION.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SICER.MIGRACION.Documents.Structs
{
    class SalesInvoices : MDocument
    {

        private const string INVOICES_HEADER_SP = "EXEC " + "MSS_SP_SICER_PAYROLLINVOICES"; //"SEI_STW_PayrollInvoices";
        private const string INVOICES_TABLE = "FacturasPayroll";
        private const string INVOICES_KEY = "IdFactura";

        public SalesInvoices(SAPbobsCOM.Company Company)
            : base(Company, INVOICES_HEADER_SP, INVOICES_KEY)
        {
        }

        protected override void update(SAPbobsCOM.Company Company, bool successful, string id, string Code)
        {
            string updateString = "UPDATE " + INVOICES_TABLE + " SET INT_Estado = '";
            if (successful)
            {
                updateString += "P' ";
            }
            else
            {
                updateString += "E', INT_Error = '" + Company.GetLastErrorDescription().Replace('\'', ' ') + "' ";
            }
            updateString += "WHERE IdFactura = " + id;

            updateRS = new SQLConnection().DoQuery(updateString);
        }

        protected override bool migrateDocuments(SAPbobsCOM.Company Company, ADODB.Recordset migrationRS)
        {
            SAPbobsCOM.Documents salesInvoice = (SAPbobsCOM.Documents)Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
            salesInvoice.DocObjectCode = SAPbobsCOM.BoObjectTypes.oInvoices;
            salesInvoice.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Service;
            string invDate = migrationRS.Fields.Item("INT_Fecha").Value.ToSafeString();
            DateTime pDate = DateTime.ParseExact(invDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            salesInvoice.CardCode = migrationRS.Fields.Item("CardCode").Value.ToSafeString();
            salesInvoice.DocDate = pDate;
            salesInvoice.TaxDate = pDate;
            salesInvoice.DocCurrency = migrationRS.Fields.Item("DocCurrency").Value.ToSafeString();
            salesInvoice.ControlAccount = migrationRS.Fields.Item("AccountCode").Value.ToSafeString();
            salesInvoice.GroupNumber = migrationRS.Fields.Item("CondicionPago").Value.ToInt32();
            salesInvoice.UserFields.Fields.Item("U_MSS_GRPFACT").Value = migrationRS.Fields.Item("CO_GRUP_FACT").Value;
            //salesInvoice.UserFields.Fields.Item("U_MSS_FECREC").Value = pDate;

            salesInvoice.Lines.LineTotal = migrationRS.Fields.Item("IM_BRUT_FACT").Value.ToDouble();
            salesInvoice.Lines.TaxCode = migrationRS.Fields.Item("IM_BRUT_IGV").Value.ToInt32() > 0 ? "IGV" : "IGV_EXE";
            salesInvoice.Lines.AccountCode = migrationRS.Fields.Item("AccountCodeDet").Value.ToSafeString();
            salesInvoice.Lines.ItemDescription = migrationRS.Fields.Item("NO_GLOS_SAP1").Value.ToSafeString();
            salesInvoice.Lines.CostingCode = migrationRS.Fields.Item("CostingCode").Value.ToSafeString();
            //salesInvoice.Lines.CostingCode2 = migrationRS.Fields.Item("CostingCode2").Value;
            //salesInvoice.Lines.CostingCode3 = migrationRS.Fields.Item("CostingCode3").Value;
            //salesInvoice.Lines.CostingCode4 = migrationRS.Fields.Item("CostingCode4").Value;
            //salesInvoice.Lines.CostingCode5 = migrationRS.Fields.Item("CostingCode5").Value;

            return salesInvoice.Add() == 0;
        }
    }
}
