using Lighter.Components.Common;

namespace Lighter.Components
{
    public class OnboardingCarousel : Root
    {
        public ApplicationDescription Application { get; set; }
        public WelcomeScreen WelcomeScreen { get; set; }
    }

    public class WelcomeScreen
    {
        public Text FirstActionKey { get; set; }
        public Text Headline { get; set; }
        public Text Subhead { get; set; }
        public Text StartCaption { get; set; }
        public Image Image { get; set; }
    }
}