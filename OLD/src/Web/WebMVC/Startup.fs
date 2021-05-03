namespace WebMVC

open System.IdentityModel.Tokens.Jwt
open System.Threading.Tasks
open Microsoft.AspNetCore.Authentication.OpenIdConnect
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

type Startup private () =
    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    member this.ConfigureServices(services: IServiceCollection) =
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services.AddControllersWithViews().AddRazorRuntimeCompilation() |> ignore
        services.AddRazorPages() |> ignore
        services
            .AddAuthorization()
            .AddAuthentication(fun opt ->
                opt.DefaultScheme <- "Cookies"
                opt.DefaultChallengeScheme <- "oidc")
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", fun opt ->
                opt.Authority <- "https://localhost:5001/"
                
                opt.ClientId <- "mvc"
                opt.ClientSecret <- "secret"
                opt.ResponseType <- "code"
                
                opt.SaveTokens <- true) |> ignore

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =

        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
        else
            app.UseExceptionHandler("/Home/Error") |> ignore
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts() |> ignore
        
        app.UseHttpsRedirection() |> ignore
        app.UseStaticFiles() |> ignore

        app.UseRouting() |> ignore

        app.UseAuthentication() |> ignore
        app.UseAuthorization() |> ignore

        app.UseEndpoints(fun endpoints ->
            endpoints.MapControllerRoute(
                name = "default",
                pattern = "{controller=Home}/{action=Index}/{id?}") |> ignore
            endpoints.MapRazorPages() |> ignore) |> ignore

    member val Configuration : IConfiguration = null with get, set
