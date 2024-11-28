using EQS.KMS.Application.Encryption;
using EQS.KMS.Application.Entities;
using EQS.KMS.Application.Manager;
using EQS.KMS.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EQS.KMS.Application.Enums;
using EQS.KMS.Application.Interfaces;
using System.Net;

namespace EQS.KMS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IUserService _userService;
        public AuthController(IApplicationDbContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }

        [HttpPost("{customerId}")]
        public async Task<IActionResult> CreateFirstCustomerUserKeySet(string customerId, [FromBody] CustomerPayload payload, [FromServices] CreateCustomer createCustomer)
        {
            var response = await createCustomer.CreateFirstCustomerUserKeySet(customerId, payload);
            if (response == HttpStatusCode.Conflict)
                return Conflict();
            else
                return Ok();
        }

        [HttpPut("{customerId}")]
        public async Task<IActionResult> Update(string customerId, [FromBody] CustomerCreatePayload payload, [FromServices] CreateCustomer createCustomer)
        {
            var customer = await createCustomer.Update(customerId, payload);
            return Ok(customer);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginToken), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateSession([FromBody] LoginPayload login)
        {
            var activeUser = _dbContext.Users.Find(login.UserId);
            if (activeUser == null)
            {
                return Unauthorized();
            }
            
            var sessionToken = Guid.NewGuid().ToString();
            var authenticationKey = Guid.NewGuid().ToString();

            var masterKey = SymmetricEncryptionManager.Decrypt(activeUser.EncryptedMasterKey, HashManager.StretchPasswordToEncryptionKey(activeUser.MasterKeyHashAlgorithmType, login.Password), activeUser.MasterKeySymmetricAlgorithmType);

            var keySet = _dbContext.KeySets.LastOrDefault(a => a.CustomerId == login.CustomerId);
            if (keySet == null)
                return Unauthorized();

            var loginToken = new CryptoSession
            {
                CreationDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddDays(1),
                Token = sessionToken,
                EncryptedMasterKey = SymmetricEncryptionManager.Encrypt(masterKey, authenticationKey, SymmetricAlgorithmType.AES256),
                KeySetId = keySet.Id,
                UserId = login.UserId
            };

            _dbContext.CryptoSessions.Add(loginToken);
            _dbContext.SaveChangesAsync();

            var loginResponse = new LoginToken
            {
                AuthKey = authenticationKey,
                SessionToken = sessionToken
            };

            var json = JsonConvert.SerializeObject(loginResponse);
            var bytes = Encoding.UTF8.GetBytes(json);

            return Ok(Convert.ToBase64String(bytes));
        }

        [HttpPost("{customerId}/userpass/users/{userId}/password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public IActionResult UpdatePassword(string customerId, string userId, [FromBody] PasswordResetIO passwordResetIo)
        {
            _userService.UpdatePassword(userId, passwordResetIo);
            return Ok();
        }
        
        [HttpPost("{customerId}/userpass/users/{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public IActionResult CreateNewUser(string customerId, string userId, [FromBody] CreateUserIO createUser)
        {
            _userService.CreateNewUser(customerId, userId, createUser.Password);
            return Ok();
        }
    }
}
