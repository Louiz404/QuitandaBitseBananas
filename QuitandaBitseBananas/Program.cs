using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuitandaBitseBananas.Data;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar o DbContext
builder.Services.AddDbContext<QuitandaBitseBananasContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QuitandaBitseBananasContext") ?? throw new InvalidOperationException("Connection string 'QuitandaBitseBananasContext' not found.")));


// --- BLOCO DE CONFIGURAÇÃO DO IDENTITY (LOGIN) ---
builder.Services.AddDefaultIdentity<Microsoft.AspNetCore.Identity.IdentityUser>(options =>
{
    // Configurações simplificadas para facilitar seus testes
    options.SignIn.RequireConfirmedAccount = false; // Não precisa confirmar email
    options.Password.RequireDigit = false;          // Aceita senha sem número
    options.Password.RequireLowercase = false;      // Aceita senha só maiúscula
    options.Password.RequireUppercase = false;      // Aceita senha só minúscula
    options.Password.RequireNonAlphanumeric = false;// Aceita senha sem @#$
    options.Password.RequiredLength = 3;            // Senha curta (ex: "123")
})
    .AddEntityFrameworkStores<QuitandaBitseBananasContext>(); // Conecta com seu banco
// ------------------

// Configurar a cultura da aplicação
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("pt-BR")
    };

    options.DefaultRequestCulture = new RequestCulture("pt-BR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Habilitar a localização
app.UseRequestLocalization();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // O valor padrão do HSTS é 30 dias. Você pode querer mudar isso para cenários de produção.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// seguir essa ordem
app.UseAuthentication(); // O que o usuário pode fazer
app.UseAuthorization(); // Quem é o usuário


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

WebApplication QuitandaBits = app;

QuitandaBits.Run();
