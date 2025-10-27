using AppWebPrueba.Models;
using AppWebPrueba.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace AppWebPrueba.Controllers
{
    [Authorize(Policy = "AdminUOperador")]
    public class DepartamentosController : Controller
    {
        private readonly ApiClientFactory _api;
        private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public DepartamentosController(ApiClientFactory api) => _api = api;

        // GET /Departamentos
        public async Task<IActionResult> Index()
        {
            var client = _api.Create();
            var response = await client.GetAsync("api/Departamentos");
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);

            var data = JsonSerializer.Deserialize<List<DepartamentoResponse>>(
                await response.Content.ReadAsStringAsync(), _json) ?? new();
            return View(data);
        }

        // GET /Departamentos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var client = _api.Create();
            var response = await client.GetAsync($"api/Departamentos/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return NotFound();
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);

            var vm = JsonSerializer.Deserialize<DepartamentoUpdate>(
                await response.Content.ReadAsStringAsync(), _json)!;
            return View(vm);
        }

        // GET /Departamentos/Create
        public IActionResult Create() => View(new DepartamentoCreate());

        // POST /Departamentos/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartamentoCreate create)
        {
            if (!ModelState.IsValid) return View(create);

            var client = _api.Create();
            var body = JsonSerializer.Serialize(new
            {
                IdDepartamento = create.IdDepartamento,           // tu API lo ignora/autogenera
                NombreDepto = create.NombreDepto,
                Presupuesto = create.Presupuesto
            });
            var response = await client.PostAsync(
                "api/Departamentos",
                new StringContent(body, Encoding.UTF8, "application/json"));

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                ModelState.AddModelError("", "El nombre ya existe.");
                return View(create);
            }
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);

            return RedirectToAction(nameof(Index));
        }

        // GET /Departamentos/Edit/5
        public async Task<IActionResult> Update(int id)
        {
            var client = _api.Create();
            var response = await client.GetAsync($"api/Departamentos/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return NotFound();
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);

            var update = JsonSerializer.Deserialize<DepartamentoUpdate>(
                await response.Content.ReadAsStringAsync(), _json)!;
            return View(update);
        }

        // POST /Departamentos/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(DepartamentoUpdate depUp)
        {
            if (!ModelState.IsValid) return View(depUp);

            var client = _api.Create();
            var body = JsonSerializer.Serialize(new
            {
                NombreDepto = depUp.NombreDepto,
                Presupuesto = depUp.Presupuesto
            });
            var response = await client.PutAsync(
                $"api/Departamentos/{depUp.IdDepartamento}",
                new StringContent(body, Encoding.UTF8, "application/json"));

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                ModelState.AddModelError("", "El nombre ya existe.");
                return View(depUp);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return NotFound();
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);

            return RedirectToAction(nameof(Index));
        }

        // POST /Departamentos/Delete/5  (solo Admin)
        [Authorize(Policy = "SoloAdmin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _api.Create();
            var response = await client.DeleteAsync($"api/Departamentos/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                TempData["Error"] = "No se puede eliminar: hay empleados vinculados.";
                return RedirectToAction(nameof(Index));
            }
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return NotFound();
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);

            return RedirectToAction(nameof(Index));
        }
    }
}

