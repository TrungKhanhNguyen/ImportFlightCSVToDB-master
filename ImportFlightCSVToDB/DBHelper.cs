using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ImportFlightCSVToDB
{
    public class DBHelper
    {
        

        private static string _cnnString = string.Empty;
        public static string getCnnString()
        {
            if (_cnnString == String.Empty)
            {
                _cnnString = ConfigurationManager.ConnectionStrings["FlyNowDB"].ConnectionString;
            }
            return _cnnString;
        }
        #region[GetDataSetSP]
        public static DataSet GetDataSetSP(string spName, List<SqlParameter> listSqlParameter)
        {
            try
            {
                //FlyNowDbContext db = new FlyNowDbContext();
                DataSet dts = new DataSet();
                SqlConnection sqlConn = new SqlConnection(getCnnString());
                sqlConn.Open();
                SqlCommand command = new SqlCommand(spName, sqlConn);
                SqlDataAdapter daReport = new SqlDataAdapter(command);
                using (command)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(listSqlParameter.ToArray());
                    daReport.Fill(dts);
                }
                sqlConn.Close();
                return dts;
            }
            catch
            {
                return null;
            }
        }
        #endregion
        #region[GetDataTableSP]
        public static DataTable GetDataTableSP(string spName, List<SqlParameter> listSqlParameter)
        {
            try
            {
                //FlyNowDbContext db = new FlyNowDbContext();
                DataSet dts = new DataSet();
                SqlConnection sqlConn = new SqlConnection(getCnnString());
                sqlConn.Open();
                SqlCommand command = new SqlCommand(spName, sqlConn);
                SqlDataAdapter daReport = new SqlDataAdapter(command);
                using (command)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(listSqlParameter.ToArray());
                    daReport.Fill(dts);
                }
                sqlConn.Close();
                return dts.Tables[0];
            }
            catch
            {
                return null;
            }
        }
        public static DataTable GetDataTableSP(string spName)
        {
            try
            {

                DataSet dts = new DataSet();
                SqlConnection sqlConn = new SqlConnection(getCnnString());
                sqlConn.Open();
                SqlCommand command = new SqlCommand(spName, sqlConn);
                SqlDataAdapter daReport = new SqlDataAdapter(command);
                using (command)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    daReport.Fill(dts);
                }
                sqlConn.Close();
                return dts.Tables[0];
            }
            catch
            {
                return null;
            }
        }
        #endregion
        #region[ExecuteScalar]
        public static object ExecuteScalarSP(string SPName)
        {

            SqlConnection sqlConn = null;
            try
            {
                sqlConn = new SqlConnection(getCnnString());
                sqlConn.Open();
                var sqlCommand = new SqlCommand(SPName, sqlConn);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                return sqlCommand.ExecuteScalar();
            }
            catch
            {
                return null;
            }
            finally
            {
                if (sqlConn != null)
                {
                    sqlConn.Close();
                }
            }
        }

        public static object ExecuteScalarSP(string SPName, List<SqlParameter> Parameters)
        {
            SqlConnection sqlConn = null;
            try
            {
                sqlConn = new SqlConnection(getCnnString());
                sqlConn.Open();
                var sqlCommand = new SqlCommand(SPName, sqlConn);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter param in Parameters)
                {
                    sqlCommand.Parameters.Add(param);
                }
                return sqlCommand.ExecuteScalar();
            }
            catch
            {
                return null;
            }
            finally
            {
                if (sqlConn != null)
                {
                    sqlConn.Close();
                }
            }
        }
        #endregion
        #region[ExecuteNonQuery]

        public static void ExecuteNonQuery(string SPName)
        {
            SqlConnection sqlConn = null;
            try
            {
                sqlConn = new SqlConnection(getCnnString());
                sqlConn.Open();
                var sqlCommand = new SqlCommand(SPName, sqlConn);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw (new Exception(ex.Message));
            }
            finally
            {
                if (sqlConn != null)
                {
                    sqlConn.Close();
                }
            }
        }

        public static void ExecuteNonQuery(string SPName, List<SqlParameter> list)
        {
            SqlConnection sqlConn = null;
            try
            {
                sqlConn = new SqlConnection(getCnnString());
                sqlConn.Open();
                var sqlCommand = new SqlCommand(SPName, sqlConn);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter param in list)
                {
                    sqlCommand.Parameters.Add(param);
                }
                sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw (new Exception(ex.Message));
            }
            finally
            {
                if (sqlConn != null)
                {
                    sqlConn.Close();
                }
            }
        }
        #endregion
        #region[GetList]
        public static List<T> GetList<T>(string SPName)
        {
            SqlConnection sqlConn = null;
            var m_List = new List<T>();
            try
            {
                sqlConn = new SqlConnection(getCnnString());
                sqlConn.Open();
                var sqlCommand = new SqlCommand(SPName, sqlConn);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                SqlDataReader dr = sqlCommand.ExecuteReader();
                if (dr == null || dr.FieldCount == 0)
                    return null;
                int fCount = dr.FieldCount;
                Type m_Type = typeof(T);
                PropertyInfo[] l_Property = m_Type.GetProperties();
                object obj;

                string pName;
                while (dr.Read())
                {
                    obj = Activator.CreateInstance(m_Type);
                    for (int i = 0; i < fCount; i++)
                    {
                        pName = dr.GetName(i);
                        if (dr[i] != DBNull.Value &&
                            l_Property.Where(a => a.Name == pName).Select(a => a.Name).Count() > 0)
                        {
                            m_Type.GetProperty(pName).SetValue(obj, dr[i], null);
                        }
                    }
                    m_List.Add((T)obj);
                }
                dr.Close();
            }
            catch (SqlException myException)
            {
                throw (new Exception(myException.Message));
            }
            finally
            {
                if (sqlConn != null)
                {
                    sqlConn.Close();
                }
            }
            return m_List;
        }

        public static List<T> GetList<T>(string SPName, List<SqlParameter> Parameters)
        {
            SqlConnection sqlConn = null;
            var m_List = new List<T>();
            try
            {
                sqlConn = new SqlConnection(getCnnString());
                sqlConn.Open();
                var sqlCommand = new SqlCommand(SPName, sqlConn);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter par in Parameters)
                {
                    sqlCommand.Parameters.Add(par);
                }
                SqlDataReader dr = sqlCommand.ExecuteReader();
                if (dr == null || dr.FieldCount == 0)
                    return null;
                int fCount = dr.FieldCount;
                Type m_Type = typeof(T);
                PropertyInfo[] l_Property = m_Type.GetProperties();
                object obj;

                string pName;
                while (dr.Read())
                {
                    obj = Activator.CreateInstance(m_Type);
                    for (int i = 0; i < fCount; i++)
                    {
                        pName = dr.GetName(i);
                        if (dr[i] != DBNull.Value &&
                            l_Property.Where(a => a.Name == pName).Select(a => a.Name).Count() > 0)
                        {
                            m_Type.GetProperty(pName).SetValue(obj, dr[i], null);
                        }
                    }
                    m_List.Add((T)obj);
                }
                dr.Close();
            }
            catch (SqlException myException)
            {
                throw (new Exception(myException.Message));
            }
            finally
            {
                if (sqlConn != null)
                {
                    sqlConn.Close();
                }
            }
            return m_List;
        }
        #endregion
    }
}
