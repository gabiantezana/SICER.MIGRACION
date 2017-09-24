using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SICER.MIGRACION.Helper
{
    public class FunctionsHelper
    {
        public static String Exec(String storeProcedureName)
        {
            return "Exec [" + storeProcedureName + "]";
        }
    }
}
