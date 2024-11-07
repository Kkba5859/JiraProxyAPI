using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace JiraProxyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JiraMetaController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _email = "kkba5859@gmail.com"; // Ваш email
        private readonly string _apiToken = "ATATT3xFfGF0zcamIJKlaeNNXPCzq-cHQc8nmR7xFH1UdCoFyzElfPB9x04P77ZZiY7ldnBls-dzX_v7zR7ND5wIBC81yXkeOrFz8UIWquIQB8aLnN2a7dbBr05BaEufh8WI3OgLkF5GxG40OurfAFSgG-avuQ6esTOnGBPRClhILmV9XPw4BOA=A7C81C33"; // Ваш API токен

        public JiraMetaController()
        {
            _httpClient = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes($"{_email}:{_apiToken}");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        [HttpGet("CreateMeta")]
        public async Task<IActionResult> GetCreateMeta()
        {
            var url = "https://kba5859.atlassian.net/rest/api/2/issue/createmeta";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    return Ok(jsonResponse);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении метаданных Jira: {ex.Message}");
            }
        }
    }
}
