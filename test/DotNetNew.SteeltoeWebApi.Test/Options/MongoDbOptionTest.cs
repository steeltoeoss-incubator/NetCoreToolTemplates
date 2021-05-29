using System.Collections.Generic;
using Steeltoe.DotNetNew.SteeltoeWebApi.Test.Utils;
using Xunit.Abstractions;

namespace Steeltoe.DotNetNew.SteeltoeWebApi.Test.Options
{
    public class MongoDbOptionTest : OptionTest
    {
        public MongoDbOptionTest(ITestOutputHelper logger) : base("mongodb", "Add access to MongoDB databases", logger)
        {
        }

        protected override void AddProjectPackages(SteeltoeVersion steeltoeVersion, Framework framework,
            List<string> packages)
        {
            packages.Add("MongoDB.Driver");
            switch (steeltoeVersion)
            {
                case SteeltoeVersion.Steeltoe2:
                    packages.Add("Steeltoe.CloudFoundry.ConnectorCore");
                    break;
                default:
                    packages.Add("Steeltoe.Connector.ConnectorCore");
                    break;
            }
        }

        protected override void AddStartupCsSnippets(SteeltoeVersion steeltoeVersion, Framework framework,
            List<string> snippets)
        {
            switch (steeltoeVersion)
            {
                case SteeltoeVersion.Steeltoe2:
                    snippets.Add("using Steeltoe.CloudFoundry.Connector.MongoDb;");
                    break;
                default:
                    snippets.Add("using Steeltoe.Connector.MongoDb;");
                    break;
            }

            snippets.Add("services.AddMongoClient(Configuration);");
        }

        protected override void AddValuesControllerCsSnippets(SteeltoeVersion steeltoeVersion, Framework framework,
            List<string> snippets)
        {
            snippets.Add("using MongoDB.Driver;");
            // snippets.Add("using System.Data;");
            snippets.Add(@"
private readonly IMongoClient _mongoClient;
private readonly MongoUrl _mongoUrl;
public ValuesController(IMongoClient mongoClient, MongoUrl mongoUrl)
{
    _mongoClient = mongoClient;
    _mongoUrl = mongoUrl;
}
");
            snippets.Add(@"
[HttpGet]
public ActionResult<IEnumerable<string>> Get()
{
    List<string> listing = _mongoClient.ListDatabaseNames().ToList();
    listing.Insert(0, _mongoUrl.Url);
    return listing;
}
");
        }
    }
}