using System;
using System.Collections.Generic;
using FluentAssertions;
using Steeltoe.NetCoreTool.Template.Test.Utilities.Models;
using Steeltoe.NetCoreTool.Template.WebApi.Test.Utils;
using Xunit.Abstractions;

namespace Steeltoe.NetCoreTool.Template.WebApi.Test
{
    public class ConfigurationPlaceholderOptionTest : ProjectOptionTest
    {
        public ConfigurationPlaceholderOptionTest(ITestOutputHelper logger) : base("configuration-placeholder",
            "Add a placeholder configuration source", logger)
        {
        }

        protected override void AssertCsprojPackagesHook(SteeltoeVersion steeltoeVersion, Framework framework,
            List<(string, string)> packages)
        {
            packages.Add(("Steeltoe.Extensions.Configuration.PlaceholderCore", "$(SteeltoeVersion)"));
        }

        protected override void AssertProgramCsSnippetsHook(SteeltoeVersion steeltoeVersion, Framework framework,
            List<string> snippets)
        {
            if (steeltoeVersion < SteeltoeVersion.Steeltoe30)
            {
                snippets.Add("using Steeltoe.Extensions.Configuration.PlaceholderCore;");
            }
            else
            {
                snippets.Add("using Steeltoe.Extensions.Configuration.Placeholder;");
            }

            snippets.Add(".AddPlaceholderResolver()");
        }

        protected override void AssertAppSettingsJsonHook(
            List<Action<SteeltoeVersion, Framework, AppSettings>> assertions)
        {
            assertions.Add(AssertPlaceholderSettings);
        }

        private void AssertPlaceholderSettings(SteeltoeVersion steeltoeVersion, Framework framework,
            AppSettings settings)
        {
            settings.ResolvedPlaceholderFromEnvVariables.Should().Be("${PATH?NotFound}");
            settings.ResolvedPlaceholderFromJson.Should()
                .Be("${Logging:LogLevel:System?${Logging:LogLevel:Default}}");
            settings.UnresolvedPlaceholder.Should().Be("${SomKeyNotFound?NotFound}");
        }
    }
}
