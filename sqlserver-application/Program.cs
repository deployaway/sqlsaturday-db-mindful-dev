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
    new ConnectionScenario().RunScenario();
}

if (scenario?.ToLower() == "parameterization") {
    new ParameterizationScenario().RunScenario();
}

if (scenario?.ToLower() == "transaction") {
    new TransactionScenario().RunScenario();
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
