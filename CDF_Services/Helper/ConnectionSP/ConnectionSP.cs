using CDF_Infrastructure.Persistence.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.Helper.ConnectionSP
{
    public class ConnectionSP
    {
        private ApplicationDBContext _dbContext;

        public ConnectionSP(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public static DataTable ExcuteStoreProcedureReturnOne(string SpName, Dictionary<string, object> parameters)
        {
            DataTable table1 = new DataTable();
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var environmentFile = "appSettings." + environmentName + ".json";
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile(environmentFile, optional: true, reloadOnChange: true);
            IConfiguration _configuration = builder.Build();
            var connectionString = _configuration.GetConnectionString("myConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = SpName;
            command.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
                foreach (KeyValuePair<string, object> kvp in parameters)
                    command.Parameters.Add(new SqlParameter(kvp.Key, kvp.Value));
            //set sproc params

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            if (dataSet.Tables.Count > 0)
                table1 = dataSet.Tables[0];
            DataSet ds = new DataSet();
            connection.Close();

            return table1;
        }

        public static Tuple<DataTable, DataTable> ExcuteStoreProcedure(string SpName, Dictionary<string, object> parameters)
        {
            DataTable table1 = new DataTable();
            DataTable table2 = new DataTable();

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var environmentFile = "appSettings." + environmentName + ".json";
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile(environmentFile, optional: true, reloadOnChange: true);
            IConfiguration _configuration = builder.Build();
            var connectionString = _configuration.GetConnectionString("myConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = SpName;
            command.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
                foreach (KeyValuePair<string, object> kvp in parameters)
                    command.Parameters.Add(new SqlParameter(kvp.Key, kvp.Value));
            //set sproc params

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            if (dataSet.Tables.Count > 0)
                table1 = dataSet.Tables[0];
            if (dataSet.Tables.Count > 1)
                table2 = dataSet.Tables[1];
            DataSet ds = new DataSet();
            connection.Close();

            return Tuple.Create(table1, table2);
        }
        public static Tuple<DataTable, DataTable, DataTable> ExcuteStoreProcedureReturnThree(string SpName, Dictionary<string, object> parameters)
        {
            DataTable table1 = new DataTable();
            DataTable table2 = new DataTable();
            DataTable table3 = new DataTable();

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var environmentFile = "appSettings." + environmentName + ".json";

            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile(environmentFile, optional: true, reloadOnChange: true);
            IConfiguration _configuration = builder.Build();
            var connectionString = _configuration.GetConnectionString("myConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = SpName;
            command.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
                foreach (KeyValuePair<string, object> kvp in parameters)
                    command.Parameters.Add(new SqlParameter(kvp.Key, kvp.Value));
            //set sproc params

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            if (dataSet.Tables.Count > 0)
                table1 = dataSet.Tables[0];
            if (dataSet.Tables.Count > 1)
                table2 = dataSet.Tables[1];
            if (dataSet.Tables.Count > 1)
                table3 = dataSet.Tables[2];
            DataSet ds = new DataSet();
            connection.Close();

            return Tuple.Create(table1, table2, table3);
        }

        public static Tuple<DataTable, DataTable, DataTable, DataTable> ExcuteStoreProcedureReturnFour(string SpName, Dictionary<string, object> parameters)
        {
            DataTable table1 = new DataTable();
            DataTable table2 = new DataTable();
            DataTable table3 = new DataTable();
            DataTable table4 = new DataTable();

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var environmentFile = "appSettings." + environmentName + ".json";

            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile(environmentFile, optional: true, reloadOnChange: true);
            IConfiguration _configuration = builder.Build();
            var connectionString = _configuration.GetConnectionString("myConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = SpName;
            command.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
                foreach (KeyValuePair<string, object> kvp in parameters)
                    command.Parameters.Add(new SqlParameter(kvp.Key, kvp.Value));
            //set sproc params

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            if (dataSet.Tables.Count > 0)
                table1 = dataSet.Tables[0];
            if (dataSet.Tables.Count > 1)
                table2 = dataSet.Tables[1];
            if (dataSet.Tables.Count > 1)
                table3 = dataSet.Tables[2];
            if (dataSet.Tables.Count > 1)
                table4 = dataSet.Tables[3];
            DataSet ds = new DataSet();
            connection.Close();

            return Tuple.Create(table1, table2, table3, table4);
        }
        public static Tuple<DataTable, DataTable, DataTable, DataTable, DataTable> ExcuteStoreProcedureReturnFive(string SpName, Dictionary<string, object> parameters)
        {
            DataTable table1 = new DataTable();
            DataTable table2 = new DataTable();
            DataTable table3 = new DataTable();
            DataTable table4 = new DataTable();
            DataTable table5 = new DataTable();


            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var environmentFile = "appSettings." + environmentName + ".json";

            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile(environmentFile, optional: true, reloadOnChange: true);
            IConfiguration _configuration = builder.Build();
            var connectionString = _configuration.GetConnectionString("myConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = SpName;
            command.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
                foreach (KeyValuePair<string, object> kvp in parameters)
                    command.Parameters.Add(new SqlParameter(kvp.Key, kvp.Value));
            //set sproc params

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            if (dataSet.Tables.Count > 0)
                table1 = dataSet.Tables[0];
            if (dataSet.Tables.Count > 1)
                table2 = dataSet.Tables[1];
            if (dataSet.Tables.Count > 1)
                table3 = dataSet.Tables[2];
            if (dataSet.Tables.Count > 1)
                table4 = dataSet.Tables[3];
            if (dataSet.Tables.Count > 1)
                table5 = dataSet.Tables[4];
            DataSet ds = new DataSet();
            connection.Close();

            return Tuple.Create(table1, table2, table3, table4, table5);
        }
        public static Tuple<DataTable, DataTable, DataTable, DataTable, DataTable, DataTable> ExcuteStoreProcedureReturnSix(string SpName, Dictionary<string, object> parameters)
        {
            DataTable table1 = new DataTable();
            DataTable table2 = new DataTable();
            DataTable table3 = new DataTable();
            DataTable table4 = new DataTable();
            DataTable table5 = new DataTable();
            DataTable table6 = new DataTable();


            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var environmentFile = "appSettings." + environmentName + ".json";

            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile(environmentFile, optional: true, reloadOnChange: true);
            IConfiguration _configuration = builder.Build();
            var connectionString = _configuration.GetConnectionString("myConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = SpName;
            command.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
                foreach (KeyValuePair<string, object> kvp in parameters)
                    command.Parameters.Add(new SqlParameter(kvp.Key, kvp.Value));
            //set sproc params

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            if (dataSet.Tables.Count > 0)
                table1 = dataSet.Tables[0];
            if (dataSet.Tables.Count > 1)
                table2 = dataSet.Tables[1];
            if (dataSet.Tables.Count > 1)
                table3 = dataSet.Tables[2];
            if (dataSet.Tables.Count > 1)
                table4 = dataSet.Tables[3];
            if (dataSet.Tables.Count > 1)
                table5 = dataSet.Tables[4];
            if (dataSet.Tables.Count > 1)
                table6 = dataSet.Tables[5];
            DataSet ds = new DataSet();
            connection.Close();

            return Tuple.Create(table1, table2, table3, table4, table5, table6);
        }
        public static DataTable ExcuteStoreProcedureReturnOne(string SpName)
        {
            DataTable table1 = new DataTable();
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var environmentFile = "appSettings." + environmentName + ".json";
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile(environmentFile, optional: true, reloadOnChange: true);
            IConfiguration _configuration = builder.Build();
            var connectionString = _configuration.GetConnectionString("myConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = SpName;
            command.CommandType = CommandType.StoredProcedure;
            //set sproc params

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            if (dataSet.Tables.Count > 0)
                table1 = dataSet.Tables[0];
            DataSet ds = new DataSet();
            connection.Close();

            return table1;
        }
        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
    }
}
