using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SICER.MIGRACION.Helper
{
    public static class ConstantHelper
    {
        [Obsolete]
        public static String SICER_INT_SBO1 { get; }
        /// <summary>
        /// DataBase Name
        /// </summary>
        public const String SICER_INT_SBO = "SICER_INT_SBO";

        public static class StoreProcedureNames
        {
            [Obsolete]
            public static String SEI_STW_BusinessPartners { get; }
            public const String MSS_SP_SICER_BUSINESSPARTNERS = "MSS_SP_SICER_BUSINESSPARTNERS";

            [Obsolete]
            public static String SEI_STW_PurchInvoices { get; }
            public const String MSS_SP_SICER_PURCHASEINVOICES = "MSS_SP_SICER_PURCHASEINVOICES";

            [Obsolete]
            public static String SEI_STW_PurchInvoicesLines { get; }
            public const String MSS_SP_SICER_PURCHASEINVOICESLINES = "MSS_SP_SICER_PURCHASEINVOICESLINES";

            [Obsolete]
            public static String SEI_STW_JournalEntriesHeader { get; }
            public const String MSS_SP_SICER_JOURNALENTRIESHEADER = "MSS_SP_SICER_JOURNALENTRIESHEADER";

            [Obsolete]
            public static String SEI_STW_JournalEntriesLines { get; }
            public const String MSS_SP_SICER_JOURNALENTRIESLINES = "MSS_SP_SICER_JOURNALENTRIESLINES";

            [Obsolete]
            public static String SEI_STW_AccountPayments { get; }
            public const String MSS_SP_SICER_ACCOUNTPAYMENTS = "MSS_SP_SICER_ACCOUNTPAYMENTS";

            [Obsolete]
            public static String SEI_STW_Refacturables { get; }
            public const String MSS_SP_SICER_REFACTURABLES = "MSS_SP_SICER_REFACTURABLES";

            [Obsolete]
            public static String SEI_STW_PayrollInvoices { get; }
            public const String MSS_SP_SICER_PAYROLLINVOICES = "MSS_SP_SICER_PAYROLLINVOICES";

            [Obsolete]
            public static String SEI_STW_PendingInvoices { get; }
            public const String MSS_SP_SICER_PENDINGINVOICES = "MSS_SP_SICER_PENDINGINVOICES";

            [Obsolete]
            public static String SEI_STW_WebUDOInvoices { get; }
            public const String MSS_SP_SICER_WEBUDOINVOICES = "MSS_SP_SICER_WEBUDOINVOICES";

            [Obsolete]
            public static String SEI_STW_AccountInfo { get; }
            public const String MSS_SP_SICER_ACCOUNTINFO = "MSS_SP_SICER_ACCOUNTINFO";

            [Obsolete]
            public static String SEI_STW_PrimeraEtapa { get; }
            public const String MSS_SP_SICER_PRIMERAETAPA = "MSS_SP_SICER_PRIMERAETAPA";

            [Obsolete]
            public static String SEI_STW_RendicionesSegundaEtapa { get; }
            public const String MSS_SP_SICER_RENDICIONESSEGUNDAETAPA = "MSS_SP_SICER_RENDICIONESSEGUNDAETAPA";

            [Obsolete]
            public static String SEI_STW_DatosEREtapa1 { get; }
            public const String MSS_SP_SICER_DATOSERETAPA1 = "MSS_SP_SICER_DATOSERETAPA1";

            [Obsolete]
            public static String SEI_STW_FacturacionWeb { get; }
            public const String MSS_SP_SICER_FACTURACIONWEB = "MSS_SP_SICER_FACTURACIONWEB";

            [Obsolete]
            public static String SEI_STW_FacturacionWebDetalle { get; }
            public const String MSS_SP_SICER_FACTURACIONWEBDETALLE = "MSS_SP_SICER_FACTURACIONWEBDETALLE";

            [Obsolete]
            public static String SEI_ATW_CuentasPago { get; }
            public const String MSS_SP_SICER_CUENTASPAGO = "MSS_SP_SICER_CUENTASPAGO";

            [Obsolete]
            public static String SEI_STW_CuentasValidas { get; }
            public const String MSS_SP_SICER_CUENTASVALIDAS = "MSS_SP_SICER_CUENTASVALIDAS";

            [Obsolete]
            public static String SEI_STW_FacturasRendidasER { get; }
            public const String MSS_SP_SICER_FACTURASRENDIDASER = "MSS_SP_SICER_FACTURASRENDIDASER";

            [Obsolete]
            public static String SEI_STW_GruposFacturacion { get; }
            public const String MSS_SP_SICER_GRUPOSFACTURACION = "MSS_SP_SICER_GRUPOSFACTURACION";

            [Obsolete]
            public static String SEI_STW_TrasladoFacturas { get; }
            public const String MSS_SP_SICER_TRASLADOFACTURAS = "MSS_SP_SICER_TRASLADOFACTURAS";

        }
    }
}
