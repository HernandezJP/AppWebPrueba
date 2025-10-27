using AppWebPrueba.Models.Auth;
using AppWebPrueba.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace AppWebPrueba.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly JwtTokenService _jwtSvc;

        public AuthController(IHttpClientFactory http, JwtTokenService jwtSvc)
        {
            _http = http;
            _jwtSvc = jwtSvc;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new Login());
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(Login login, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.Contrasena))
            {
                ModelState.AddModelError("", "Credenciales inválidas.");
                return View(login);
            }

            var api = _http.CreateClient("Api");
            var payload = JsonSerializer.Serialize(new { email = login.Email, contrasena = login.Contrasena });
            var resp = await api.PostAsync("api/Autenticacion/validar",
                new StringContent(payload, Encoding.UTF8, "application/json"));

            if (!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                return View(login);
            }

            using var doc = await JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync());
            var token = doc.RootElement.GetProperty("token").GetString() ?? "";

            
            HttpContext.Session.SetString("JWT", token);

            
            var principal = _jwtSvc.CreatePrincipalFromToken(token);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("JWT");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult Denegado() => View();
    }
}
