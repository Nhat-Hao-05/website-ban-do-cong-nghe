using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[ApiController]
[Route("email")]
public class EmailController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;

    public EmailController(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _httpClientFactory = httpClientFactory;
    }

    // POST /email/send-mailchimp
    [HttpPost("send-mailchimp")]
    public async Task<IActionResult> SendMailchimp([FromBody] MailchimpEmailPayload payload)
    {
        try
        {
            var apiKey = _config["Mailchimp:ApiKey"];        // lưu trữ trong appsettings.json
            var serverPrefix = _config["Mailchimp:Server"];  // ví dụ: us21, us7...
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new System.Uri($"https://{serverPrefix}.api.mailchimp.com/3.0/");
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"anystring:{apiKey}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            // 1) Tạo campaign
            var createBody = new
            {
                type = "regular",
                recipients = new { list_id = payload.AudienceId },
                settings = new
                {
                    subject_line = payload.Subject,
                    preview_text = payload.PreviewText,
                    title = $"Campaign - {payload.Subject}",
                    from_name = payload.FromName,
                    reply_to = payload.ReplyTo
                }
            };

            var createRes = await client.PostAsync("campaigns",
                new StringContent(JsonSerializer.Serialize(createBody), Encoding.UTF8, "application/json"));
            var createJson = await createRes.Content.ReadAsStringAsync();
            if (!createRes.IsSuccessStatusCode) return Ok(new { status = "fail", message = createJson });

            using var doc = JsonDocument.Parse(createJson);
            var campaignId = doc.RootElement.GetProperty("id").GetString();

            // 2) Set content (HTML)
            var html = BuildHtml(payload.HeroImage, payload.HtmlContent);
            var contentBody = new { html };
            var contentRes = await client.PutAsync($"campaigns/{campaignId}/content",
                new StringContent(JsonSerializer.Serialize(contentBody), Encoding.UTF8, "application/json"));
            var contentJson = await contentRes.Content.ReadAsStringAsync();
            if (!contentRes.IsSuccessStatusCode) return Ok(new { status = "fail", message = contentJson });

            // 3) Send campaign
            var sendRes = await client.PostAsync($"campaigns/{campaignId}/actions/send", new StringContent(""));
            var sendJson = await sendRes.Content.ReadAsStringAsync();
            if (!sendRes.IsSuccessStatusCode) return Ok(new { status = "fail", message = sendJson });

            return Ok(new { status = "success", campaignId });
        }
        catch (System.Exception ex)
        {
            return Ok(new { status = "fail", message = ex.Message });
        }
    }

    private static string BuildHtml(string heroImage, string htmlContent)
    {
        var hero = string.IsNullOrWhiteSpace(heroImage) ? "" :
            $"<div style='text-align:center;'><img src='{heroImage}' alt='' style='max-width:100%;height:auto;border-radius:8px;'></div>";

        // Minimal responsive email wrapper
        return $@"
<!doctype html>
<html>
<head>
  <meta charset='utf-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1'>
  <title>Email</title>
</head>
<body style='margin:0;padding:0;background:#f6f8fb;font-family:Arial,sans-serif;'>
  <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
    <tr>
      <td align='center'>
        <table role='presentation' width='640' style='margin:24px;background:#ffffff;border-radius:12px;padding:24px;'>
          <tr><td>
            {hero}
            <div style='font-size:16px;line-height:1.6;color:#222;'>{htmlContent}</div>
          </td></tr>
          <tr><td style='padding-top:24px;color:#666;font-size:12px;text-align:center;'>
            Bạn nhận email này vì đã đăng ký trên website của chúng tôi.
          </td></tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
    }
}

public class MailchimpEmailPayload
{
    public string AudienceId { get; set; }
    public string FromName { get; set; }
    public string ReplyTo { get; set; }
    public string Subject { get; set; }
    public string PreviewText { get; set; }
    public string HeroImage { get; set; }
    public string HtmlContent { get; set; }
}
