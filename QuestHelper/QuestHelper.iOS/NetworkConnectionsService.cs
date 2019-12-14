
using QuestHelper.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(NetworkConnectionsService))]
namespace QuestHelper.iOS
{
    public class NetworkConnectionsService : INetworkConnectionsService
    {
        public bool IsRoaming()
        {
            //Не знаю способ узнать для ios - роуминг или нет
            return false;
        }

    }
}