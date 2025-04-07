using Newtonsoft.Json;

namespace MarkdownImageAPI.Handlers;

public class MarkdownRequestArgs
{
    [JsonProperty("auto_width")]
    public bool AutoWidth { get; init; } = true;

    [JsonProperty("auto_height")]
    public bool AutoHeight { get; init; } = true;

    [JsonProperty("timeout")]
    public int TimeOut { get; init; } = 5000;

    [JsonProperty("enable_dark")]
    public bool Dark { get; init; } = false;

    [JsonProperty("content")]
    public string MarkdownContent { get; init; } = string.Empty;
}
