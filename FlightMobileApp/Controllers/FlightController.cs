using System;
using FlightMobileApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightMobileApp.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class FlightController : Controller
    {
        private ISimulatorConnector _simulatorConnector;

        public FlightController(ISimulatorConnector simulatorConnector)
        {
            this._simulatorConnector = simulatorConnector;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Oops!! This route only supports HTTP POST.");
        }

        [HttpPost]
        public IActionResult SendCommand([FromBody] FlightCommand flightCommand)
        {
            if (_simulatorConnector.SendCommand(flightCommand))
                return Ok();

            return BadRequest("Could not add server.");
        }

    }
}
