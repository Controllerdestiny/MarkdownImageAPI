using System.Reflection;
using MarkdownImageAPI;

Utils.Kill();

Console.Title = "MarkdownImageAPI";
if (!File.Exists("appsettings.json"))
{
    var assembly = Assembly.GetExecutingAssembly();
    using var stream = assembly.GetManifestResourceStream("MarkdownImageAPI.Resources.appsettings.json");
    if (stream == null)
    {
        Console.WriteLine("appsettings.json not found in resources.");
        return;
    }
    using var fs = new FileStream("appsettings.json", FileMode.Create, FileAccess.Write);
    stream?.CopyTo(fs);
    fs.Close();
    stream?.Close();
}
MarkdownApp.Start();