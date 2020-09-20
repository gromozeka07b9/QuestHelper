using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper
{
    public interface IPathService
    {
        string InternalFolder { get; }
        string PublicExternalFolder { get; }
        string PrivateExternalFolder { get; }
        string PublicDirectoryPictures { get; }
        string PublicDirectoryDcim { get; }
        string GetLastUsedDCIMPath();
    }
}
