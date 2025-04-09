using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vendsys.Interfaces;
using Vendsys.Models;

namespace Vendsys.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VendsysController : ControllerBase
    {
        private readonly ILogger<VendsysController> _logger;
        private readonly IVendsysManager _vendsysManager;
        private ResponseCommon _response;
        public VendsysController(ILogger<VendsysController> logger, IVendsysManager vendsysManager)
        {
            _logger = logger;
            _vendsysManager = vendsysManager;
        }

        [Authorize(Roles ="Admin")]
        [HttpPost("/vdi-dex", Name = "vdi-dex")]
        public async Task<IActionResult> dexProcess([FromBody] string dex)
        {
            try
            {
                switch (dex)
                {
                    case "delete":
                        _response = await _vendsysManager.ManageDeleteRows();
                        break;
                    default:
                        _response = await _vendsysManager.ManageDexFile(Request.Headers["machine"], Encoding.UTF8.GetString(Convert.FromBase64String(dex)));
                        break;
                }

                return Ok(_response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
