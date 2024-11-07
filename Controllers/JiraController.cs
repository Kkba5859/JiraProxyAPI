using Microsoft.AspNetCore.Mvc;
using Atlassian.Jira;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JiraProxyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JiraController : ControllerBase
    {
        private readonly Jira _jira;

        public JiraController()
        {
            // Настройка соединения с Jira с использованием ваших учетных данных
            var email = "kkba5859@gmail.com"; // Ваш email
            var apiToken = "ATATT3xFfGF0zcamIJKlaeNNXPCzq-cHQc8nmR7xFH1UdCoFyzElfPB9x04P77ZZiY7ldnBls-dzX_v7zR7ND5wIBC81yXkeOrFz8UIWquIQB8aLnN2a7dbBr05BaEufh8WI3OgLkF5GxG40OurfAFSgG-avuQ6esTOnGBPRClhILmV9XPw4BOA=A7C81C33"; // Ваш API токен
            _jira = Jira.CreateRestClient("https://kba5859.atlassian.net", email, apiToken); // URL вашего домена
        }

        [HttpPost("CreateTicket")]
        public async Task<IActionResult> CreateTicket([FromBody] TicketRequest request)
        {
            // Проверка на обязательное поле Summary
            if (string.IsNullOrWhiteSpace(request.Summary))
            {
                return BadRequest("Поле 'Summary' обязательно для заполнения.");
            }

            try
            {
                // Создаем задачу в Jira
                var ticketId = await CreateJiraIssue(request);
                return Ok(new { TicketLink = $"https://kba5859.atlassian.net/browse/{ticketId}" });
            }
            catch (Exception ex)
            {
                // Обработка ошибки при создании задачи
                return StatusCode(500, $"Ошибка при создании задачи в Jira: {ex.Message}");
            }
        }

        private async Task<string> CreateJiraIssue(TicketRequest request)
        {
            const string projectKey = "SCRUM"; // Ключ проекта в Jira

            // Создаем задачу типа "Bug" в проекте
            var issue = _jira.CreateIssue(projectKey);
            issue.Type = "Bug"; // Задаем тип задачи как "Bug"
            issue.Summary = request.Summary;
            issue.Description = request.Description;

            // Устанавливаем приоритет задачи
            if (request.Priority == "High")
                issue.Priority = _jira.Priorities.GetPriorityByName("High");
            else if (request.Priority == "Average")
                issue.Priority = _jira.Priorities.GetPriorityByName("Medium");
            else
                issue.Priority = _jira.Priorities.GetPriorityByName("Low");

            // Устанавливаем статус задачи
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                var status = _jira.Statuses.GetStatusByName(request.Status);
                issue.Status = status;
            }

            // Дополнительно можно обработать поле Template, если оно используется
            if (!string.IsNullOrEmpty(request.Template))
            {
                issue.Description += $"\n\nTemplate: {request.Template}";
            }

            // Сохраняем изменения и возвращаем идентификатор задачи
            await issue.SaveChangesAsync();
            return issue.Key.Value;
        }

        public class TicketRequest
        {
            [JsonProperty(nameof(Summary))]
            public string Summary { get; set; }

            [JsonProperty(nameof(Description))]
            public string Description { get; set; }

            [JsonProperty(nameof(Priority))]
            public string Priority { get; set; }  // Пример: "High", "Average", "Low"

            [JsonProperty(nameof(Status))]
            public string Status { get; set; }  // Пример: "Opened", "In Progress", "Rejected", "Fixed"

            [JsonProperty(nameof(Template))]
            public string Template { get; set; }  // Пример: может быть пустым или содержать имя шаблона
        }
    }
}
