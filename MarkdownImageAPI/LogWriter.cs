using System.Diagnostics;
namespace MarkdownImageAPI;

public class LogWriter
{
    public virtual StreamWriter StreamWriter { get; set; }

    public LogWriter()
    {
        var file = Path.Combine(Program.Config.LogPath, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".log");
        var dir = Path.GetDirectoryName(file);
        if (dir == null)
            throw new NullReferenceException("目录文件不存在!");
        Directory.CreateDirectory(dir);
        StreamWriter = new(file);
    }

    private static void OutPutConsole(string msg, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    public virtual void ConsoleError(string info)
    {
        OutPutConsole(info, ConsoleColor.Red);
        Writer(info, TraceLevel.Error);
    }

    public void ConsoleInfo(string info, ConsoleColor color = ConsoleColor.Gray)
    {
        OutPutConsole(info, color);
        Writer(info, TraceLevel.Info);
    }

    public virtual void ConsoleWarn(string info)
    {
        OutPutConsole(info, ConsoleColor.Yellow);
        Writer(info, TraceLevel.Warning);
    }

    public virtual void Dispose()
    {
        StreamWriter.Dispose();
        GC.SuppressFinalize(this);
    }

    public virtual void Error(string info)
    {
        Writer(info, TraceLevel.Error);
    }

    public virtual void Info(string info)
    {
        Writer(info, TraceLevel.Info);
    }

    public virtual void Warn(string info)
    {
        Writer(info, TraceLevel.Warning);
    }

    public virtual void Writer(string info, TraceLevel level)
    {
        if (string.IsNullOrEmpty(info))
            return;
        if (StreamWriter.BaseStream.Length * 1024 * 1024 > Program.Config.LogSize)
        {
            StreamWriter.Close();
            StreamWriter.Dispose();
            StreamWriter = new(Path.Combine(Program.Config.LogPath, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".log"));
        }
        var trace = new StackTrace();
        var frame = trace.GetFrame(2);
        if (frame != null)
        {
            var prefix = frame.GetMethod()?.DeclaringType?.Name;
            StreamWriter.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{prefix}] [{level.ToString().ToUpper()}]: {info}");
            StreamWriter.Flush();
        }
    }
}
