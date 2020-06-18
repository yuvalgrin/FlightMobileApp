using System;
using FlightMobileApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightMobileApp.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class ScreenshotController : Controller
    {
        private ISimulatorConnector _simulatorConnector;

        public ScreenshotController(ISimulatorConnector simulatorConnector)
        {
            this._simulatorConnector = simulatorConnector;
        }

        [HttpGet]
        public IActionResult GetScreen()
        {
            Byte[] bytes = _simulatorConnector.GetScreenshot();
            if (bytes != null && bytes.Length > 0)
                return File(bytes, "image/jpeg");

            return BadRequest(new Response("Could not get screenshot."));
        }

    }
}
