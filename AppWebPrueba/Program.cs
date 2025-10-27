using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Auth/Login";
        o.LogoutPath = "/Auth/Logout";
        o.AccessDeniedPath = "/Auth/Denegado";
        o.SlidingExpiration = true;
        o.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminUOperador", p => p.RequireRole("Administrador", "Operador"));
    options.AddPolicy("SoloAdmin", p => p.RequireRole("Administrador"));
    options.AddPolicy("PuedeEliminar", p => p.RequireRole("Administrador"));
    options.AddPolicy("PuedeVerReportes", p => p.RequireRole("Administrador"));
});

builder.Services.AddHttpClient("Api", c =>
{
    c.BaseAddress = new Uri("https://localhost:7260/");
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<AppWebPrueba.Services.ApiClientFactory>();
builder.Services.AddScoped<AppWebPrueba.Services.JwtTokenService>();

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

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
