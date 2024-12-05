namespace ApplicationConfigurationService;

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

public sealed class ConfigurationService
{
    private readonly List<ApplicationSettingDto> applicationSettingDtos =
    [
        new ApplicationSettingDto{ SettingId = 1, ParentId = null, Key = "Logging", Value = null, Index = null },
        new ApplicationSettingDto{ SettingId = 2, ParentId = 1, Key = "LogLevel", Value = null, Index = null },
        new ApplicationSettingDto{ SettingId = 3, ParentId = 2, Key = "Default", Value = "Information", Index = null },
        new ApplicationSettingDto{ SettingId = 4, ParentId = 2, Key = "Microsoft", Value = "Warning", Index = null },
        new ApplicationSettingDto{ SettingId = 5, ParentId = 1, Key = "ExcludedCategories", Value = null, Index = null },
        new ApplicationSettingDto{ SettingId = 6, ParentId = 5, Key = null, Value = "Category1", Index = 0 },
        new ApplicationSettingDto{ SettingId = 7, ParentId = 5, Key = null, Value = "Category2", Index = 1 },
        new ApplicationSettingDto{ SettingId = 8, ParentId = null, Key = "Individual", Value = "Glen Wilkin", Index = 1 },
        new ApplicationSettingDto{ SettingId = 9, ParentId = null, Key = "ConnectionStrings", Value = null, Index = null },
        new ApplicationSettingDto{ SettingId = 10, ParentId = 9, Key = "ConnString1", Value = "server=something", Index = null },
        new ApplicationSettingDto{ SettingId = 11, ParentId = 9, Key = "ConnString2", Value = "server=somethingelse", Index = null }
    ];

    private const string NullKeyForNonArrayElementError = "Key cannot be null for non-array elements.";

    private IConfiguration? configuration;
    public IConfiguration Configuration
    {
        get
        {
            if (this.configuration == null)
            {
                var hierarchicalData = BuildHierarchy(this.applicationSettingDtos);
                var json = JsonConvert.SerializeObject(hierarchicalData, Formatting.Indented);
                this.configuration = new ConfigurationBuilder()
                    .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
                    .Build();
            }

            return this.configuration;
        }
    }

    private static Dictionary<string, object> BuildHierarchy(List<ApplicationSettingDto> settings)
    {
        var lookup = settings.ToLookup(s => s.ParentId);
        var retVal = (Dictionary<string, object>)BuildNode(null)!;
        return retVal;

        object BuildNode(int? parentId)
        {
            var children = lookup[parentId]
                .OrderBy(s => s.Index)
                .Select(
                    setting => setting.Key == null &&
                    setting.Value != null
                        ? (object)setting.Value
                        : new KeyValuePair<string, object>(
                            setting.Key ?? throw new InvalidOperationException(NullKeyForNonArrayElementError),
                            setting.Value ?? BuildNode(setting.SettingId)
                    ));

            return parentId != null && children.All(c => c is string)
                ? children.Cast<string>().ToArray()
                : children
                .OfType<KeyValuePair<string, object>>()
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}
