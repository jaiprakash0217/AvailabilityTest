using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KPIReporting.AvailabilityTest
{
    /// <summary>
    /// Azure Function that performs an availability test on a test app
    /// and reports result to Application Insights
    /// The function will run every 15 minutes
    /// </summary>
    public class AvailabilityTest
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly HttpClient _httpClient;
        private readonly string _testAppUrl;
        private readonly string _testJob1Url;
        private readonly string _testJob2Url;
        private readonly string _testJob3Url;
        private readonly string _testJob4Url;
        private readonly string _testJob5Url;
        private readonly string _testJob6Url;

        public AvailabilityTest(IHttpClientFactory httpClientFactory, TelemetryClient telemetryClient, IConfiguration configuration)
        {
            _telemetryClient = telemetryClient;
            _httpClient = httpClientFactory.CreateClient();
            _testAppUrl = configuration["TestAppUrl"];
            _testJob1Url = configuration["TestJob1Url"];
            _testJob2Url = configuration["TestJob2Url"];
            _testJob3Url = configuration["TestJob3Url"];
            _testJob4Url = configuration["TestJob4Url"];
            _testJob5Url = configuration["TestJob5Url"];
            _testJob6Url = configuration["TestJob6Url"];
        }

        [FunctionName("KPIReporting-AvailabilityTest")]
        public async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer, ILogger log)
        {

            log.LogInformation($"Availability test executed at: {DateTime.Now}");

            try
            {
                // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response =  _httpClient.GetAsync(_testAppUrl);
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                response.EnsureSuccessStatusCode();
                log.LogInformation($"Successful response! Response code for {_testAppUrl}: {response.StatusCode} ");

                // Repeat this task for all web jobs
                // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response1 =  _httpClient.GetAsync(_testJob1Url);
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                response1.EnsureSuccessStatusCode();
                log.LogInformation($"Successful response! Response code for {_testJob1Url}: {response1.StatusCode} ");

                 // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response2 =  _httpClient.GetAsync(_testJob2Url);                
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                response2.EnsureSuccessStatusCode();
                log.LogInformation($"Successful response! Response code for {_testJob2Url}: {response2.StatusCode} ");

                 // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response3 =  _httpClient.GetAsync(_testJob3Url);                
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                response3.EnsureSuccessStatusCode();
                log.LogInformation($"Successful response! Response code for {_testJob3Url}: {response3.StatusCode} ");

                 // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response4 =  _httpClient.GetAsync(_testJob4Url);                
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                response4.EnsureSuccessStatusCode();
                log.LogInformation($"Successful response! Response code for {_testJob4Url}: {response4.StatusCode} ");

                 // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response5 =  _httpClient.GetAsync(_testJob5Url);
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                response5.EnsureSuccessStatusCode();
                log.LogInformation($"Successful response! Response code for {_testJob5Url}: {response5.StatusCode} ");

                 // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response6 =  _httpClient.GetAsync(_testJob6Url);                
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                response6.EnsureSuccessStatusCode();
                log.LogInformation($"Successful response! Response code for {_testJob6Url}: {response6.StatusCode} ");

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
                Name = nameof(AvailabilityTest),
                Success = isSuccess
            });
        }
    }
}
