using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestHelper.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(PathService))]
namespace QuestHelper.iOS
{
    public class PathService : IPathService
    {
        public string InternalFolder
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }

        public string PublicExternalFolder
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }

        public string PrivateExternalFolder
        {
            get
            {
                string absPath = string.Empty;
                try
                {
                    absPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                catch (Exception)
                {
                }
                return absPath;
            }
        }
    }
}