using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

namespace SampleProject.Webhooks
{
    public class WebhookSender
    {
        private readonly HttpClient _httpClient;
        private readonly IReportingService _reportingService;

        public WebhookSender(HttpClient httpClient, IReportingService reportingService)
        {
            _httpClient = httpClient;
        }

        public async Task SendWebhook(WebhookConfiguration configuration, bool webhookIsActive)
        {
            var d = new WebhookDataStore("https://webhooks.checkout.com/datastore");
            var additionalWebhookInfo = d.GetAdditionalInfo(configuration.WebhookId);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, configuration.Url);
            var webhookResponse = _httpClient.Send(httpRequest);

            if (webhookResponse.StatusCode == HttpStatusCode.OK)
            {
                await d.UpdateResult("Success");
                configuration.Success = true;
            }
            else if (webhookResponse.StatusCode == HttpStatusCode.NotFound)
            {
                await d.UpdateResult("Failed");
            }
            else if (webhookResponse.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                await d.UpdateResult("Failed");
            }

            try
            {
                await _reportingService.UpdateReportingDatabase(configuration);
            }
            catch (Exception ex)
            {

            }
        }
    }