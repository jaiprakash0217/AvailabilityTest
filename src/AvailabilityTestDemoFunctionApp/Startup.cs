using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Contoso.AvailabilityTest.Startup))]


namespace Contoso.AvailabilityTest
{
    /// <summary>
    /// Startup class to set up dependencies for the Azure Function
    /// </summary>
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

        }
    }
}
