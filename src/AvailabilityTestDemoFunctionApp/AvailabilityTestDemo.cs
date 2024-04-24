using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Contoso.AvailabilityTest
{
    /// <summary>
    /// Azure Function that performs an availability test on a test app
    /// and reports result to Application Insights
    /// The function will run every 5 minutes
    /// </summary>
    public class AvailabilityTestDemo
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly HttpClient _httpClient;
        private readonly string _testAppUrl;

        public AvailabilityTestDemo(IHttpClientFactory httpClientFactory, TelemetryClient telemetryClient, IConfiguration configuration)
        {
            _telemetryClient = telemetryClient;
            _httpClient = httpClientFactory.CreateClient();
            _testAppUrl = configuration["TestAppUrl"];
        }

        [FunctionName("AvailabilityTestDemo")]
        public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {

            log.LogInformation($"Availability test executed at: {DateTime.Now}");

            try
            {
                // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response = await _httpClient.GetAsync(_testAppUrl);
                
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                response.EnsureSuccessStatusCode();

                log.LogInformation($"Successful response! Response code: {response.StatusCode} ");

                // Signal to App Insights that everything is ok
                this.TrackAvailability(true);
            }
            catch (HttpRequestException e)
            {
                // Handle failed requests (signal to app insights)
                log.LogInformation($"Availability test failed! Reason: {e.Message} ");
                this.TrackAvailability(false);
            }
        }

       /// <summary>
       /// Helper method to track availability
       /// </summary>
        private void TrackAvailability(bool isSuccess)
        {
            _telemetryClient.TrackAvailability(new AvailabilityTelemetry()
            {
                Name = nameof(AvailabilityTestDemo),
                Success = isSuccess
            });
        }
    }
}
