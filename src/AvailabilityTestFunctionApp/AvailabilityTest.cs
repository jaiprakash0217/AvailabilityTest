using System;
using System.Text;
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
        private readonly string _userName;
        private readonly string _password;
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
            _userName = configuration["WebUser"];
            _password = configuration["Password"];
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
                var byteArray = Encoding.ASCII.GetBytes($"{_userName}:{_password}");
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                using HttpResponseMessage response = await _httpClient.GetAsync(_testAppUrl);
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                response.EnsureSuccessStatusCode();
                log.LogInformation($"Successful response! Response code for Base URL: {response.StatusCode} ");
               

                // Repeat this task for all web jobs
                // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response1 = await _httpClient.GetAsync(_testJob1Url);
                var Job1Status = await response1.Content.ReadAsStringAsync();  
                if (Job1Status.Contains("\"status\":\"Running\"") || Job1Status.Contains("\"status\":\"Completed\""))
                {
                log.LogInformation($"Successful response! Response status for {_testJob1Url}: Running");
                }
                else
                {
                    log.LogInformation($"Successful response! Response status for {_testJob1Url}: Not Running");
                    throw new ArgumentException($"Please start web job: {_testJob1Url}");
                }
                
                 // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response2 = await _httpClient.GetAsync(_testJob2Url);                
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                var Job2Status = await response2.Content.ReadAsStringAsync();
                if (Job2Status.Contains("\"status\":\"Running\"") || Job2Status.Contains("\"status\":\"Completed\""))
                {
                log.LogInformation($"Successful response! Response status for {_testJob2Url}: Running");
                }
                else
                {
                    log.LogInformation($"Successful response! Response status for {_testJob2Url}: Not Running");
                    throw new ArgumentException($"Please start web job: {_testJob2Url}");
                }

                 // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response3 = await _httpClient.GetAsync(_testJob3Url);                
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                var Job3Status = await response3.Content.ReadAsStringAsync();
                if (Job3Status.Contains("\"status\":\"Running\"") || Job3Status.Contains("\"status\":\"Completed\""))
                {
                log.LogInformation($"Successful response! Response status for {_testJob3Url}: Running");
                }
                else
                {
                    log.LogInformation($"Successful response! Response status for {_testJob3Url}: Not Running");
                    throw new ArgumentException($"Please start web job: {_testJob3Url}");
                }

                 // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response4 = await _httpClient.GetAsync(_testJob4Url);                
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                var Job4Status = await response4.Content.ReadAsStringAsync();
                if (Job4Status.Contains("\"status\":\"Running\"") || Job4Status.Contains("\"status\":\"Completed\""))
                {
                log.LogInformation($"Successful response! Response status for {_testJob4Url}: Running");
                }
                else
                {
                    log.LogInformation($"Successful response! Response status for {_testJob4Url}: Not Running");
                    throw new ArgumentException($"Please start web job: {_testJob4Url}");
                }

                 // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response5 = await _httpClient.GetAsync(_testJob5Url);
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                var Job5Status = await response5.Content.ReadAsStringAsync();
                if (Job5Status.Contains("\"status\":\"Running\"") || Job5Status.Contains("\"status\":\"Completed\""))
                {
                log.LogInformation($"Successful response! Response status for {_testJob5Url}: Running");
                }
                else
                {
                    log.LogInformation($"Successful response! Response status for {_testJob5Url}: Not Running");
                    throw new ArgumentException($"Please start web job: {_testJob5Url}");
                }

                 // Make a request to the test app that we monitor for availability
                using HttpResponseMessage response6 = await _httpClient.GetAsync(_testJob6Url);                
                // Ensure we get a successful response (typically 200 OK). Otherwise, an exception will be thrown
                var Job6Status = await response6.Content.ReadAsStringAsync();
                if (Job6Status.Contains("\"status\":\"Running\"") || Job6Status.Contains("\"status\":\"Completed\""))
                {
                log.LogInformation($"Successful response! Response status for {_testJob6Url}: Running");
                }
                else
                {
                    log.LogInformation($"Successful response! Response status for {_testJob6Url}: Not Running");
                    throw new ArgumentException($"Please start web job: {_testJob6Url}");
                }

                // Signal to App Insights that everything is ok
                this.TrackAvailability(true);
            }
            catch (HttpRequestException e)
            {
                // Handle failed requests (signal to app insights)
                log.LogInformation($"Availability test failed! Reason: {e.Message} ");
                this.TrackAvailability(false);
            }
            catch (Exception e)
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
