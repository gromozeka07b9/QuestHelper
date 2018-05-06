using System;
namespace QuestHelper.Model
{
    public class MainPageMenuItem
    {
        public MainPageMenuItem()
        {
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public string IconName { get; set; }
        public Type TargetType { get; set; }
    }
}
