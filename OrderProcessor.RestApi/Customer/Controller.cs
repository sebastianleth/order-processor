using Microsoft.AspNetCore.Mvc;

namespace OrderProcessor.Customer
{
    [ApiController]
    [Route("[controller]")]
    public class Controller : ControllerBase
    {
        private readonly ILogger<Controller> _logger;

        public Controller(ILogger<Controller> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "GetWeatherForecast")]
        public IEnumerable<int> Get()
        {
            return null;
        }
    }
}