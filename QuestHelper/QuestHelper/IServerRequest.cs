using System.Net;
using System.Threading.Tasks;

namespace QuestHelper
{
    public interface IServerRequest
    {
        HttpStatusCode GetLastStatusCode();
        Task<string> HttpRequestGet(string relativeUrl, string authToken);
        Task<bool> HttpRequestGetFile(string fileUrl, string fullNameFile, string authToken, bool urlRelative = true);
    }
}