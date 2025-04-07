using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownImageAPI.Interface;

public interface IHttpRequestHandler
{
    public string Path { get; }
    public string Method { get;}

    public Task InvokeAsync(HttpRequestArgs args, CancellationToken cancellationToken);

}
