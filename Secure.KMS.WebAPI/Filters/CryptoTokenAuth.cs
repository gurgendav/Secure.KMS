using System;
using EQS.KMS.WebAPI.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EQS.KMS.WebAPI.Filters
{
    public class CryptoTokenAuth : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(HeaderNames.CryptoToken, out var token) ||
                string.IsNullOrEmpty(token))
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}