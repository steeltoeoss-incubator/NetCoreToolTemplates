using System.Collections.Generic;
using Steeltoe.NetCoreTool.Template.WebApi.Test.Utils;
using Xunit.Abstractions;

namespace Steeltoe.NetCoreTool.Template.WebApi.Test
{
    public class SqlServerProjectOptionTest : ProjectOptionTest
    {
        public SqlServerProjectOptionTest(ITestOutputHelper logger) : base("sqlserver",
            "Add access to Microsoft SQL Server databases",
            logger)
        {
        }

        protected override void AssertCsprojPackagesHook(SteeltoeVersion steeltoeVersion, Framework framework,
            List<(string, string)> packages)
        {
            packages.Add(("System.Data.SqlClient", "4.8.*"));
            if (steeltoeVersion < SteeltoeVersion.Steeltoe30)
            {
                packages.Add(("Steeltoe.CloudFoundry.ConnectorCore", "$(SteeltoeVersion)"));
            }
            else
            {
                packages.Add(("Steeltoe.Connector.ConnectorCore", "$(SteeltoeVersion)"));
            }
        }

        protected override void AssertStartupCsSnippetsHook(SteeltoeVersion steeltoeVersion, Framework framework,
            List<string> snippets)
        {
            if (steeltoeVersion < SteeltoeVersion.Steeltoe30)
            {
                snippets.Add("using Steeltoe.CloudFoundry.Connector.SqlServer;");
            }
            else
            {
                snippets.Add("using Steeltoe.Connector.SqlServer;");
            }

            snippets.Add("services.AddSqlServerConnection(Configuration);");
        }

        protected override void AssertValuesControllerCsSnippetsHook(SteeltoeVersion steeltoeVersion,
            Framework framework,
            List<string> snippets)
        {
            snippets.Add("using System.Data;");
            snippets.Add("using System.Data.SqlClient;");
            snippets.Add("private readonly SqlConnection _dbConnection;");
            snippets.Add(@"
private readonly SqlConnection _dbConnection;
public ValuesController([FromServices] SqlConnection dbConnection)
{
    _dbConnection = dbConnection;
}
");
            snippets.Add(@"
[HttpGet]
public ActionResult<IEnumerable<string>> Get()
{
    List<string> tables = new List<string>();
    _dbConnection.Open();
    DataTable dt = _dbConnection.GetSchema(""Tables"");
    _dbConnection.Close();
    foreach (DataRow row in dt.Rows)
    {
        string tablename = (string)row[2];
        tables.Add(tablename);
    }
    return tables;
}
");
        }
    }
}
