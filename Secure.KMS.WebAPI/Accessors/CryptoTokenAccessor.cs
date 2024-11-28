using System;
using System.Text;
using EQS.KMS.Application.Interfaces;
using EQS.KMS.Application.Models;
using EQS.KMS.WebAPI.Constants;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace EQS.KMS.WebAPI.Accessors
{
    public class CryptoTokenAccessor : ICryptoTokenAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CryptoTokenAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public LoginToken GetToken()
        {
            if (!_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(HeaderNames.CryptoToken, out var token))
            {
                return null;
            }
            
            var base64EncodedBytes = Convert.FromBase64String(token);
            var json = Encoding.UTF8.GetString(base64EncodedBytes);
            return JsonConvert.DeserializeObject<LoginToken>(json);
        }
    }
}