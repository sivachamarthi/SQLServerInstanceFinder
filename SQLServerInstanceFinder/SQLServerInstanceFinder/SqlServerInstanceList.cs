using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLServerInstanceFinder
{
    /// <summary>
    /// Class to get all the sql server instances in a network.
    /// </summary>
    public static class SqlServerInstanceList
    {
        public static List<String> Get()
        {

            SqlDataSourceEnumerator sqlSource = SqlDataSourceEnumerator.Instance;

            DataTable dtSQLInstances = sqlSource.GetDataSources();

            List<String> lstSQLInstances = new List<string>();

            string serverName = null, instanceName = null;
            foreach (DataRow record in dtSQLInstances.Rows)
            {
                serverName = record["ServerName"].ToString();
                instanceName = record["InstanceName"] != null ? record["InstanceName"].ToString() : "";

                lstSQLInstances.Add(String.IsNullOrEmpty(instanceName) ? serverName : serverName + "\\" + instanceName);
            }

            return lstSQLInstances;
        }
    }
}
