namespace ApplicationConfigurationService;

public sealed class ApplicationSettingDto
{
    public int SettingId { get; set; }

    public int? ParentId { get; set; }

    public string? Key { get; set; }

    public string? Value { get; set; }

    public int? Index { get; set; }
}
