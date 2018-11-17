namespace QuestHelper.Server.Models
{
    public interface IVersionedObject
    {
        string GetObjectId();
        int GetVersion();
    }
}