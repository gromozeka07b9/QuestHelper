using System.Text.Json;

namespace Lighter.Components.OnboardingCarousel
{
    public class OnboardingCarouselDeserializer
    {
        public Models.OnboardingCarousel Deserialize(string jsonData)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<Models.OnboardingCarousel>(jsonData, options);
        } 
    }
}