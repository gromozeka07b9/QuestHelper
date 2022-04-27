using System.Collections.Generic;
using Lighter.Components.Models.Common;

namespace Lighter.Components.Models
{
    public class OnboardingCarousel : Root
    {
        public ApplicationDescription Application { get; set; }
        public WelcomeScreen WelcomeScreen { get; set; }
        public List<OnboardingScreen> OnboardingScreens { get; set; }
    }

    public class WelcomeScreen
    {
        public Text FirstActionKey { get; set; }
        public Text Headline { get; set; }
        public Text Subhead { get; set; }
        public Text StartCaption { get; set; }
        public Image Image { get; set; }
    }
    public class OnboardingScreen
    {
        public int Order { get; set; }
        public Text Headline { get; set; }
        public Text Subhead { get; set; }
        public Image Image { get; set; }
    }

}