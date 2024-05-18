
using MarkdownImageAPI;
using PuppeteerSharp;
using System.Diagnostics;
using System.Timers;

class Program
{
    public static Config Config { get; private set; } = new();

    static async Task Main()
    {
        Utils.Kill();
        LoadConfig();
        ParseCommandArgs();
        var md = "# 标题 \n ##标题2";
        await new BrowserFetcher().DownloadAsync();
        await Utils.Markdown(new()
        {
            MarkdownContent = md
        });
        //HttpReceive.Start();
        //Console.ReadLine();
    }

    

    private static void ParseCommandArgs()
    {
        var cmdArgs = Utils.ParseArguements(Environment.GetCommandLineArgs());
        foreach (var (key, value) in cmdArgs)
        {
            switch (key)
            {
                case "-port":
                    if (int.TryParse(value, out var port))
                    {
                        Config.Prot = port;
                    }
                    break;
                case "-chrome":
                    Config.ChromePath = value;
                    break;
            }
        }
        Config.Write();
    }

    private static void LoadConfig()
    {
        if (File.Exists(Config.PATH))
        {
            Config = Config.Read();
        }
        else
        {
            Config = new();
            Config.Write();
        }
    }
}
