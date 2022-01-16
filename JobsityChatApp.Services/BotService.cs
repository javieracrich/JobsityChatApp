using JobsityChatApp.Core;
using Microsoft.Extensions.Options;
using RestSharp;

namespace JobsityChatApp.Services;

public interface IBotService
{
    public Task RequestQuote(string stockCode);
}

public class BotService : IBotService
{
    private readonly BotOptions botOptions;

    public BotService(IOptions<BotOptions> botOptions)
    {
        this.botOptions = botOptions.Value;
    }

    public async Task RequestQuote(string stockCode)
    {
        var client = new RestClient(this.botOptions.Url!);
        var request = new RestRequest($"stock?code={stockCode}", Method.Get);
        await client.ExecuteAsync(request);
    }
}
