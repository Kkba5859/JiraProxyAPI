using Microsoft.AspNetCore.Mvc;
using Atlassian.Jira;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace JiraProxyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FetchIssuesController : ControllerBase
    {
        private readonly Jira _jira;

        public FetchIssuesController()
        {
            var email = "kkba5859@gmail.com"; // Ваш email
            var apiToken = "ATATT3xFfGF0zcamIJKlaeNNXPCzq-cHQc8nmR7xFH1UdCoFyzElfPB9x04P77ZZiY7ldnBls-dzX_v7zR7ND5wIBC81yXkeOrFz8UIWquIQB8aLnN2a7dbBr05BaEufh8WI3OgLkF5GxG40OurfAFSgG-avuQ6esTOnGBPRClhILmV9XPw4BOA=A7C81C33"; // Ваш API токен
            _jira = Jira.CreateRestClient("https://kba5859.atlassian.net", email, apiToken); // URL вашего домена
        }

        [HttpGet("issues")]
        public async Task<IActionResult> GetUserIssues()
        {
            try
            {
                var jqlQuery = "project=\"SCRUM\""; // Используем ключ проекта SCRUM
                // Получаем задачи из Jira с помощью JQL запроса
                var issues = await _jira.Issues.GetIssuesFromJqlAsync(jqlQuery);

                // Проверяем, есть ли задачи в списке
                var issuesList = issues.ToList(); // Преобразуем в список

                if (issuesList.Count == 0) // Проверяем, если задач нет
                {
                    return NotFound("Не найдено задач для проекта SCRUM.");
                }

                // Преобразуем задачи в нужный формат и возвращаем
                var result = new
                {
                    Issues = issuesList.Select(issue => new
                    {
                        Key = issue.Key.Value, // Извлекаем значение из поля key
                        issue.Summary,
                        issue.Description,
                        Assignee = issue.Assignee, // Здесь просто берем строку, без DisplayName
                        Reporter = issue.Reporter, // То же самое для Reporter
                        issue.Created
                    }).ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Возвращаем ошибку в случае исключения
                return StatusCode(500, $"Ошибка при получении задач из Jira: {ex.Message}");
            }
        }
    }
}
