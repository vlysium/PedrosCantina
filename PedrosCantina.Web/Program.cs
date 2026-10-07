using DotNetEnv;
using PedrosCantina.Library;
using PedrosCantina.Library.Repositories;
using PedrosCantina.Library.Services;

var builder = WebApplication.CreateBuilder(args);

string envPath = Path.Combine(builder.Environment.ContentRootPath, "..", ".env");
Env.Load(envPath);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<DBWorker>();
builder.Services.AddSingleton<EmployeeService>();
builder.Services.AddSingleton<ManagerService>();
builder.Services.AddSingleton<EmployeeRepository>();
builder.Services.AddSingleton<ManagerRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
