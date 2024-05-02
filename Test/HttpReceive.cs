using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace MarkdownImageAPI;

public class HttpReceive
{
    private static readonly HttpListener HttpListener = new();

    private static readonly LogWriter Log = new();

    public static void Start()
    {
        HttpListener.Prefixes.Add($"http://*:{Program.Config?.Prot}/");
        HttpListener.Start();
        HttpListener.BeginGetContext(OnContext, null);
    }

    private static async void OnContext(IAsyncResult ar)
    {
        HttpListener.BeginGetContext(OnContext, null);
        var data = HttpListener.EndGetContext(ar);
        if (data.Request.HttpMethod == HttpMethod.Post.Method)
        {
            using var reader = new StreamReader(data.Request.InputStream);
            var body = reader.ReadToEnd();
            var text = body
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("\r\n", "");
            Log.ConsoleInfo($"[{data.Request.UserHostAddress}]: {text}", ConsoleColor.Green);

            try
            {
                var revc = JsonConvert.DeserializeObject<ReceiveArgs>(body);
                if (revc == null)
                {
                    Response(data.Response, new
                    {
                        code = 1,
                        message = "无法解析参数!",
                    });
                    return;
                }
                var (bytes, time) = await Utils.Markdown(revc);
                File.WriteAllBytes("out.png", bytes);
                var base64 = Convert.ToBase64String(bytes);
                Response(data.Response, new
                {
                    code = 0,
                    message = "转换成功",
                    timer = time.TotalSeconds,
                    data = base64
                });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.ToString());
                Response(data.Response, new
                {
                    code = 1,
                    message = ex.Message,
                });
            }
        }
        else
        {
            var ret = new
            {
                code = 1,
                message = "仅支持Post请求!",
            };
            Response(data.Response, ret);
        }
    }

    private static void Response(HttpListenerResponse resp, object data)
    {
        resp.ContentEncoding = Encoding.UTF8;
        resp.StatusCode = 200;
        resp.OutputStream.Write(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, Formatting.Indented)));
        resp.Close();
    }
}
