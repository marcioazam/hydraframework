using System.Collections.Generic;
using System.Data.SqlClient;

namespace HydraFramework
{
    public class HydraParameters
    {
        private List<SqlParameter> SqlParameter = new List<SqlParameter>();
      
        private SqlParameter PKSqlParameter = null;

        public void Add(string nameParameter, object valor)
        {
            SqlParameter.Add(new SqlParameter("@" + nameParameter.Replace("@",""), valor));
        }

        public void AddCustom(SqlParameter sqlParameter)
        {
            SqlParameter.Add(sqlParameter);
        }

        public void AddPK(string nameParameter, object valor)
        {
        	PKSqlParameter = new SqlParameter("@" + nameParameter, valor);
        }

        public List<SqlParameter> ReturnParameters()
        {        
            return SqlParameter;
        }

        public List<SqlParameter> ReturnWithPK()
        {
            if (PKSqlParameter != null)
            {
                SqlParameter.Add(PKSqlParameter);
            }

            return SqlParameter;
        }
    }
}