using System.Threading.Tasks;
using EQS.KMS.Application.Models;

namespace EQS.KMS.Application.Interfaces
{
    public interface IKeyService
    {
        Task RotateKeySet(string customerId, KeyPayload payload);
    }
}