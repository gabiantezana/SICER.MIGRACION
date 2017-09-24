using SICER.MIGRACION.Helper;
using SICER.MIGRACION.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SICER.MIGRACION.Documents
{
    class Payments
    {

        public void executePayments(SAPbobsCOM.Company Company)
        {
            ADODB.Recordset pendingPayments = new ADODB.Recordset();
            ADODB.Recordset updateRS = new ADODB.Recordset();

            //pendingPayments.DoQuery("EXEC SEI_STW_AccountPayments");
            String Execquery = "EXEC " + nameof(SICER_INT_SBOEntities.MSS_SP_SICER_ACCOUNTPAYMENTS);
            pendingPayments.DoQuery(Execquery);

            while (!pendingPayments.EOF)
            {
                try
                {
                    Company.StartTransaction();
                    if (payDocument(Company, pendingPayments))
                    {
                        String query = "UPDATE PagosCuenta SET INT_Estado = 'P' WHERE IdPago = " + pendingPayments.Fields.Item("IdPago").Value;
                        updateRS.DoQuery(query);
                        Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                    }
                    else
                    {
                        if (Company.InTransaction)
                        {
                            Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        }
                        String query = "UPDATE PagosCuenta SET INT_Estado = 'E', INT_Desc = '" + Company.GetLastErrorDescription().Replace('\'', ' ') + "' WHERE IdPago = " + pendingPayments.Fields.Item("IdPago").Value;
                        updateRS.DoQuery(query);
                    }
                }
                catch (Exception)
                {
                    if (Company.InTransaction)
                    {
                        if (Company.InTransaction)
                        {
                            Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        }
                        String query = "UPDATE PagosCuenta SET INT_Estado = 'E' WHERE IdPago = " + pendingPayments.Fields.Item("IdPago").Value;
                        updateRS.DoQuery(query);
                    }
                }
                finally
                {
                    pendingPayments.MoveNext();
                }
            }
        }

        private bool payDocument(SAPbobsCOM.Company Company, ADODB.Recordset recSetInstance)
        {
            SAPbobsCOM.Payments incPayment = Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments);
            incPayment.DocType = SAPbobsCOM.BoRcptTypes.rAccount;
            incPayment.DocCurrency = recSetInstance.Fields.Item("Moneda").Value;
            incPayment.ControlAccount = recSetInstance.Fields.Item("CuentaControl").Value;
            incPayment.AccountPayments.AccountCode = recSetInstance.Fields.Item("CuentaDetalle").Value;
            incPayment.AccountPayments.AccountName = recSetInstance.Fields.Item("Nombre").Value;
            incPayment.AccountPayments.SumPaid = recSetInstance.Fields.Item("Monto").Value;
            incPayment.AccountPayments.Decription = recSetInstance.Fields.Item("Memo").Value;
            incPayment.Series = 1; //change

            incPayment.TransferAccount = recSetInstance.Fields.Item("CuentaDetalle").Value;
            incPayment.TransferSum = recSetInstance.Fields.Item("Monto").Value;
            return incPayment.Add() == 0;
        }

    }
}
