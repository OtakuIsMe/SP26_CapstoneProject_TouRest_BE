using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Microsoft.Extensions.Configuration;
using TouRest.Application.Common.Constants;
using TouRest.Application.DTOs.Payment;
using TouRest.Application.Interfaces;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class VNPayService : IVNPayService
    {
        private readonly string _tmnCode;
        private readonly string _secureSecret;
        private readonly string _returnUrl;
        private readonly IBookingRepository _bookingRepo;
        private readonly HttpClient _http;

        private static readonly TimeZoneInfo Gmt7 =
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public VNPayService(IConfiguration config, IBookingRepository bookingRepo, HttpClient http)
        {
            _tmnCode      = config["VNPay:TmnCode"]     ?? throw new InvalidOperationException("VNPay:TmnCode not configured");
            _secureSecret = config["VNPay:SecureSecret"] ?? throw new InvalidOperationException("VNPay:SecureSecret not configured");
            _returnUrl    = config["VNPay:ReturnUrl"]    ?? throw new InvalidOperationException("VNPay:ReturnUrl not configured");
            _bookingRepo  = bookingRepo;
            _http         = http;
        }

        public async Task<VNPayQrResponse> GenerateQrAsync(Guid bookingId, string ipAddr, int expireMinutes = 15)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
                return new VNPayQrResponse { Code = "99", Message = "Booking not found" };

            var now    = TimeZoneInfo.ConvertTime(DateTime.UtcNow, Gmt7);
            var expire = now.AddMinutes(expireMinutes);

            var sortedParams = new SortedDictionary<string, string>
            {
                ["vnp_Amount"]      = ((long)booking.TotalAmount * 100).ToString(),
                ["vnp_Command"]     = VNPayCode.Command,
                ["vnp_CreateDate"]  = now.ToString("yyyyMMddHHmmss"),
                ["vnp_CurrCode"]    = VNPayCode.CurrCode,
                ["vnp_ExpireDate"]  = expire.ToString("yyyyMMddHHmmss"),
                ["vnp_IpAddr"]      = string.IsNullOrWhiteSpace(ipAddr) ? "127.0.0.1" : ipAddr,
                ["vnp_Locale"]      = VNPayCode.Locale,
                ["vnp_OrderInfo"]   = $"Thanh toan dat tour {booking.Code}",
                ["vnp_OrderType"]   = VNPayCode.OrderType,
                ["vnp_ReturnUrl"]   = _returnUrl,
                ["vnp_TmnCode"]     = _tmnCode,
                ["vnp_TxnRef"]      = booking.Code,
                ["vnp_Version"]     = VNPayCode.Version,
            };

            var rawData    = BuildRawData(sortedParams);
            var secureHash = HmacSha512(_secureSecret, rawData);
            var query      = BuildQueryString(sortedParams);
            var url        = $"{VNPayCode.PaymentUrl}?{query}&vnp_SecureHash={secureHash}";

            try
            {
                var httpResponse = await _http.GetAsync(url);
                httpResponse.EnsureSuccessStatusCode();

                var json = await httpResponse.Content.ReadAsStringAsync();
                var raw  = JsonSerializer.Deserialize<VnPayRawResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return new VNPayQrResponse
                {
                    Code      = raw?.Code      ?? "99",
                    Message   = raw?.Message   ?? "Unknown error",
                    QrContent = raw?.QrContent ?? "",
                };
            }
            catch (Exception ex)
            {
                return new VNPayQrResponse { Code = "99", Message = ex.Message };
            }
        }

        private static string BuildRawData(SortedDictionary<string, string> p) =>
            string.Join("&", p.Select(kv => $"{kv.Key}={kv.Value}"));

        private static string BuildQueryString(SortedDictionary<string, string> p) =>
            string.Join("&", p.Select(kv => $"{kv.Key}={HttpUtility.UrlEncode(kv.Value)}"));

        private static string HmacSha512(string key, string data)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).ToLower();
        }

        private class VnPayRawResponse
        {
            [JsonPropertyName("code")]
            public string Code { get; set; } = "";

            [JsonPropertyName("message")]
            public string Message { get; set; } = "";

            [JsonPropertyName("qrcontent")]
            public string QrContent { get; set; } = "";
        }
    }
}
