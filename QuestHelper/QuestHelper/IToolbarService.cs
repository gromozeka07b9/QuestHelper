namespace QuestHelper
{
    public interface IToolbarServiceDEL
    {
        bool ToolbarIsHidden();
        void SetVisibilityToolbar(bool Visibility);
        void SetDarkMode(bool DarkMode);
    }
}