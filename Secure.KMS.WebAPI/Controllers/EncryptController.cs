using EQS.KMS.Application.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EQS.KMS.Application.Interfaces;
using EQS.KMS.WebAPI.Filters;

namespace EQS.KMS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncryptController : ControllerBase
    {
        private readonly ICryptoService _cryptoService;

        public EncryptController(ICryptoService cryptoService)
        {
            _cryptoService = cryptoService;
        }
        
        [HttpPost("{customerId}/envelope")]
        // No Auth
        [ProducesResponseType(typeof(EncryptRO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> EncryptEnvelopeRsa(string customerId, EncryptIO encrypt)
        {
            var result = await _cryptoService.EncryptEnvelopeRsa(customerId, encrypt.PlainText);
            return Ok(new EncryptRO
            {
                CipherText = result
            });
        }
        
        [HttpPost("{customerId}/rsa")]
        // No Auth
        [ProducesResponseType(typeof(EncryptRO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> EncryptRsa(string customerId, EncryptIO encrypt)
        {
            var result = await _cryptoService.EncryptRsa(customerId, encrypt.PlainText);
            return Ok(new EncryptRO
            {
                CipherText = result
            });
        }
        
        [HttpPost("{customerId}/aes")]
        [CryptoTokenAuth]
        [ProducesResponseType(typeof(EncryptRO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> EncryptAes(string customerId, EncryptIO encrypt)
        {
            var result = await _cryptoService.EncryptAes(customerId, encrypt.PlainText);
            return Ok(new EncryptRO
            {
                CipherText = result
            });
        }
    }
}
