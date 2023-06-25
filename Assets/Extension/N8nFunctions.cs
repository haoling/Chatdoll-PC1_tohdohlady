using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using ChatdollKit.Network;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public class N8nFunctions : MonoBehaviour
{
    public string n8nUrl;
    private ChatdollHttp client = new ChatdollHttp(timeout: 20000);

    public ChatGPTStreamSkill.ChatGPTFunction CreateSearchGoogleFunction()
    {
        var googleSearchFunction = new ChatGPTStreamSkill.ChatGPTFunction("search", "知識にないことを調べます", this.SearchGoogleAsync);
        googleSearchFunction.AddProperty("query", new Dictionary<string, object>()
        {
            { "type", "string" }
        });
        return googleSearchFunction;
    }

    public async UniTask<string> SearchGoogleAsync(string jsonString, CancellationToken token)
    {
        // google検索する
        var funcArgs = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
        var searchWord = funcArgs["query"];
        var searchResponse = await client.GetJsonAsync<SearchResponses>($"{n8nUrl}?function=search&q={searchWord}", cancellationToken: token);
        Debug.Log($"Search result: {JsonConvert.SerializeObject(searchResponse, Formatting.Indented)}");

        return $"以下は検索結果です。あなたの言葉でユーザーに伝えてください。\n\n```json\n{JsonConvert.SerializeObject(searchResponse.results, Formatting.Indented)}\n```";
    }

    private class SearchResponses
    {
        public SearchResponse[] results;
    }

    private class SearchResponse
    {
        public string title;
        public string snippet;
    }
}
