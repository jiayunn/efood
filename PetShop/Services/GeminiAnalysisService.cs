using Newtonsoft.Json;
using System;
using System.Linq; // 需要這個來使用 FirstOrDefault()
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace PetShop.Services
{
    public class GeminiAnalysisService : IGeminiAnalysisService
    {
        private readonly string _apiKey;
        private static readonly HttpClient client = new HttpClient();

        public GeminiAnalysisService()
        {
            _apiKey = WebConfigurationManager.AppSettings["GeminiApiKey"];
        }

        public async Task<string> GetDietAnalysisAsync(string prompt)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                return "錯誤：Gemini API 金鑰未在 Web.config 中設定。";
            }
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(url, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<GeminiResponse>(responseString);

                    if (responseObject?.candidates?.FirstOrDefault()?.content?.parts?.FirstOrDefault()?.text != null)
                    {
                        return responseObject.candidates[0].content.parts[0].text;
                    }
                    return "成功收到回覆，但內容為空。";
                }
                else
                {
                    var errorString = await response.Content.ReadAsStringAsync();
                    return $"API 請求失敗，狀態碼：{response.StatusCode}，錯誤訊息：{errorString}";
                }
            }
            catch (Exception ex)
            {
                return $"呼叫 Gemini API 時發生網路錯誤: {ex.Message}";
            }
        }
    }

    // --- 輔助解析 Gemini API 回應的類別 ---
    public class GeminiResponse { public Candidate[] candidates { get; set; } }
    public class Candidate { public Content content { get; set; } }
    public class Content { public Part[] parts { get; set; } public string role { get; set; } }
    public class Part { public string text { get; set; } }
}