using SICER.MIGRACION.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SICER.MIGRACION.Connections
{
    public class SQLConnection
    {
        private ADODB.Connection _Connection { get; set; }
        private ADODB.Recordset _Recordset { get; set; }

        public ADODB.Recordset DoQuery(String SQLquery)
        {
            try
            {
                String _connectionStringFromEF = new SICER_INT_SBOEntities().Database.Connection.ConnectionString;
                String _connectionString = "Provider = SQLOLEDB; " + _connectionStringFromEF; // Data Source = LAPTOP-GAP\\SQL2012; Initial Catalog = SICER_INT_SBO;User ID = sa; Password = root";
                _Connection = new ADODB.Connection();
                _Connection.Open(_connectionString, String.Empty, String.Empty, -1);

                _Recordset = new ADODB.Recordset();
                _Recordset.Open(SQLquery, _Connection, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockBatchOptimistic, 0);

                //_Connection.Close();
                return _Recordset;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
