using Microsoft.Data.SqlClient;

public class TransactionScenario {
    public void RunScenario() {
        /// 500 values insert
        var stringList = Enumerable.Range(0, 10).Select(n => "randomString-" + n.ToString()).ToList();
    
        using (SqlConnection connection = new SqlConnection(helpers.GetSqlConnectionString()))
        {
            connection.Open();
            foreach (String stringValue in stringList) {
                var transaction = connection.BeginTransaction();
                using SqlCommand insert_command = new SqlCommand("INSERT INTO dbo.TestTable VALUES (@NumParam, @StringParam)", connection, transaction);
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
                using SqlCommand insert_command = new SqlCommand("INSERT INTO dbo.TestTable VALUES (@NumParam, @StringParam)", connection, transaction);
                insert_command.Parameters.Add(new SqlParameter("NumParam", 14));
                insert_command.Parameters.Add(new SqlParameter("StringParam", stringValue));
                insert_command.ExecuteNonQuery();
            }
            transaction.Commit();
        }
    }

    Helpers helpers = new Helpers();
}