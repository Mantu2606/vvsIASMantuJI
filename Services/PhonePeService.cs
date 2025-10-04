using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FossTech.Configurations;
using FossTech.Data;
using FossTech.Helpers;
using FossTech.Models.StudyMaterialModels;
using Microsoft.EntityFrameworkCore;

namespace FossTech.Services
{
    public class PhonePeService
    {
        private readonly HttpClient _httpClient;
        private readonly IPhonePeConfiguration _config;
        private readonly ApplicationDbContext _context;

        public PhonePeService(HttpClient httpClient, IPhonePeConfiguration config, ApplicationDbContext context)
        {
            _httpClient = httpClient;
            _config = config;
            _context = context;
        }

        public async Task<string> CreateStudyMaterialPaymentRedirectUrl(StudyMaterial studyMaterial, string orderId, string userId, double amountPaise)
        {
            var tokenUrl = "https://api.phonepe.com/apis/identity-manager/v1/oauth/token";
            var tokenRequest = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", _config.MerchantId),
                new KeyValuePair<string, string>("client_version", "1"),
                new KeyValuePair<string, string>("client_secret", _config.SaltKey),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            };

            using var tokenContent = new FormUrlEncodedContent(tokenRequest);
            var tokenResponse = await _httpClient.PostAsync(tokenUrl, tokenContent);
            var tokenResponseStr = await tokenResponse.Content.ReadAsStringAsync();
            Console.WriteLine("Token response: " + tokenResponseStr);

            if (!tokenResponse.IsSuccessStatusCode)
                throw new Exception($"Token request failed: {tokenResponse.StatusCode}, {tokenResponseStr}");

            using var tokenDoc = JsonDocument.Parse(tokenResponseStr);
            if (!tokenDoc.RootElement.TryGetProperty("access_token", out var accessTokenElement))
                throw new Exception("Token response does not contain access_token");

            var accessToken = accessTokenElement.GetString();

            var payloadObj = new
            {
                //orderId = orderId,
                //merchantId = _config.MerchantId,
                merchantOrderId = $"{orderId}",
                amount = amountPaise,
                expireAfter = 1200,
                metaInfo = new
                {
                    Name = studyMaterial.Name,
                    userId = $"{userId}",
                    date = DateTime.Now.ToString("o") 
                },
                paymentFlow = new
                {
                    type = "PG_CHECKOUT",
                    message = "Payment message used for collect requests",
                    merchantUrls = new
                    {
                        redirectUrl = "https://vedaedtech.com/dashboard/StudyMaterials/thankyou"
                    }
                }
            };

            string jsonPayload = JsonSerializer.Serialize(payloadObj);

            string apiPath = "/checkout/v2/pay";

            var paymentInitiateUrl = _config.BaseURL + apiPath;


            using var paymentRequest = new HttpRequestMessage(HttpMethod.Post, paymentInitiateUrl);
            paymentRequest.Headers.Authorization = new AuthenticationHeaderValue("O-Bearer", accessToken);
            paymentRequest.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            paymentRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");


            using var paymentResponse = await _httpClient.SendAsync(paymentRequest);

            var paymentResponseContent = await paymentResponse.Content.ReadAsStringAsync();

            if (!paymentResponse.IsSuccessStatusCode)
                throw new Exception($"Payment initiation failed with status {paymentResponse.StatusCode}: {paymentResponseContent}");

            using var paymentDoc = JsonDocument.Parse(paymentResponseContent);
            var root = paymentDoc.RootElement; 
            string orderId1 = root.GetProperty("orderId").GetString();
            string state = root.GetProperty("state").GetString();
            string redirectUrl1 = root.GetProperty("redirectUrl").GetString();
            long expireAt = root.GetProperty("expireAt").GetInt64();

            var usma = await _context.UserStudyMaterialAccesses.FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (usma !=null)
            {
                usma.PhonePayOrderId = orderId1;
                usma.PaymentStatus = "PENDING";
                _context.UserStudyMaterialAccesses.Update(usma);
                await _context.SaveChangesAsync();
            }

            return redirectUrl1;
        }





        public async Task<string> CreateFlashCardPaymentRedirectUrl(FlashCard flashCard, string orderId, string userId, double amountPaise)
        {
            var tokenUrl = "https://api.phonepe.com/apis/identity-manager/v1/oauth/token";
            var tokenRequest = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", _config.MerchantId),
                new KeyValuePair<string, string>("client_version", "1"),
                new KeyValuePair<string, string>("client_secret", _config.SaltKey),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            };

            using var tokenContent = new FormUrlEncodedContent(tokenRequest);
            var tokenResponse = await _httpClient.PostAsync(tokenUrl, tokenContent);
            var tokenResponseStr = await tokenResponse.Content.ReadAsStringAsync();
            Console.WriteLine("Token response: " + tokenResponseStr);

            if (!tokenResponse.IsSuccessStatusCode)
                throw new Exception($"Token request failed: {tokenResponse.StatusCode}, {tokenResponseStr}");

            using var tokenDoc = JsonDocument.Parse(tokenResponseStr);
            if (!tokenDoc.RootElement.TryGetProperty("access_token", out var accessTokenElement))
                throw new Exception("Token response does not contain access_token");

            var accessToken = accessTokenElement.GetString();

            var payloadObj = new
            {
                //orderId = orderId,
                //merchantId = _config.MerchantId,
                merchantOrderId = $"{orderId}",
                amount = amountPaise,
                expireAfter = 1200,
                metaInfo = new
                {
                    Name = flashCard.Name,
                    userId = $"{userId}",
                    date = DateTime.Now.ToString("o")
                },
                paymentFlow = new
                {
                    type = "PG_CHECKOUT",
                    message = "Payment message used for collect requests",
                    merchantUrls = new
                    {
                        redirectUrl = "https://vedaedtech.com/dashboard/FlashCards/thankyou"
                    }
                }
            };

            string jsonPayload = JsonSerializer.Serialize(payloadObj);

            string apiPath = "/checkout/v2/pay";

            var paymentInitiateUrl = _config.BaseURL + apiPath;


            using var paymentRequest = new HttpRequestMessage(HttpMethod.Post, paymentInitiateUrl);
            paymentRequest.Headers.Authorization = new AuthenticationHeaderValue("O-Bearer", accessToken);
            paymentRequest.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            paymentRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");


            using var paymentResponse = await _httpClient.SendAsync(paymentRequest);

            var paymentResponseContent = await paymentResponse.Content.ReadAsStringAsync();

            if (!paymentResponse.IsSuccessStatusCode)
                throw new Exception($"Payment initiation failed with status {paymentResponse.StatusCode}: {paymentResponseContent}");

            using var paymentDoc = JsonDocument.Parse(paymentResponseContent);
            var root = paymentDoc.RootElement;
            string orderId1 = root.GetProperty("orderId").GetString();
            string state = root.GetProperty("state").GetString();
            string redirectUrl1 = root.GetProperty("redirectUrl").GetString();
            long expireAt = root.GetProperty("expireAt").GetInt64();

            var usma = await _context.UserFlashCardAccessess.FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (usma != null)
            {
                usma.PhonePayOrderId = orderId1;
                usma.PaymentStatus = "PENDING";
                _context.UserFlashCardAccessess.Update(usma);
                await _context.SaveChangesAsync();
            }

            return redirectUrl1;
        }
    }
}
