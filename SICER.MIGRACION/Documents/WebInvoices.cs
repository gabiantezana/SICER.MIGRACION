using SICER.MIGRACION.Connections;
using SICER.MIGRACION.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SICER.MIGRACION.Documents
{
    class WebInvoices
    {
        private const string INVOICES_SP_HEADER = "EXEC " + "MSS_SP_SICER_FACTURACIONWEB"; //"EXEC [SEI_STW_FacturacionWeb]";
        private const string INVOICES_SP_LINES = "EXEC " + "MSS_SP_SICER_FACTURACIONWEBDETALLE"; //"EXEC [SEI_STW_FacturacionWebDetalle] ";
        private const string INVOICES_TABLE = "FacturasWeb";

        public void migrate(SAPbobsCOM.Company Company)
        {
            ADODB.Recordset migrationRS = new ADODB.Recordset();
            ADODB.Recordset updateRS = new ADODB.Recordset();

            migrationRS = new SQLConnection().DoQuery(INVOICES_SP_HEADER);

            while (!migrationRS.EOF)
            {
                migrateDocuments(Company, migrationRS);
                migrationRS.MoveNext();
            }
        }

        protected void migrateDocuments(SAPbobsCOM.Company Company, ADODB.Recordset migrationRS)
        {
            int exCode = migrationRS.Fields.Item("ExCode").Value.ToInt32();
            int tipoDoc = migrationRS.Fields.Item("TipoDocumento").Value.ToInt32();
            int etapa = migrationRS.Fields.Item("Etapa").Value.ToInt32();
            int idFactura = migrationRS.Fields.Item("IdFactura").Value.ToInt32();
            ADODB.Recordset updateRS = new ADODB.Recordset();
            try
            {
                SAPbobsCOM.Documents invoice;
                int docSubType = migrationRS.Fields.Item("DocSubType").Value.ToInt32();
                switch (docSubType)
                {
                    case 18:
                        invoice = Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
                        break;
                    case 19:
                        invoice = Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes);
                        break;
                    case 181:
                        invoice = Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
                        break;
                    default:
                        invoice = Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
                        break;
                }

                //invoice.UserFields.Fields.Item("U_BPP_MDTD").Value = migrationRS.Fields.Item("U_BPP_MDTD").Value;
                string sDocDate = migrationRS.Fields.Item("DocDate").Value.ToSafeString();
                string sDueDate = migrationRS.Fields.Item("DocDueDate").Value.ToSafeString();
                string sTaxDate = migrationRS.Fields.Item("TaxDate").Value.ToSafeString();
                DateTime docDate = DateTime.ParseExact(sDocDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                DateTime dueDate = DateTime.ParseExact(sDueDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                DateTime taxDate = DateTime.ParseExact(sTaxDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                //invoice.Series = migrationRS.Fields.Item("Series").Value;
                invoice.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Service;
                invoice.CardCode = migrationRS.Fields.Item("CardCode").Value.ToSafeString();
                invoice.DocCurrency = migrationRS.Fields.Item("DocCurrency").Value.ToSafeString();
                invoice.JournalMemo = migrationRS.Fields.Item("JournalMemo").Value.ToSafeString();
                invoice.Comments = migrationRS.Fields.Item("Asunto").Value.ToSafeString();
                //invoice.PaymentMethod = migrationRS.Fields.Item("MetodoPago").Value.ToSafeString();
                invoice.NumAtCard = migrationRS.Fields.Item("NumAtCard").Value.ToSafeString();
                //invoice.ControlAccount = migrationRS.Fields.Item("ControlAccount").Value;
                invoice.FolioPrefixString = migrationRS.Fields.Item("FolioPref").Value.ToSafeString();
                invoice.FolioNumber = migrationRS.Fields.Item("FolioNum").Value.ToInt32();
                //invoice.UserFields.Fields.Item("U_MSS_REFACT_COT").Value = migrationRS.Fields.Item("Refacturable").Value;
                //invoice.UserFields.Fields.Item("U_MSS_Refacturado").Value = "Y";
                invoice.DocDate = docDate;
                invoice.TaxDate = taxDate;
                invoice.DocDueDate = dueDate;
                invoice.UserFields.Fields.Item("U_ExCode").Value = exCode.ToString();
                invoice.UserFields.Fields.Item("U_Etapa").Value = etapa.ToString();
                invoice.UserFields.Fields.Item("U_WebType").Value = tipoDoc.ToString();

                /*--------------------------------------GET CONTROL ACCOUNT/*--------------------------------------*/

                String folioPref = migrationRS.Fields.Item("FolioPref").Value.ToSafeString();
                String U_codigo = String.Empty;

                switch (invoice.DocCurrency)
                {
                    case "SOL":
                    case "S/":
                    case "SOLES":
                        switch (folioPref)
                        {
                            case "CC": U_codigo = "DPRMN"; break;
                            case "ER": U_codigo = "ERCTAMN"; break;
                            case "RE": U_codigo = "REDPRMN"; break;

                        }
                        break;
                    default:
                        switch (folioPref)
                        {
                            case "CC": U_codigo = "DPRME"; break;
                            case "ER": U_codigo = "ERCTAME"; break;
                            case "RE": U_codigo = "REDPRME"; break;
                        }
                        break;
                }

                ADODB.Recordset getAccountRS = new ADODB.Recordset();
                String queryControlAccount = "EXEC MSS_SP_SICER_GETACCOUNTFROMCONFIG '" + folioPref + "' , '" + U_codigo + "'";
                getAccountRS = new SQLConnection().DoQuery(queryControlAccount);

                String controlAccount = getAccountRS.Fields.Item("U_CuentaContable").Value.ToSafeString();
                if (String.IsNullOrEmpty(controlAccount))
                    throw new Exception("No se encontró ControlAccount para documento en la tabla de configuración. Query: " + queryControlAccount);
                else
                    invoice.ControlAccount = controlAccount;

                /*------------------------------------FIN GET CONTROL ACCOUNT/*--------------------------------------*/


                String query = INVOICES_SP_LINES + "'" + exCode + "', '" + tipoDoc + "', '" + etapa + "', '" + migrationRS.Fields.Item("IdFactura").Value + "'";
                ADODB.Recordset lines = new SQLConnection().DoQuery(query);
                while (!lines.EOF)
                {
                    invoice.Lines.AccountCode = lines.Fields.Item("AccountCode").Value.ToSafeString();
                    invoice.Lines.TaxCode = lines.Fields.Item("TaxCode").Value.ToSafeString();
                    //invoice.Lines.TaxCode = "IGV";
                    invoice.Lines.LineTotal = Convert.ToDouble(lines.Fields.Item("LineTotal").Value);
                    invoice.Lines.CostingCode = lines.Fields.Item("CostingCode").Value.ToSafeString();
                    //invoice.Lines.CostingCode2 = lines.Fields.Item("CostingCode2").Value;
                    //invoice.Lines.CostingCode3 = lines.Fields.Item("CostingCode3").Value;
                    //invoice.Lines.CostingCode4 = lines.Fields.Item("CostingCode4").Value;
                    //invoice.Lines.CostingCode5 = lines.Fields.Item("CostingCode5").Value;
                    invoice.Lines.ItemDescription = lines.Fields.Item("Description").Value.ToSafeString();
                    invoice.Lines.Add();
                    lines.MoveNext();
                }
                int totalLines = invoice.Lines.Count;
                double documentTotal = invoice.DocTotal;
                Company.StartTransaction();

                if (invoice.Add() == 0)
                {
                    int newDocEntry = int.Parse(Company.GetNewObjectKey());
                    bool isInvoice = (docSubType != 19);
                    bool shouldProceed;
                    if (etapa == 1)
                    {
                        shouldProceed = true;
                    }
                    else
                    {
                        shouldProceed = payInvoice(Company, newDocEntry, isInvoice);
                    }
                    if (shouldProceed)
                    {
                        updateRS = new SQLConnection().DoQuery("UPDATE " + INVOICES_TABLE + " SET INT_Estado = 'P' WHERE IdFactura = " + idFactura + " AND ExCode = " + exCode + " AND TipoDocumento = " + tipoDoc + " AND Etapa = " + etapa);
                        Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                    }
                    else
                    {
                        if (Company.InTransaction)
                        {
                            Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        }
                        updateRS = new SQLConnection().DoQuery("UPDATE " + INVOICES_TABLE + " SET INT_Estado = 'E', INT_Error = '" + Company.GetLastErrorDescription().Replace('\'', ' ').Substring(0, Company.GetLastErrorDescription().ToString().Length > 200 ? 190 : Company.GetLastErrorDescription().ToString().Length) + "' WHERE IdFactura = " + idFactura + " AND ExCode = " + exCode + " AND TipoDocumento = " + tipoDoc + " AND Etapa = " + etapa);
                    }
                }
                else
                {
                    if (Company.InTransaction)
                    {
                        Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                    }
                    updateRS = new SQLConnection().DoQuery("UPDATE " + INVOICES_TABLE + " SET INT_Estado = 'E', INT_Error = '" + Company.GetLastErrorDescription().Replace('\'', ' ').Substring(0, Company.GetLastErrorDescription().ToString().Length > 200 ? 190 : Company.GetLastErrorDescription().ToString().Length) + "' WHERE IdFactura = " + idFactura + " AND ExCode = " + exCode + " AND TipoDocumento = " + tipoDoc + " AND Etapa = " + etapa);
                }
            }
            catch (Exception e)
            {
                if (Company.InTransaction)
                {
                    Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                }
                ExceptionHelper.LogException(e);
                updateRS = new SQLConnection().DoQuery("UPDATE " + INVOICES_TABLE + " SET INT_Estado = 'E', INT_Error = '" + e.ToString().Replace('\'', ' ').Substring(0, e.ToString().Length > 200 ? 190 : e.ToString().Length) + "' WHERE IdFactura = " + idFactura + " AND ExCode = " + exCode + " AND TipoDocumento = " + tipoDoc + " AND Etapa = " + etapa);
            }
        }

        private bool payInvoice(SAPbobsCOM.Company Company, int docEntry, bool isInvoice)
        {

            SAPbobsCOM.Payments payment = Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oVendorPayments);
            SAPbobsCOM.Documents doc = isInvoice ? Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices) : Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes);
            doc.GetByKey(docEntry);

            ADODB.Recordset stageOneAccount = new ADODB.Recordset();

            //String query = "EXEC SEI_ATW_CuentasPago " + doc.UserFields.Fields.Item("U_WebType").Value + ", " + doc.UserFields.Fields.Item("U_ExCode").Value;
            String query = "EXEC  " + "MSS_SP_SICER_CUENTASPAGO" + " " + doc.UserFields.Fields.Item("U_WebType").Value + ", " + doc.UserFields.Fields.Item("U_ExCode").Value;

            stageOneAccount = new SQLConnection().DoQuery(query);


            payment.CardCode = doc.CardCode;
            payment.DocDate = doc.DocDate;
            payment.TaxDate = doc.TaxDate;
            payment.DueDate = doc.DocDueDate;
            payment.TransferDate = doc.DocDate;
            payment.DocCurrency = doc.DocCurrency;
            // payment.Series = stageOneAccount.Fields.Item("Series").Value.ToInt32(); ;//34 PERU Y CONSULTING, ROOM 32
            payment.TransferAccount = stageOneAccount.Fields.Item("AcctCode").Value.ToSafeString();
            payment.Remarks = doc.JournalMemo;
            payment.JournalRemarks = doc.JournalMemo;

            payment.Invoices.InvoiceType = isInvoice ? SAPbobsCOM.BoRcptInvTypes.it_PurchaseInvoice : SAPbobsCOM.BoRcptInvTypes.it_PurchaseCreditNote;
            payment.Invoices.DocEntry = docEntry;


            switch (doc.DocCurrency)
            {
                case "SOL":
                case "S/":
                    payment.Invoices.SumApplied = doc.DocTotal;
                    break;
                default:
                    payment.Invoices.AppliedFC = doc.DocTotalFc;
                    break;
            }
            payment.TransferSum = doc.DocCurrency.Equals("SOL") ? doc.DocTotal : doc.DocTotalFc;
            return payment.Add() == 0;
        }
    }
}
