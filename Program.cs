using Amazon.Runtime;
using Amazon.S3;
using MinioWebClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var mvcBuilder = builder.Services.AddControllersWithViews();
if (builder.Environment.IsDevelopment()) 
    mvcBuilder.AddRazorRuntimeCompilation();

builder.Services.AddSingleton<IAmazonS3>(_ =>
{
    var credentials = new BasicAWSCredentials(Config.MinioAccessKey, Config.MinioSecretKey);
    var config = new AmazonS3Config
    {
        ServiceURL = Config.MinioUri.ToString(),
        UseHttp = Config.MinioUri.Scheme == "http",
        ForcePathStyle = true
    };
    
    return new AmazonS3Client(credentials, config);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();