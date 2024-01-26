using LGC.BNP.MIKUNI.Web.Models;
using LGC.BNP.MIKUNI.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.Configure<CookiePolicyOptions>(options =>
{
	// This lambda determines whether user consent for non-essential cookies is needed for a given request.  
	options.CheckConsentNeeded = context => false;
	options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(option =>
	{
		option.LoginPath = "/Security/Signin";
	});

builder.Services.AddTransient<IDatabaseConnectionFactory>(e =>
{
	return new ConnectionFactory(builder.Configuration.GetConnectionString("DBConnection"));
});
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<CookieTempDataProviderOptions>(options => {
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped(typeof(AccountService));
builder.Services.AddScoped(typeof(UtilityService));
builder.Services.AddScoped(typeof(MasterService));
builder.Services.AddScoped(typeof(TransactionService));
builder.Services.AddScoped(typeof(MailService));
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".CLB.BNP.Session";
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(60);
});
builder.Services.AddAntiforgery(options => { options.Cookie.Expiration = TimeSpan.Zero; });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseCookiePolicy();
app.UseSession();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transaction}/{action=Monitor}/{id?}");

app.Run();
