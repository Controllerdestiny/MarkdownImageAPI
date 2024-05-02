using Newtonsoft.Json;

namespace MarkdownImageAPI;

public class Config
{
    [JsonProperty("端口")]
    public int Prot { get; internal set; } = 7776;

    [JsonProperty("内核路径", NullValueHandling = NullValueHandling.Ignore)]
    public string ChromePath { get; internal set; }

    [JsonProperty("日志路径")]
    public string LogPath { get; internal set; } = "Log";

    [JsonProperty("日志大小")]
    public int LogSize { get; internal set; } = 32;

    [JsonProperty("启用无头")]
    public bool EnableHeadLess { get; internal set; } = false;

    [JsonProperty("内核启动参数")]
    public List<string> ChromeCommandArgs { get; internal set; } = [];

    [JsonIgnore]
    public static string PATH => Path.Combine(Environment.CurrentDirectory, "Config.json");

    public static Config Read()
    {
        if (File.Exists(PATH))
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(PATH)) ?? new();
        return new();
    }

    public void Write()
    {
        File.WriteAllText(PATH, JsonConvert.SerializeObject(this, Formatting.Indented));
    }
}
