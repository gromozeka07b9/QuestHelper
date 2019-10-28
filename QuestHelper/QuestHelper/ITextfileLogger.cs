namespace QuestHelper
{
    public interface ITextfileLogger
    {
        void AddStringEvent(string textEvent);
        void SaveReport();
        void NewFile();

    }
}