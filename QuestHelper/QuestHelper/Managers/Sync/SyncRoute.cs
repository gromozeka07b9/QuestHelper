using System;
namespace QuestHelper.Managers.Sync
{
    public class SyncRoute
    {
        private string _authToken = string.Empty;
        private readonly string _routeId = string.Empty;

        public SyncRoute(string routeId)
        {
            _routeId = routeId;
        }

        public bool Sync()
        {
            return true;
        }

        public void SetAuthToken(string authToken)
        {
            _authToken = authToken;
        }
    }
}
