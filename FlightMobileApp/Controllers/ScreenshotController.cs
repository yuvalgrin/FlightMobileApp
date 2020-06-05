using System;
using FlightMobileApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightMobileApp.Controllers
{
    [Route("/")]
    [ApiController]
    public class ScreenshotController : Controller
    {
        private ISimulatorConnector _simulatorConnector;

        public ScreenshotController(ISimulatorConnector simulatorConnector)
        {
            this._simulatorConnector = simulatorConnector;
        }

        [HttpPost]
        public IActionResult AddServer([FromBody] FlightCommand flightCommand)
        {
            if (_simulatorConnector.sendCommand(flightCommand))
                return Ok();
            return BadRequest("Could not add server.");
        }

    }
}
