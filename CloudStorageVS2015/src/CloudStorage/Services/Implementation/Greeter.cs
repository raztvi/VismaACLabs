using Microsoft.Extensions.Configuration;

namespace CloudStorage.Services.Implementation
{
    public class Greeter : IGreeter
    {
        private readonly string _greeting;

        public Greeter(IConfiguration configuration)
        {
            _greeting = configuration["GreetingMessage"];
        }

        public string GetGreeting()
        {
            return _greeting;
        }
    }
}