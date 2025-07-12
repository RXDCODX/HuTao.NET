using System.Text.Json.Nodes;

namespace HuTao.NET.Models;

public abstract class BaseFetchDynamicApi(ClientData clientData, ICookie cookie)
{
    protected ClientData clientData = clientData;
    protected ICookie cookie = cookie;

    protected async Task<JsonNode> FetchDynamicApi(string url, bool isPost = false)
    {
        HttpResponseMessage res;
        if (!isPost)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);

            req.Headers.Add("x-rpc-app_version", "1.5.0");
            req.Headers.Add("x-rpc-client_type", "5");
            req.Headers.Add("x-rpc-language", clientData.Language);
            req.Headers.Add("user-agent", clientData.UserAgent);
            req.Headers.Add("Cookie", cookie.GetCookie());

            res = await clientData.HttpClient.SendAsync(req);
        }
        else
        {
            HttpContent req = new StringContent("");

            req.Headers.Add("x-rpc-app_version", "1.5.0");
            req.Headers.Add("x-rpc-client_type", "5");
            req.Headers.Add("x-rpc-language", clientData.Language);
            clientData.HttpClient.DefaultRequestHeaders.Add("User-Agent", clientData.UserAgent);
            req.Headers.Add("Cookie", cookie.GetCookie());

            res = await clientData.HttpClient.PostAsync(url, req);
        }

        var jsonString = await res.Content.ReadAsStringAsync();

        return JsonNode.Parse(jsonString) ?? throw new NullReferenceException();
    }
}
