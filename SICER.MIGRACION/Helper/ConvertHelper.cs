using ADODB;
using SICER.MIGRACION.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SICER.MIGRACION.Helper
{
    public static class ConvertHelper
    {
        public static Field Item(this Fields fields, String item)
        {
            return fields[item];
        }
        public static void DoQuery(this Recordset recordset, String query)
        {
            recordset = new SQLConnection().DoQuery(query);
        }
    }
}
