using Microsoft.AspNetCore.Authentication.Cookies;
using Booking_Mvc.Services;

var builder = WebApplication.CreateBuilder(args);

// -------------------------
// Ajout des services MVC
// -------------------------
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// -------------------------
// Configuration HttpClient pour appeler ton WebAPI
// -------------------------
builder.Services.AddHttpClient("BookingAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5289/"); // URL de ton WebAPI
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// -------------------------
// Authentification Cookie
// -------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";          // Redirige si non connecté
        options.AccessDeniedPath = "/Account/AccessDenied"; // Redirige si rôle non autorisé
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(1); // Durée du cookie
        options.SlidingExpiration = true;              // Renouvellement automatique
    });

// -------------------------
// Autorisation
// -------------------------
builder.Services.AddAuthorization();
// -------------------------
// Injection de service personnalisé pour API
// -------------------------
builder.Services.AddScoped<UserService>(); // service que je t'avais montré pour Dashboard

//--------------------------
// Session + Token
//--------------------------
builder.Services.AddSession();

var app = builder.Build();

// -------------------------
// Middleware
// -------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection(); // Décommenter si tu passes en HTTPS

app.UseStaticFiles();
app.UseRouting();
app.UseDeveloperExceptionPage();
app.UseAuthentication();  // Toujours avant UseAuthorization
app.UseAuthorization();
app.UseSession();

// -------------------------
// Routes MVC
// -------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();