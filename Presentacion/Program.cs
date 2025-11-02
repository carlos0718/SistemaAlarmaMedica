using Presentacion.Middleware;
using Presentacion.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IFarmacoServiceWeb, FarmacoServiceWeb>();
builder.Services.AddTransient<IMedicoServiceWeb, MedicoServiceWeb>();
builder.Services.AddTransient<IPacienteServiceWeb, PacienteServiceWeb>();
builder.Services.AddTransient<IUsuarioServiceWeb, UsuarioServiceWeb>();
builder.Services.AddTransient<IOrdenMedicaServiceWeb, OrdenMedicaServiceWeb>();
builder.Services.AddTransient<ITurnoServiceWeb, TurnoServiceWeb>();

builder.Services.AddHttpClient<HttpClientService>();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configurar autenticación con Google
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
})
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/Login/Index";
    options.LogoutPath = "/Login/Logout";
    options.AccessDeniedPath = "/Home/AccessDenied";
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    googleOptions.CallbackPath = "/signin-google";
    googleOptions.SaveTokens = true;
    googleOptions.SignInScheme = "Cookies";

    // Solicitar información básica del perfil
    googleOptions.Scope.Clear();
    googleOptions.Scope.Add("openid");
    googleOptions.Scope.Add("profile");
    googleOptions.Scope.Add("email");
});

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
app.UseSessionValidation();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
