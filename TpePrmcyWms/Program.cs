using Microsoft.EntityFrameworkCore;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Service;
using System.Configuration;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".TpePrmcyWms.Session";
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
});
builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All)); //避免ViewBag,ViewData,TempData中文亂碼

builder.Services.AddRazorPages()
    .AddMvcOptions(options =>
    {        
        options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "此為必填欄位");

    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{fid?}");

app.Run();
