using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestHelper.LocalDB.Model;
using QuestHelper.Model;
using Realms;
using Xamarin.Essentials;

namespace QuestHelper.Managers
{
    public class UserManager : RealmInstanceMaker
    {
        public UserManager()
        {
        }

        internal User GetById(string userId)
        {
            return RealmInstance.Find<User>(userId);
        }

        internal bool Save(ViewUserInfo viewUserInfo)
        {
            bool result = false;
            try
            {
                RealmInstance.Write(() =>
                {
                    var dbObject = !string.IsNullOrEmpty(viewUserInfo.UserId) ? RealmInstance.Find<User>(viewUserInfo.UserId) : null;
                    if (dbObject == null)
                    {
                        dbObject = new User();
                        dbObject.UserId = viewUserInfo.UserId;
                        RealmInstance.Add(dbObject);
                    }
                    dbObject.Name = viewUserInfo.Name;
                    dbObject.Email = viewUserInfo.Email;
                    dbObject.ImgUrl = viewUserInfo.ImgUrl;
                });

                result = true;
            }
            catch (Exception e)
            {
                HandleError.Process("UserManager", "Save", e, false);
            }

            return result;
        }
    }
}
