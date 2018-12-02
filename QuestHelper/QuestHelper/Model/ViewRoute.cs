using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using Realms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace QuestHelper.Model
{
    public class ViewRoute : ISaveable
    {
        private string _id = string.Empty;
        private string _name = string.Empty;
        private DateTimeOffset _createDate;
        private int _version = 0;

        public ViewRoute()
        {
        }

        public void Load(string routeId)
        {
            RouteManager manager = new RouteManager();
            Route route = manager.GetRouteById(routeId);
            if (route != null)
            {
                _id = route.RouteId;
                _name = route.Name;
                _createDate = route.CreateDate;
                _version = route.Version;
            }
        }

        public void Refresh(string routeId)
        {
            if(!string.IsNullOrEmpty(routeId))
            {
                Load(routeId);
            }
        }
        public string Id
        {
            set
            {
                _id = value;
            }
            get
            {
                return _id;
            }
        }
        public string RouteId
        {
            set
            {
                _id = value;
            }
            get
            {
                return _id;
            }
        }
        public string Name
        {
            set
            {
                _name = value;
            }
            get
            {
                return _name;
            }
        }
        public string CreateDateText
        {
            get
            {
                return _createDate.ToLocalTime().ToString();
            }
        }

        public DateTimeOffset CreateDate
        {
            get
            {
                return _createDate;
            }
            set
            {
                _createDate = value;
            }
        }

        public int Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
            }
        }

        public string UserId { get; set; }

        public bool Save()
        {
            RouteManager routeManager = new RouteManager();
            return routeManager.Save(this);
        }
    }
}
