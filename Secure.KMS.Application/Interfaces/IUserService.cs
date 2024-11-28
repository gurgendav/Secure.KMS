using System.Threading.Tasks;
using EQS.KMS.Application.Models;

namespace EQS.KMS.Application.Interfaces
{
    public interface IUserService
    {
        void UpdatePassword(string userId, PasswordResetIO passwordReset);
        Task CreateNewUser(string customerId, string userId, string password);
    }
}