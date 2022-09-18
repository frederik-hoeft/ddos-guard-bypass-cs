// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Text.RegularExpressions;

DDOSGuard guard = new();
guard.Get(new Uri("https://anidex.info/"));

class DDOSGuard
{
    private readonly HttpClient _client;
    private readonly CookieContainer _cookies;
    public DDOSGuard()
	{
        _cookies = new CookieContainer();
        _client = new HttpClient(new SocketsHttpHandler() 
        { 
            AllowAutoRedirect = true,
            UseCookies = true,
            CookieContainer = _cookies,
        });
    }

    public HttpResponseMessage Get(Uri uri)
    {
        using HttpRequestMessage getCheck = new(HttpMethod.Get, "https://check.ddos-guard.net/check.js");
        using HttpResponseMessage checkResponse = _client.Send(getCheck);
        string check = checkResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        Match src = Regex.Match(check, @"new Image\(\).src = '(.+?)';");
        Capture parseCheck = src.Groups[1];
        using HttpRequestMessage getValidation = new(HttpMethod.Get, $"{uri.Scheme}://{uri.DnsSafeHost}{parseCheck}");
        using HttpResponseMessage validationResponse = _client.Send(getValidation);
        using HttpRequestMessage getUrl = new(HttpMethod.Get, uri);
        getUrl.Headers.Referrer = uri;
        return _client.Send(getUrl);
    }
}