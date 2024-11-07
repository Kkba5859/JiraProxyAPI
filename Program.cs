namespace JiraProxyAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddHttpClient(); // ��������� HttpClient

            // Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // CORS ���������
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()  // ��������� ��� ������
                          .AllowAnyMethod()  // ��������� ��� ������
                          .AllowAnyHeader();  // ��������� ����� ���������
                });
            });

            var app = builder.Build();

            // ��������� ��������� HTTP-��������
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Jira Proxy API V1");
                c.RoutePrefix = "swagger"; // ����� Swagger ��� �������� �� /swagger
            });

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");  // ��������� CORS-��������
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
