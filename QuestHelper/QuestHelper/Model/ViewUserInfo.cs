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
    public class ViewUserInfo
    {
        private string _userId = string.Empty;
        private string _name = string.Empty;
        private string _email = string.Empty;

        public ViewUserInfo()
        {
        }

        public string UserId
        {
            set { _userId = value; }
            get
            {
                return _userId;
            }
        }
        public string Name
        {
            set { _name = value; }
            get
            {
                return _name;
            }
        }
        public string Email
        {
            set { _email = value; }
            get
            {
                return _email;
            }
        }      
    }
}
