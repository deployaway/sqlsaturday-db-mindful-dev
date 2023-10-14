using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

var scenario = args.Length == 0 ? null : args.First<String>();

var helpers = new Helpers();

if (scenario.IsNullOrEmpty()) {
    Console.WriteLine("No scenario supplied!");
    return;
}

if (scenario?.ToLower() == "connection") {
    /// 500 values insert
    var stringList = Enumerable.Range(0, 500).Select(n => "randomString-" + n.ToString()).ToList();

    var stopwatch = new Stopwatch();
    stopwatch.Start();

    foreach (String stringValue in stringList) {
        using (SqlConnection connection = new SqlConnection(helpers.GetSqlConnectionString()))
        {
            connection.Open();
            using SqlCommand insert_command = new SqlCommand("INSERT INTO dbo.TestTable VALUES (@NumParam, @StringParam)", connection);
            insert_command.Parameters.Add(new SqlParameter("NumParam", 14));
            insert_command.Parameters.Add(new SqlParameter("StringParam", stringValue));
            insert_command.ExecuteNonQuery();
        }
    }
    
    stopwatch.Stop();
    Console.WriteLine("Connection Management Demo, Approach 1: " + stopwatch.ElapsedMilliseconds + "ms");

    var stopwatch2 = new Stopwatch();
    stopwatch2.Start();

    using (SqlConnection connection = new SqlConnection(helpers.GetSqlConnectionString()))
    {
        connection.Open();
        foreach (String stringValue in stringList) {   
            using SqlCommand insert_command = new SqlCommand("INSERT INTO dbo.TestTable VALUES (@NumParam, @StringParam)", connection);
            insert_command.Parameters.Add(new SqlParameter("NumParam", 14));
            insert_command.Parameters.Add(new SqlParameter("StringParam", stringValue));
            insert_command.ExecuteNonQuery();
        }
    }
    
    stopwatch2.Stop();
    Console.WriteLine("Connection Management Demo, Approach 2: " + stopwatch2.ElapsedMilliseconds + "ms");
}

if (scenario?.ToLower() == "parameterization") {
    /// Suboptimal practice
    var parameter_good = "test";
    var parameter_bad = "test'; INSERT INTO dbo.TestTable VALUES (100, 'got here'); SELECT 'boom";

    var non_parameterized_query_good = "SELECT NumberColumn, StringColumn FROM dbo.TestTable WHERE StringColumn = '" + parameter_good + "'";
    var non_parameterized_query_bad = "SELECT NumberColumn, StringColumn FROM dbo.TestTable WHERE StringColumn = '" + parameter_bad + "'";
    using (SqlConnection connection = new SqlConnection(helpers.GetSqlConnectionString()))
    {
        connection.Open();
        using SqlCommand good_command = new SqlCommand(non_parameterized_query_good, connection);
        good_command.ExecuteNonQuery();
        using SqlCommand bad_command = new SqlCommand(non_parameterized_query_bad, connection);
        bad_command.ExecuteNonQuery();
    }

    /// Optimal practice
    var parameterized_query = "SELECT NumberColumn, StringColumn FROM dbo.TestTable WHERE StringColumn = @Parameter";
    var parameter_value = "test";
    using (SqlConnection connection = new SqlConnection(helpers.GetSqlConnectionString()))
    {
        connection.Open();
        using SqlCommand good_command = new SqlCommand(parameterized_query, connection);
        good_command.Parameters.Add(new SqlParameter("Parameter", parameter_value));
        good_command.ExecuteNonQuery();
    }
}

if (scenario?.ToLower() == "transaction") {
    /// 500 values insert
    var stringList = Enumerable.Range(0, 10).Select(n => "randomString-" + n.ToString()).ToList();
   
    using (SqlConnection connection = new SqlConnection(helpers.GetSqlConnectionString()))
    {
        connection.Open();
        foreach (String stringValue in stringList) {
            var transaction = connection.BeginTransaction();
            using SqlCommand insert_command = new SqlCommand("INSERT INTO dbo.TestTable VALUES (@NumParam, @StringParam)", connection);
            insert_command.Parameters.Add(new SqlParameter("NumParam", 14));
            insert_command.Parameters.Add(new SqlParameter("StringParam", stringValue));
            insert_command.ExecuteNonQuery();
            transaction.Commit();
        }
    }

    using (SqlConnection connection = new SqlConnection(helpers.GetSqlConnectionString()))
    {
        connection.Open();
        var transaction = connection.BeginTransaction();
        foreach (String stringValue in stringList) {
            using SqlCommand insert_command = new SqlCommand("INSERT INTO dbo.TestTable VALUES (@NumParam, @StringParam)", connection);
            insert_command.Parameters.Add(new SqlParameter("NumParam", 14));
            insert_command.Parameters.Add(new SqlParameter("StringParam", stringValue));
            insert_command.ExecuteNonQuery();
        }
        transaction.Commit();
    }
}


public class Helpers {
    public string GetSqlConnectionString() {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
        {
            DataSource = "localhost",
            UserID = "sa",
            Password = "D@tabaseMindful",
            InitialCatalog = "dbmindful",
            TrustServerCertificate = true,
            //Pooling = false,
        };

        return builder.ConnectionString;
    }
}
