using SICER.MIGRACION.Connections;
using SICER.MIGRACION.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SICER.MIGRACION.Documents
{
    class BusinessPartners : MDocument
    {
        private const string BUSINESS_PARTNER_SP_SUFFIX = "EXEC " + "MSS_SP_SICER_BUSINESSPARTNERS"; //"SEI_STW_BusinessPartners";
        private const string BUSINESS_PARTNER_TABLE = "SICER_INT_SBO.dbo.MaestroTrabajadores";
        private const string BP_KEY = "CardCode";

        public BusinessPartners(SAPbobsCOM.Company Company)
            : base(Company, BUSINESS_PARTNER_SP_SUFFIX, BP_KEY)
        {
        }

        protected override void update(SAPbobsCOM.Company Company, bool successful, string id, string code)
        {
            string updateString = "UPDATE " + BUSINESS_PARTNER_TABLE + " SET INT_Estado = '";
            if (successful)
            {
                updateString += "P' ";
            }
            else
            {
                updateString += "E', INT_Error = '" + Company.GetLastErrorDescription().Replace('\'', ' ') + "' ";
            }
            updateString += "WHERE CardCode = '" + id + "' AND Code = '" + code + "'";

            updateRS = new SQLConnection().DoQuery(updateString);
        }

        protected override bool migrateDocuments(SAPbobsCOM.Company Company, ADODB.Recordset migrationRS)
        {
            string action = migrationRS.Fields.Item("INT_Estado").Value.ToSafeString();
            switch (action)
            {
                case "A":
                    return addBP(Company, migrationRS);
                case "U":
                    return updateBP(Company, migrationRS);
                case "C":
                    return closeBP(Company, migrationRS);
            }
            return false;
        }

        private bool addBP(SAPbobsCOM.Company Company, ADODB.Recordset recordSet)
        {
            SAPbobsCOM.BusinessPartners businessPartner = (SAPbobsCOM.BusinessPartners)Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
            //Fields
            businessPartner.CardCode = recordSet.Fields.Item("CardCode").Value.ToSafeString();
            businessPartner.CardName = recordSet.Fields.Item("CardName").Value.ToSafeString();
            businessPartner.CardType = SAPbobsCOM.BoCardTypes.cSupplier;
            businessPartner.FederalTaxID = recordSet.Fields.Item("LicTradNum").Value.ToSafeString();
            businessPartner.DebitorAccount = recordSet.Fields.Item("DebitAccount").Value.ToSafeString();
            //businessPartner.ContactPerson = recordSet.Fields.Item("CntPerson").Value;
            businessPartner.GroupCode = recordSet.Fields.Item("GroupCode").Value.ToInt32(); 
            businessPartner.Address = recordSet.Fields.Item("Address").Value.ToSafeString();
            businessPartner.Phone1 = recordSet.Fields.Item("Phone1").Value.ToSafeString();
            businessPartner.Phone2 = recordSet.Fields.Item("Phone2").Value.ToSafeString();
            businessPartner.Cellular = recordSet.Fields.Item("Cellular").Value.ToSafeString();
            businessPartner.EmailAddress = recordSet.Fields.Item("E_Mail").Value.ToSafeString();
            businessPartner.Currency = "##";
            //businessPartner.UserFields.Fields.Item("U_BPP_BPNO").Value = recordSet.Fields.Item("U_BPP_BPNO").Value;
            //if (businessPartner.CardCode.StartsWith("E"))
            //{
            //    if (Company.CompanyDB.Equals("SBO_IIMP_PRUEBAS"))
            //    {
            //        businessPartner.BPPaymentMethods.PaymentMethodCode = "BcpCheqSoles-P";
            //        businessPartner.BPPaymentMethods.Add();
            //        businessPartner.BPPaymentMethods.PaymentMethodCode = "BCPTrLiqSoles-P";
            //        businessPartner.BPPaymentMethods.Add();
            //        businessPartner.BPPaymentMethods.PaymentMethodCode = "BCPChGeLiqSol-P";
            //        businessPartner.BPPaymentMethods.Add();
            //        businessPartner.BPPaymentMethods.PaymentMethodCode = "BBVATrLiqSole-P";
            //        businessPartner.BPPaymentMethods.Add();
            //    }
            //    else
            //    {
            //        businessPartner.BPPaymentMethods.PaymentMethodCode = "BcpCheqSoles-C";
            //        businessPartner.BPPaymentMethods.Add();
            //        businessPartner.BPPaymentMethods.PaymentMethodCode = "BCPTrLiqSoles-C";
            //        businessPartner.BPPaymentMethods.Add();
            //        businessPartner.BPPaymentMethods.PaymentMethodCode = "BCPChGeLiqSol-C";
            //        businessPartner.BPPaymentMethods.Add();
            //        businessPartner.BPPaymentMethods.PaymentMethodCode = "BBVATrLiqSole-C";
            //        businessPartner.BPPaymentMethods.Add();
            //    }
            //}
            //else
            //{
            //    if (businessPartner.CardCode.Length == 12)
            //    {
            //        if (Company.CompanyDB.Equals("SBO_TAWA_PERU_SAC"))
            //        {
            //            businessPartner.BPPaymentMethods.PaymentMethodCode = "BcpProvSoles-P";
            //            businessPartner.BPPaymentMethods.Add();
            //            businessPartner.BPPaymentMethods.PaymentMethodCode = "BcpTraRetSole-P";
            //            businessPartner.BPPaymentMethods.Add();
            //            businessPartner.BPPaymentMethods.PaymentMethodCode = "BcpProvDolar-P";
            //            businessPartner.BPPaymentMethods.Add();
            //            businessPartner.BPPaymentMethods.PaymentMethodCode = "BBVATrDetSole-P";
            //            businessPartner.BPPaymentMethods.Add();
            //        }
            //        else
            //        {
            //            businessPartner.BPPaymentMethods.PaymentMethodCode = "BcpProvSoles-C";
            //            businessPartner.BPPaymentMethods.Add();
            //            businessPartner.BPPaymentMethods.PaymentMethodCode = "BcpTraRetSole-C";
            //            businessPartner.BPPaymentMethods.Add();
            //            businessPartner.BPPaymentMethods.PaymentMethodCode = "BcpProvDolar-C";
            //            businessPartner.BPPaymentMethods.Add();
            //            businessPartner.BPPaymentMethods.PaymentMethodCode = "BBVATrDetSole-C";
            //            businessPartner.BPPaymentMethods.Add();
            //        }
            //    }
            //    else
            //    {
            //        if (Company.CompanyDB.Equals("SBO_IIMP_PRUEBAS"))
            //        {
            //            businessPartner.BPPaymentMethods.PaymentMethodCode = "BCPTrERCSoles-P";
            //            businessPartner.BPPaymentMethods.Add();
            //            businessPartner.BPPaymentMethods.PaymentMethodCode = "BBVATrERCSol-P";
            //            businessPartner.BPPaymentMethods.Add();
            //        }
            //        else
            //        {
            //            businessPartner.BPPaymentMethods.PaymentMethodCode = "BCPTrERCSoles-C";
            //            businessPartner.BPPaymentMethods.Add();
            //            businessPartner.BPPaymentMethods.PaymentMethodCode = "BBVATrERCSol-C";
            //            businessPartner.BPPaymentMethods.Add();
            //        }
            //    }
            //}

            if (recordSet.Fields.Item("U_BankAcct").Value != "")
            {
                businessPartner.BPBankAccounts.AccountNo = recordSet.Fields.Item("U_BankAcct").Value.ToSafeString();
                businessPartner.BPBankAccounts.BankCode = recordSet.Fields.Item("U_BankCode").Value.ToSafeString();
                businessPartner.BPBankAccounts.Country = "PE";
                businessPartner.BPBankAccounts.Add();
            }

            return businessPartner.Add() == 0;
        }

        private bool closeBP(SAPbobsCOM.Company Company, ADODB.Recordset recordSet)
        {
            string cardCode = recordSet.Fields.Item("CardCode").Value.ToSafeString();
            SAPbobsCOM.BusinessPartners businessPartner = (SAPbobsCOM.BusinessPartners)Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
            //Fields
            if (businessPartner.GetByKey(cardCode))
            {
                businessPartner.Valid = SAPbobsCOM.BoYesNoEnum.tNO;
                businessPartner.Frozen = SAPbobsCOM.BoYesNoEnum.tYES;
            }
            return businessPartner.Update() == 0;
        }

        private bool updateBP(SAPbobsCOM.Company Company, ADODB.Recordset recordSet)
        {
            string cardCode = recordSet.Fields.Item("CardCode").Value.ToSafeString(); ;
            SAPbobsCOM.BusinessPartners businessPartner = Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
            //Fields
            if (businessPartner.GetByKey(cardCode))
            {
                businessPartner.CardName = recordSet.Fields.Item("CardName").Value.ToSafeString();
                businessPartner.FederalTaxID = recordSet.Fields.Item("LicTradNum").Value.ToSafeString();
                businessPartner.DebitorAccount = recordSet.Fields.Item("DebitAccount").Value.ToSafeString();
                businessPartner.GroupCode = recordSet.Fields.Item("GroupCode").Value.ToInt32();
                businessPartner.Address = recordSet.Fields.Item("Address").Value.ToSafeString();
                businessPartner.Phone1 = recordSet.Fields.Item("Phone1").Value.ToSafeString();
                businessPartner.Phone2 = recordSet.Fields.Item("Phone2").Value.ToSafeString();
                businessPartner.Cellular = recordSet.Fields.Item("Cellular").Value.ToSafeString();
                businessPartner.EmailAddress = recordSet.Fields.Item("E_Mail").Value.ToSafeString();
                businessPartner.Currency = "##";
                businessPartner.UserFields.Fields.Item("U_BPP_BPNO").Value = recordSet.Fields.Item("U_BPP_BPNO").Value;

                if (businessPartner.BPBankAccounts.Count > 0)
                {
                    businessPartner.BPBankAccounts.AccountNo = recordSet.Fields.Item("U_BankAcct").Value.ToSafeString();
                    businessPartner.BPBankAccounts.BankCode = recordSet.Fields.Item("U_BankCode").Value.ToSafeString();
                    businessPartner.BPBankAccounts.Country = "PE";
                    //businessPartner.BPBankAccounts.Add();
                }
                else
                {
                    businessPartner.BPBankAccounts.AccountNo = recordSet.Fields.Item("U_BankAcct").Value.ToSafeString();
                    businessPartner.BPBankAccounts.BankCode = recordSet.Fields.Item("U_BankCode").Value.ToSafeString();
                    businessPartner.BPBankAccounts.Country = "PE";
                    businessPartner.BPBankAccounts.Add();
                }

                return businessPartner.Update() == 0;
            }
            else
            {
                return false;
            }
        }
    }
}
