namespace QuestHelper
{
    public interface INotificationService
    {
        void Show(string textMessage);
        void ShowProgress(int progressMax, int progressCurrent);
        void HideProgress();
    }
}