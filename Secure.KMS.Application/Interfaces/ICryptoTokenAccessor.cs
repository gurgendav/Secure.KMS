using EQS.KMS.Application.Models;

namespace EQS.KMS.Application.Interfaces
{
    public interface ICryptoTokenAccessor
    {
        LoginToken GetToken();
    }
}