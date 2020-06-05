using System;
using FlightMobileApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightMobileApp.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class CommandController : Controller
    {
        private ISimulatorConnector _simulatorConnector;

        public CommandController(ISimulatorConnector simulatorConnector)
        {
            this._simulatorConnector = simulatorConnector;
        }

        [HttpPost]
        public IActionResult AddServer([FromBody] FlightCommand flightCommand)
        {
            if (_simulatorConnector.sendCommands(FlightCommand flightCommand))
                return Ok();
            return BadRequest("Could not add server.");
        }

    }
}
