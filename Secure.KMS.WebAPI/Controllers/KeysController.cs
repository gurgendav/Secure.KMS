using EQS.KMS.Application.Interfaces;
using EQS.KMS.Application.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EQS.KMS.WebAPI.Filters;

namespace EQS.KMS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CryptoTokenAuth]
    public class KeysController : ControllerBase
    {
        private readonly IKeyService _keyService;
        private readonly ICryptoService _cryptoService;

        public KeysController(IKeyService keyService, ICryptoService cryptoService)
        {
            _keyService = keyService;
            _cryptoService = cryptoService;
        }

        [HttpPost("{customerId}/rotate")]
        public async Task<IActionResult> Rotate(string customerId, [FromBody] KeyPayload payload)
        {
            await _keyService.RotateKeySet(customerId, payload);
            return Ok();
        }
        
        [HttpPost("{customerId}/rewrap")]
        public async Task<IActionResult> Rotate(string customerId, [FromBody] RewrapPayload payload)
        {
            var result = await _cryptoService.Rewrap(customerId, payload.Ciphertext);
            return Ok(new EncryptRO
            {
                CipherText = result
            });
        }
    }
}
