using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
        var param = JsonConvert.DeserializeObject<MarkdownRequestArgs>(requestBody);
        if (param == null)
        {
            args.ReplyJson(new { code = 400, msg = "参数错误" }, HttpStatusCode.BadRequest);
            return;
        }
        var (buffer, _) = await Utils.Markdown(param);
        args.ReplyImage(buffer, HttpStatusCode.OK);
    }
}
