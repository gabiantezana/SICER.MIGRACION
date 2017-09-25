using SICER.MIGRACION.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SICER.MIGRACION.Connections
{
    public class SQLConnection
    {
        ADODB.Recordset _Recordset { get; set; }
        private static ADODB.Connection _Connection { get; set; }

        public ADODB.Recordset DoQuery(String SQLquery)
        {
            try
            {
                String _connectionStringFromEF = System.Configuration.ConfigurationManager.ConnectionStrings["SQLRECORDSETS"].ConnectionString;
                String _connectionString = "Provider = SQLOLEDB; " + _connectionStringFromEF; // Data Source = LAPTOP-GAP\\SQL2012; Initial Catalog = SICER_INT_SBO;User ID = sa; Password = root";

                String ASFD = new SICER_INT_SBOEntities().Database.Connection.ConnectionString;

                _Connection = new ADODB.Connection();
                _Recordset = new ADODB.Recordset();

                _Connection.Open(_connectionString, String.Empty, String.Empty, -1);
                object obj = new object();
                _Recordset = _Connection.Execute("SET NOCOUNT ON; " + SQLquery, out obj);
                //_Connection.Close();

                return _Recordset;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
               /* System.Runtime.InteropServices.Marshal.ReleaseComObject(_Connection);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_Recordset);
                _Connection = null;
                _Recordset = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                */
            }
        }

        /*
        public static ADODB.Connection GetConnection()
        {
            if (_Connection == null)
            {
                String _connectionStringFromEF = System.Configuration.ConfigurationManager.ConnectionStrings["SQLRECORDSETS"].ConnectionString;
                String _connectionString = "Provider = SQLOLEDB; " + _connectionStringFromEF;
                _Connection = new ADODB.Connection();
                _Connection.Open(_connectionString, String.Empty, String.Empty, -1);
            }
            else if (_Connection.State != 1)
            {
                String _connectionStringFromEF = System.Configuration.ConfigurationManager.ConnectionStrings["SQLRECORDSETS"].ConnectionString;
                String _connectionString = "Provider = SQLOLEDB; " + _connectionStringFromEF;
                _Connection = new ADODB.Connection();
                _Connection.Open(_connectionString, String.Empty, String.Empty, -1);
            }
            return _Connection;
        }*/

        /*
        public void DoQuery(String SQLquery, out ADODB.Recordset recordset)
        {
            try
            {
                String _connectionStringFromEF = System.Configuration.ConfigurationManager.ConnectionStrings["SQLRECORDSETS"].ConnectionString;
                String _connectionString = "Provider = SQLOLEDB; " + _connectionStringFromEF; // Data Source = LAPTOP-GAP\\SQL2012; Initial Catalog = SICER_INT_SBO;User ID = sa; Password = root";

                String ASFD = new SICER_INT_SBOEntities().Database.Connection.ConnectionString;

                _Connection = new ADODB.Connection();
                _Recordset = new ADODB.Recordset();

                _Connection.Open(_connectionString, String.Empty, String.Empty, -1);
                object obj = new object();
                _Recordset = _Connection.Execute(SQLquery, out obj);
                //_Connection.Close();
                recordset = _Recordset;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_Connection);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_Recordset);
                _Connection = null;
                _Recordset = null;

                //GC.Collect();
                //GC.WaitForPendingFinalizers();

            }
        }*/
    }
}
