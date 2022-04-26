using System.ComponentModel;
using System.Text.Json;
using Lighter.Components.Common;

namespace Lighter.Components
{
    public class OnboardingCarouselDeserializer
    {
        public OnboardingCarousel Deserialize(string jsonData)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<OnboardingCarousel>(jsonData, options);
        } 
    }
}