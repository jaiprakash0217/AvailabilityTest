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
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
        {
            
            log.LogInformation($"Availability test executed at: {DateTime.Now}");

            try
            {
                // Make a request to the test app that we monitor for availability
                 _httpClient.BaseAddress = new Uri("https://usawu2gdpcntrl-dev-wap.scm.usawu2gdpcntrl-dev-ase.appserviceenvironment.net");
                _httpClient.DefaultRequestHeaders.Add($"Authorization", $"Basic {Base64Encode("usawu2gdpcntrl-dev-wap:0DM4aSvwWWM9mJ28GMqGrcFQtaKPB7b135Zq4vg4pJFGjT61ntGfDT25TXSX")}");
                using HttpResponseMessage response = await _httpClient.GetAsync("/api/continuouswebjobs/MetallurgyReportWebJob");
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                response.EnsureSuccessStatusCode();
                log.LogInformation($"Successful response! Response code for base URL: {response} ");
               

                // Repeat this task for all web jobs
                // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response1 = await _httpClient.GetAsync("/api/continuouswebjobs/MetallurgyReportWebJob1");
                
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                response1.EnsureSuccessStatusCode();
                log.LogInformation($"Successful response! Response code for MetallurgyReportWebJob1: {response1} ");
                
              /*  
                 // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response2 = await _httpClient.GetAsync(_testJob2Url);                
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                response2.EnsureSuccessStatusCode();
                log.LogInformation($"Successful response! Response code for {_testJob2Url}: {response2.StatusCode} ");

                 // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response3 = await _httpClient.GetAsync(_testJob3Url);                
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                response3.EnsureSuccessStatusCode();
                log.LogInformation($"Successful response! Response code for {_testJob3Url}: {response3.StatusCode} ");

                 // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response4 = await _httpClient.GetAsync(_testJob4Url);                
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                response4.EnsureSuccessStatusCode();
                log.LogInformation($"Successful response! Response code for {_testJob4Url}: {response4.StatusCode} ");

                 // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response5 = await _httpClient.GetAsync(_testJob5Url);
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                response5.EnsureSuccessStatusCode();
                log.LogInformation($"Successful response! Response code for {_testJob5Url}: {response5.StatusCode} ");

                 // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response6 = await _httpClient.GetAsync(_testJob6Url);                
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                response6.EnsureSuccessStatusCode();
                log.LogInformation($"Successful response! Response code for {_testJob6Url}: {response6.StatusCode} ");
*/
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
