using System.Diagnostics;
using Microsoft.Data.SqlClient;

public class ConnectionScenario {
    public void RunScenario() {
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

    Helpers helpers = new Helpers();
}