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
    public class ViewUserInfo : ISaveable
    {
        private string _userId = string.Empty;
        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _imgUrl;
        private UserManager _manager = new UserManager();

        public ViewUserInfo()
        {
        }

        public void Load(string userId)
        {
            var userObject = _manager.GetById(userId);
            if (userObject != null)
            {
                _userId = userObject.UserId;
                _name = userObject.Name;
                _email = userObject.Email;
                _imgUrl = userObject.ImgUrl;
            }
        }

        internal void FillFromWSModel(SharedModelsWS.UserAccount wsUser)
        {
            if (wsUser != null)
            {
                _userId = wsUser.Id;
                _name = wsUser.Name;
                _email = wsUser.Email;
                _imgUrl = wsUser.ImgUrl;
            }
        }

        public string Id
        {
            set { _userId = value; }
            get
            {
                return _userId;
            }
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
        public string ImgUrl
        {
            set { _imgUrl = value; }
            get
            {
                return _imgUrl;
            }
        }

        public bool Save()
        {
            return _manager.Save(this);
        }
    }
}
