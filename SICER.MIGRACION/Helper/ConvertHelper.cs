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

        public static String ToSafeString(this object val)
        {
            return (val ?? String.Empty).ToString();
        }

        public static Int32 ToInt32(this object val)
        {
            Int32 value = 0;
            try
            {
                value = Convert.ToInt32(val);
            }
            catch(Exception ex)
            {
            }
            finally
            {
            }
            return value;

        }

        public static Double ToDouble(this object val)
        {
            Double value = 0;
            try
            {
                value = Convert.ToDouble(val);
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
            return value;

        }
        /*
        public static void DoQuery(this Recordset recordset, String query)
        {
            recordset = new SQLConnection().DoQuery(query);
        }*/
    }
}
