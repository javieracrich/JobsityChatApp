using Refit;

namespace JobsityChatApp.Services;

public interface IBotApi
{
    [Get("/stock?code={code}&roomId={roomId}")]
    Task RequestQuote(string code, int roomId);
}
