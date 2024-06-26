using Markdig;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Diagnostics;
namespace MarkdownImageAPI;

public class Utils
{
    private static IBrowser? browser = null;

    private static IPage? Page = null;

    private static Dictionary<string, string> ReplaceDic = new()
    {
        { "\n", "\\n" },
        { "\r\n", "\\n" },
        { "\r", "\\n" },
        { "'", "\\'" }
    };

    public static Dictionary<string, string> ParseArguements(string[] args)
    {
        string text = null;
        string text2 = "";
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Length == 0)
            {
                continue;
            }
            if (args[i][0] == '-' || args[i][0] == '+')
            {
                if (text != null)
                {
                    dictionary.Add(text.ToLower(), text2);
                }
                text = args[i];
                text2 = "";
            }
            else
            {
                if (text2 != "")
                {
                    text2 += " ";
                }
                text2 += args[i];
            }
        }
        if (text != null)
        {
            dictionary.Add(text.ToLower(), text2);
        }
        return dictionary;
    }

    public static void Kill()
    {
        foreach (var process in Process.GetProcesses())
        {
            if (process.ProcessName.Contains("chrome"))
            {
                process.Kill();
            }
        }
    }

    public static async Task<(byte[], TimeSpan)> Markdown(ReceiveArgs args)
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();
        if (browser == null || !browser.IsConnected || browser.IsClosed || browser.Process.HasExited)
        {
            var option = new LaunchOptions()
            {
                Headless = Program.Config.EnableHeadLess,
                Args = [.. Program.Config.ChromeCommandArgs]
            };
            if (!string.IsNullOrEmpty(Program.Config.ChromePath))
            {
                option.ExecutablePath = Program.Config.ChromePath;
            }
            browser = await Puppeteer.LaunchAsync(option);
        }
        if (Page == null || Page.IsClosed || Page.Browser.Process.HasExited)
        {
            Page = await browser.NewPageAsync();
        }
        var opt = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseAlertBlocks()
            .UsePipeTables()
            .UseEmphasisExtras()
            .UseListExtras()
            .UseSoftlineBreakAsHardlineBreak()
            .UseFootnotes()
            .UseFooters()
            .UseCitations()
            .UseGenericAttributes()
            .UseGridTables()
            .UseAbbreviations()
            .UseEmojiAndSmiley()
            .UseDefinitionLists()
            .UseCustomContainers()
            .UseFigures()
            .UseMathematics()
            .UseBootstrap()
            .UseMediaLinks()
            .UseSmartyPants()
            .UseAutoIdentifiers()
            .UseTaskLists()
            .UseDiagrams()
            .UseYamlFrontMatter()
            .UseNonAsciiNoEscape()
            .UseAutoLinks()
            .UseGlobalization()
            .Build();
        var postData = Markdig.Markdown.ToHtml(args.MarkdownContent, opt);
        await Page.WaitForNetworkIdleAsync(new()
        {
            Timeout = args.TimeOut
        });
        foreach (var (oldChar, newChar) in ReplaceDic)
        {
            postData = postData.Replace(oldChar, newChar);
        }
        await Page.GoToAsync($"http://docs.oiapi.net/view.php?theme={(args.Dark ? "dark" : "light")}", args.TimeOut).ConfigureAwait(false);
        await Page.EvaluateExpressionAsync($"document.querySelector('#app').innerHTML = '{postData}'");
        var app = await Page.QuerySelectorAsync("body").ConfigureAwait(false);
       await Page.EvaluateExpressionAsync("document.querySelector(\"#app\").style.backgroundColor = '#ccc'");
        await app.EvaluateFunctionAsync("element => element.style.backgroundColor = '#ccc'");
        if (args.AutoWidth)
            await app.EvaluateFunctionAsync("element => element.style.width = 'fit-content'");
        var clip = await app!.BoundingBoxAsync().ConfigureAwait(false);
        var ret = await Page.ScreenshotDataAsync(new()
        {
            Clip = new Clip
            {
                Width = clip!.Width,
                Height = clip.Height,
                X = clip.X,
                Y = clip.Y
            },
            Type = ScreenshotType.Png
        });
        stopwatch.Stop();
        return (ret, stopwatch.Elapsed);
    }
}
