using System.Net;
using MarkdownImageAPI.Interface;
using Newtonsoft.Json;

namespace MarkdownImageAPI.Handlers;

public class MarkdownHandler : IHttpRequestHandler
{
    public string Path => "/markdown";

    public string Method => HttpMethod.Post.Method;

    public async Task InvokeAsync(HttpRequestArgs args, CancellationToken cancellationToken)
    {
        using var sr = new StreamReader(args.Context.Request.InputStream, args.Context.Request.ContentEncoding);
        var requestBody = await sr.ReadToEndAsync(cancellationToken);
        var param = JsonConvert.DeserializeObject<MarkdownRequestArgs>(requestBody) ?? throw new ArgumentNullException("请求参数不能为空");
        var (buffer, _) = await Utils.Markdown(param);
        args.ReplyImage(buffer, HttpStatusCode.OK);
    }
}
