using System.IO;
using System.Linq;
using System.Text;
using Lighter.Components;
using NUnit.Framework;

namespace Lighter.Tests
{
    public class ComponentDeserializer
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("onboarding.json")]
        public void MustDeserializeOnboardingJson(string fileForTest)
        {
            var jsonFile = GetResourceTextFile(fileForTest);
            OnboardingCarouselDeserializer deserializer = new OnboardingCarouselDeserializer();
            var onboardingCarousel = deserializer.Deserialize(jsonFile);
            Assert.That(onboardingCarousel, Is.Not.Null);
        }
        
        public string GetResourceTextFile(string filename)
        {
            var resourceName = GetType().Module.Assembly.GetManifestResourceNames().Where(x => x.EndsWith(filename)).FirstOrDefault() ?? string.Empty;
            var stream = GetType().Module.Assembly.GetManifestResourceStream(resourceName);
            if(stream == null)
                return string.Empty;
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

    }    
}
