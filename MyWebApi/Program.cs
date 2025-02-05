var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Get the port assigned by AWS Elastic Beanstalk
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";

// ✅ Fix: Bind to all available network interfaces
app.Urls.Add($"http://0.0.0.0:{port}");

app.MapGet("/", () => "Hello, välkommen till Abdullahi Elastic Beanstalk!");

app.Run();
