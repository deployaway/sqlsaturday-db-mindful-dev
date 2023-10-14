using System.Diagnostics;
using Microsoft.Data.SqlClient;

public class ParameterizationScenario {
    public void RunScenario() {
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

    Helpers helpers = new Helpers();
}