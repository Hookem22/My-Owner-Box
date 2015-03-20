using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Reflection;

/// <summary>
/// Summary description for BaseClass
/// </summary>
public abstract class BaseClass<T>
{
    public BaseClass()
    {
    }
    #region Properties

    public int Id { get; set; }

    #endregion //Properties

    #region Load
    public static T LoadById(int id)
    {
        return LoadByPropName("Id", id.ToString())[0];
    }
    public static List<T> LoadByPropName(string propName, string propValue)
    {
        List<T> all = new List<T>();
        SqlConnection conn = null;
        SqlDataReader rdr = null;

        try
        {
            string connection = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            conn = new SqlConnection(connection);
            conn.Open();

            string table = typeof(T).Name;
            string properties = "";
            foreach (var prop in typeof(T).GetProperties())
            {
                if (prop.GetCustomAttributes(typeof(NonDB), false).Length > 0)
                    continue;

                properties += prop.Name + ", ";
            }
            properties = properties.Remove(properties.LastIndexOf(","));

            string sql = string.Format("SELECT {0} FROM {1} WHERE {2} = @{2}", properties, table, propName);

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new SqlParameter("@" + propName, propValue));

            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                T item = (T)typeof(T).GetConstructor(new Type[] { }).Invoke(new object[0]);
                foreach (var prop in typeof(T).GetProperties())
                {
                    if (prop.GetCustomAttributes(typeof(NonDB), false).Length > 0)
                        continue;

                    Type type = prop.PropertyType;
                    string readerValue = string.Empty;

                    if (rdr[prop.Name] != DBNull.Value)
                    {
                        readerValue = rdr[prop.Name].ToString();
                    }

                    if (!string.IsNullOrEmpty(readerValue))
                    {
                        try
                        {
                            prop.SetValue(item, Convert.ChangeType(readerValue, type), null);
                        }
                        catch
                        {
                            DateTime dateTime;
                            if (DateTime.TryParse(readerValue, out dateTime))
                                prop.SetValue(item, dateTime, null);
                        }
                    }
                }
                all.Add(item);
            }
        }
        finally
        {
            if (conn != null)
                conn.Close();
            if (rdr != null)
                rdr.Close();
        }

        return all;
    }
    public static List<T> LoadAll()
    {
        List<T> all = new List<T>();
        SqlConnection conn = null;
        SqlDataReader rdr = null;

        try
        {
            string connection = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            conn = new SqlConnection(connection);
            conn.Open();

            string table = typeof(T).Name;
            string properties = "";
            foreach (var prop in typeof(T).GetProperties())
            {
                if (prop.GetCustomAttributes(typeof(NonDB), false).Length > 0)
                    continue;

                properties += prop.Name + ", ";
            }
            properties = properties.Remove(properties.LastIndexOf(","));

            string sql = string.Format("SELECT {0} FROM {1}", properties, table);

            SqlCommand cmdGet = new SqlCommand(sql, conn);
            cmdGet.CommandType = CommandType.Text;

            rdr = cmdGet.ExecuteReader();

            while (rdr.Read())
            {
                T item = (T)typeof(T).GetConstructor(new Type[] { }).Invoke(new object[0]);
                foreach (var prop in typeof(T).GetProperties())
                {
                    if (prop.GetCustomAttributes(typeof(NonDB), false).Length > 0)
                        continue;

                    Type type = prop.PropertyType;
                    dynamic readerValue = null;

                    if (rdr[prop.Name] != DBNull.Value)
                    {
                        readerValue = rdr[prop.Name];
                    }
                    if (readerValue != null)
                    {
                        prop.SetValue(item, readerValue);
                        //prop.SetValue(item, Convert.ChangeType(readerValue, type), null);
                    }
                }
                all.Add(item);
            }
        }
        finally
        {
            if (conn != null)
                conn.Close();
            if (rdr != null)
                rdr.Close();
        }

        return all;
    }
    #endregion //Load

    #region Data
    public void Save()
    {
        string table = this.GetType().Name;
        T obj = (T)this.MemberwiseClone();
        string sql = "";
        if (this.Id > 0)
        {
            string properties = "";
            foreach (var prop in typeof(T).GetProperties())
            {
                if (prop.Name == "Id" || prop.GetCustomAttributes(typeof(NonDB), false).Length > 0 || prop.GetValue(obj) == null)
                    continue;

                properties += string.Format("{0} = @{0}, ", prop.Name);
            }
            properties = properties.Remove(properties.LastIndexOf(","));

            string idString = "Id = @Id";
            sql = string.Format("UPDATE {0} SET {1} WHERE {2}", table, properties, idString);
        }
        else
        {
            string propertyNames = "";
            string propertyValues = "";
            foreach (var prop in typeof(T).GetProperties())
            {
                if (prop.Name == "Id" || prop.GetCustomAttributes(typeof(NonDB), false).Length > 0 || prop.GetValue(obj) == null)
                    continue;

                propertyNames += prop.Name + ", ";
                propertyValues += "@" + prop.Name + ", ";
            }
            propertyNames = propertyNames.Remove(propertyNames.LastIndexOf(","));
            propertyValues = propertyValues.Remove(propertyValues.LastIndexOf(","));

            sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2}) SET @Id = SCOPE_IDENTITY()", table, propertyNames, propertyValues);
        }

        SqlConnection conn = null;
        string connection = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        conn = new SqlConnection(connection);

        SqlCommand cmd = new SqlCommand(sql, conn);
        SqlParameter idParameter = new SqlParameter("@Id", SqlDbType.Int);
        if (this.Id != 0)
            idParameter.Value = this.Id;
        else
            idParameter.Direction = ParameterDirection.Output;

        cmd.Parameters.Add(idParameter);
        foreach (var prop in typeof(T).GetProperties())
        {
            if (prop.Name == "Id" || prop.GetCustomAttributes(typeof(NonDB), false).Length > 0 || prop.GetValue(obj) == null)
                continue;

            try
            {
                cmd.Parameters.Add(new SqlParameter("@" + prop.Name, typeof(T).GetProperty(prop.Name).GetValue(obj, null)));
            }
            catch 
            {
                cmd.Parameters.Add(new SqlParameter("@" + prop.Name, ""));
            }
        }

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();

        if (this.Id == 0)
            this.Id = (Int32)cmd.Parameters["@Id"].Value;
    
    }

    public void Delete()
    {
        string table = this.GetType().Name;
        string sql = string.Format("DELETE FROM {0} WHERE Id = @Id", table);

        SqlConnection conn = null;
        string connection = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        conn = new SqlConnection(connection);

        SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add(new SqlParameter("@Id", this.Id));

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();
    }
    #endregion //Data
}
