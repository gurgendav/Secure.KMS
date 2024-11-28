using EQS.KMS.Application.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EQS.KMS.Application.Interfaces;
using EQS.KMS.WebAPI.Filters;

namespace EQS.KMS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CryptoTokenAuth]
    public class DecryptController : ControllerBase
    {
        private readonly ICryptoService _cryptoService;

        public DecryptController(ICryptoService cryptoService)
        {
            _cryptoService = cryptoService;
        }
        
        [HttpPost("{customerId}")]
        [ProducesResponseType(typeof(DecryptRO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Decrypt(string customerId, DecryptIO decrypt)
        {
            var result = await _cryptoService.Decrypt(customerId, decrypt.CipherText);
            return Ok(new DecryptRO { PlainText = result });
        }
    }
}
