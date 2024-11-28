using EQS.KMS.Application.Interfaces;
using EQS.KMS.Application.Manager;
using EQS.KMS.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EQS.KMS.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ICryptoService, CryptoService>();
            serviceCollection.AddTransient<IKeyService, KeyService>();
            serviceCollection.AddTransient<CreateCustomer>();
        }
    }
}