using SICER.MIGRACION.Connections;
using SICER.MIGRACION.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace SICER.MIGRACION.Documents
{
    public class WebInvoices
    {
        private const string INVOICES_SP_HEADER = "EXEC " + "MSS_SP_SICER_FACTURACIONWEB"; //"EXEC [SEI_STW_FacturacionWeb]";
        private const string INVOICES_SP_LINES = "EXEC " + "MSS_SP_SICER_FACTURACIONWEBDETALLE"; //"EXEC [SEI_STW_FacturacionWebDetalle] ";
        //private const string INVOICES_TABLE = "FacturasWeb";
        private const string INVOICES_TABLE = "FACTURASWEBMIGRACION";
        private const string SP_GETFACTURAS = "EXEC MSS_SP_SICER_GETFACTURASWEBMIGRACION";

        public void migrate(SAPbobsCOM.Company Company)
        {
            ADODB.Recordset migrationRS = new ADODB.Recordset();
            ADODB.Recordset updateRS = new ADODB.Recordset();

            migrationRS = new SQLConnection().DoQuery(SP_GETFACTURAS);

            while (!migrationRS.EOF)
            {
                migrateDocuments(Company, migrationRS);
                migrationRS.MoveNext();
            }
        }

        protected void migrateDocuments(SAPbobsCOM.Company Company, ADODB.Recordset migrationRS)
        {
            TipoDocumentoWeb tipoDocumentoWeb = (TipoDocumentoWeb)migrationRS.Fields.Item("TipoDocumento").Value.ToInt32();
            int exCode = migrationRS.Fields.Item("ExCode").Value.ToInt32();
            int tipoDoc = migrationRS.Fields.Item("TipoDocumento").Value.ToInt32();
            int etapa = migrationRS.Fields.Item("Etapa").Value.ToInt32();
            int idFacturaWebMigracion = migrationRS.Fields.Item("IdFacturaWebMigracion").Value.ToInt32();
            var asdfasf = migrationRS.Fields.Item("DocDate").Value.ToSafeString();

            String _aperturaCodigo = migrationRS.Fields.Item("Code").Value.ToSafeString();

            ADODB.Recordset updateRS = new ADODB.Recordset();
            try
            {
                SAPbobsCOM.Documents invoice;
                int docSubType = migrationRS.Fields.Item("DocSubType").Value.ToInt32();
                switch (docSubType)
                {
                    case 18:
                    case 181:
                    default:
                        invoice = Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
                        break;
                    case 19:
                        invoice = Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes);
                        break;
                }

                invoice.Indicator = migrationRS.Fields.Item("U_BPP_MDTD").Value.ToSafeString();
                invoice.ControlAccount = migrationRS.Fields.Item("ControlAccount").Value.ToSafeString();
                if (migrationRS.Fields.Item("Series").Value.ToInt32() != 0)
                    invoice.Series = (Int32)migrationRS.Fields.Item("Series").Value;
                invoice.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Service;
                invoice.CardCode = migrationRS.Fields.Item("CardCode").Value.ToSafeString();
                invoice.DocCurrency = migrationRS.Fields.Item("DocCurrency").Value.ToSafeString();
                invoice.JournalMemo = migrationRS.Fields.Item("JournalMemo").Value.ToSafeString();
                invoice.Comments = migrationRS.Fields.Item("Asunto").Value.ToSafeString();
                invoice.PaymentMethod = migrationRS.Fields.Item("PaymentMethod").Value.ToSafeString();
                invoice.NumAtCard = migrationRS.Fields.Item("NumAtCard").Value.ToSafeString();
                invoice.FolioPrefixString = migrationRS.Fields.Item("FolioPref").Value.ToSafeString();
                invoice.FolioNumber = migrationRS.Fields.Item("FolioNum").Value.ToInt32();
                invoice.DocDate = DateTime.ParseExact(migrationRS.Fields.Item("DocDate").Value.ToSafeString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                invoice.DocDueDate = DateTime.ParseExact(migrationRS.Fields.Item("DocDueDate").Value.ToSafeString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                invoice.TaxDate = DateTime.ParseExact(migrationRS.Fields.Item("TaxDate").Value.ToSafeString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                //CAMPOS DE USUARIO
                try
                { invoice.UserFields.Fields.Item("U_MSSL_TOP").Value = "02"; }
                catch (Exception ex) { ExceptionHelper.LogException(ex); }

                try
                { invoice.UserFields.Fields.Item("U_MSSL_TBI").Value = "A"; }
                catch (Exception ex) { ExceptionHelper.LogException(ex); }

                try
                { invoice.UserFields.Fields.Item("U_ExCode").Value = exCode.ToString(); }
                catch (Exception ex) { ExceptionHelper.LogException(ex); }

                try
                { invoice.UserFields.Fields.Item("U_Etapa").Value = etapa.ToString(); }
                catch (Exception ex) { ExceptionHelper.LogException(ex); }

                try
                { invoice.UserFields.Fields.Item("U_WebType").Value = tipoDoc.ToString(); }
                catch (Exception ex) { ExceptionHelper.LogException(ex); }

                try
                {
                    if (migrationRS.Fields.Item("FolioPref").Value.ToSafeString() == TipoDocumentoSunat.ReciboDeHonorarios.GetCodigoSunat())
                        invoice.UserFields.Fields.Item("U_MSSL_TBI").Value = "R";
                }
                catch (Exception ex) { ExceptionHelper.LogException(ex); }


                //DOCUMENT LINES
                invoice.Lines.AccountCode = migrationRS.Fields.Item("AccountCode").Value.ToSafeString();
                invoice.Lines.TaxCode = migrationRS.Fields.Item("TaxCode").Value.ToSafeString();
                invoice.Lines.LineTotal = Convert.ToDouble(migrationRS.Fields.Item("LineTotal").Value);
                invoice.Lines.CostingCode = migrationRS.Fields.Item("CostingCode").Value.ToSafeString();
                invoice.Lines.CostingCode2 = migrationRS.Fields.Item("CostingCode2").Value.ToSafeString();
                invoice.Lines.CostingCode3 = migrationRS.Fields.Item("CostingCode3").Value.ToSafeString();
                invoice.Lines.CostingCode4 = migrationRS.Fields.Item("CostingCode4").Value.ToSafeString();
                invoice.Lines.CostingCode5 = migrationRS.Fields.Item("CostingCode5").Value.ToSafeString(); ;
                invoice.Lines.ItemDescription = migrationRS.Fields.Item("Description").Value.ToSafeString();

                try
                { invoice.Lines.UserFields.Fields.Item("U_MSS_ORD").Value = migrationRS.Fields.Item("U_MSS_ORD").Value.ToSafeString(); }
                catch (Exception ex) { ExceptionHelper.LogException(ex); }

                invoice.Lines.Add();
                Company.StartTransaction();

                if (invoice.Add() == 0)
                {
                    int newDocEntry = int.Parse(Company.GetNewObjectKey());
                    bool isInvoice = (docSubType != 19);
                    bool shouldProceed;

                    if (etapa == 1)
                        shouldProceed = true;
                    else
                        shouldProceed = payInvoice(Company, newDocEntry, isInvoice, _aperturaCodigo);

                    if (shouldProceed)
                    {
                        UpdateFacturaWebMigracion(idFacturaWebMigracion, "P", null, newDocEntry, ref updateRS);
                        Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                    }
                    else
                    {
                        if (Company.InTransaction)
                            Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

                        String error = Company.GetLastErrorDescription().Replace('\'', ' ').Substring(0, Company.GetLastErrorDescription().ToString().Length > 200 ? 190 : Company.GetLastErrorDescription().ToString().Length);
                        UpdateFacturaWebMigracion(idFacturaWebMigracion, "E", error, newDocEntry, ref updateRS);
                    }
                }
                else
                {
                    if (Company.InTransaction)
                        Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

                    String error = Company.GetLastErrorDescription().Replace('\'', ' ').Substring(0, Company.GetLastErrorDescription().ToString().Length > 200 ? 190 : Company.GetLastErrorDescription().ToString().Length);
                    UpdateFacturaWebMigracion(idFacturaWebMigracion, "E", error, null, ref updateRS);
                }
            }
            catch (Exception e)
            {
                if (Company.InTransaction)
                {
                    Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                }

                ExceptionHelper.LogException(e);

                //String error = Company.GetLastErrorDescription().Replace('\'', ' ').Substring(0, Company.GetLastErrorDescription().ToString().Length > 200 ? 190 : Company.GetLastErrorDescription().ToString().Length);
                String error = e.ToString();
                UpdateFacturaWebMigracion(idFacturaWebMigracion, "E", error, null, ref updateRS);
            }
        }

        private bool payInvoice(SAPbobsCOM.Company Company, int docEntry, bool isInvoice, String _aperturaCodigo)
        {
            try
            {
                SAPbobsCOM.Payments payment = Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oVendorPayments);
                SAPbobsCOM.Documents doc = isInvoice
                    ? Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices)
                    : Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes);
                doc.GetByKey(docEntry);

                ADODB.Recordset stageOneAccount = new ADODB.Recordset();

                String query = "EXEC  " + "MSS_SP_SICER_CUENTASPAGO '" + _aperturaCodigo + "'";
                stageOneAccount = new SQLConnection().DoQuery(query);
                ExceptionHelper.LogException(new Exception(query));

                payment.CardCode = doc.CardCode;
                payment.DocDate = doc.DocDate;
                payment.TaxDate = doc.TaxDate;
                payment.DueDate = doc.DocDueDate;
                payment.TransferDate = doc.DocDate;
                payment.DocCurrency = doc.DocCurrency;
                payment.CounterReference = _aperturaCodigo;
                payment.TransferAccount = stageOneAccount.Fields.Item("AccountCode").Value.ToSafeString();
                payment.Remarks = doc.JournalMemo;
                payment.JournalRemarks = doc.JournalMemo;

                payment.Invoices.InvoiceType = isInvoice
                    ? SAPbobsCOM.BoRcptInvTypes.it_PurchaseInvoice
                    : SAPbobsCOM.BoRcptInvTypes.it_PurchaseCreditNote;
                payment.Invoices.DocEntry = docEntry;

                try
                { payment.UserFields.Fields.Item("U_MSSL_TMP").Value = "008"; }
                catch (Exception ex) { ExceptionHelper.LogException(ex); }
               
                switch (doc.DocCurrency)
                {
                    case "SOL":
                    case "S/":
                        payment.Invoices.SumApplied = doc.DocTotal;
                        payment.TransferSum = doc.DocTotal;
                        break;
                    default:
                        payment.Invoices.AppliedFC = doc.DocTotalFc;
                        payment.TransferSum = doc.DocTotalFc;
                        break;
                }
                return payment.Add() == 0;
            }
            catch (Exception ex)
            {
                ExceptionHelper.LogException(ex);
                throw;
            }
            
        }

        public void UpdateFacturaWebMigracion(Int32 idFacturaMigracion, String INT_Estado, String INT_Error, Int32? DocEntry, ref ADODB.Recordset updateRS)
        {
            updateRS = new SQLConnection().DoQuery("UPDATE " + INVOICES_TABLE + " SET INT_Estado = '" + INT_Estado + "', INT_Error = '" + INT_Error + "', DocEntry=" + (DocEntry ?? 0) + " WHERE IdFacturaWebMigracion = " + idFacturaMigracion);
        }
    }
}
