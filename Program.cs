namespace JiraProxyAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddHttpClient(); // Добавляем HttpClient

            // Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // CORS настройки
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()  // Разрешить все домены
                          .AllowAnyMethod()  // Разрешить все методы
                          .AllowAnyHeader();  // Разрешить любые заголовки
                });
            });

            var app = builder.Build();

            // Настройка конвейера HTTP-запросов
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Jira Proxy API V1");
                c.RoutePrefix = "swagger"; // Чтобы Swagger был доступен по /swagger
            });

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");  // Применяем CORS-политику
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
