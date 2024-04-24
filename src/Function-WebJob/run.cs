using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Barrick.AvailabilityTest.run))]


namespace Barrick.AvailabilityTest
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
