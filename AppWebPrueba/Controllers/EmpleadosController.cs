using AppWebPrueba.Models;
using AppWebPrueba.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AppWebPrueba.Controllers
{
    [Authorize(Policy = "AdminUOperador")]
    public class EmpleadosController : Controller
    {
            private readonly ApiClientFactory _api;
            private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

            public EmpleadosController(ApiClientFactory api) => _api = api;

            private async Task LoadDepartamentosAsync(int? selectedId = null)
            {
                var client = _api.Create();
                var resp = await client.GetAsync("api/Departamentos");
                var lista = new List<DepartamentoItem>();

                if (resp.IsSuccessStatusCode)
                {
                    var data = JsonSerializer.Deserialize<List<DepartamentoItem>>(
                        await resp.Content.ReadAsStringAsync(), _json) ?? new();
                    lista = data.OrderBy(d => d.NombreDepto).ToList();
                }

                ViewBag.Departamentos = lista.Select(d =>
                    new SelectListItem { Value = d.IdDepartamento.ToString(), Text = d.NombreDepto, Selected = selectedId == d.IdDepartamento }
                ).ToList();
            }

            // GET /Empleados
            public async Task<IActionResult> Index()
            {
                var client = _api.Create();
                var response = await client.GetAsync("api/Empleados");
                if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);

                var data = JsonSerializer.Deserialize<List<EmpleadoResponse>>(
                    await response.Content.ReadAsStringAsync(), _json) ?? new();
                return View(data);
            }

            // GET /Empleados/Details/5
            public async Task<IActionResult> Details(string id)
            {
                var client = _api.Create();
                var response = await client.GetAsync($"api/Empleados/{id}");
                if (response.StatusCode == HttpStatusCode.NotFound) return NotFound();
                if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);

                var deta = JsonSerializer.Deserialize<EmpleadoResponse>(
                    await response.Content.ReadAsStringAsync(), _json)!;
                return View(deta);
            }

            // GET /Empleados/Create
            public async Task<IActionResult> Create()
            {
                await LoadDepartamentosAsync();
                return View(new EmpleadoCreate());
            }

            // POST /Empleados/Create
            [HttpPost, ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(EmpleadoCreate creat)
            {
                if (!ModelState.IsValid)
                {
                    await LoadDepartamentosAsync(creat.DepartamentoId);
                    return View(creat);
                }

                var client = _api.Create();
                var body = JsonSerializer.Serialize(new
                {
                    Nombre = creat.Nombre,
                    Apellidos = creat.Apellidos,
                    DocumentoCui = creat.DocumentoCui,
                    FechaIngreso = creat.FechaIngreso,
                    SalarioActual = creat.SalarioActual,
                    FechaUltimoAumento = creat.FechaUltimoAumento,
                    FechaBaja = creat.FechaBaja,
                    Puesto = creat.Puesto,
                    Jerarquia = creat.Jerarquia,
                    DepartamentoId = creat.DepartamentoId
                });

                var response = await client.PostAsync(
                    "api/Empleados",
                    new StringContent(body, Encoding.UTF8, "application/json"));

                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    ModelState.AddModelError("", "Conflicto: Verifica CUI o restricciones de negocio.");
                    await LoadDepartamentosAsync(creat.DepartamentoId);
                    return View(creat);
                }
                if (!response.IsSuccessStatusCode)
                {
                    var errorText = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error API {(int)response.StatusCode}: {errorText}");
                    await LoadDepartamentosAsync(creat.DepartamentoId);
                    return View(creat);
                }

                return RedirectToAction(nameof(Index));
            }

            // GET /Empleados/Update/5
            public async Task<IActionResult> Update(string id)
            {
                var client = _api.Create();
                var response = await client.GetAsync($"api/Empleados/{id}");
                if (response.StatusCode == HttpStatusCode.NotFound) return NotFound();
                if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);

                var data = JsonSerializer.Deserialize<EmpleadoResponse>(
                    await response.Content.ReadAsStringAsync(), _json)!;

                var upda = new EmpleadoUpdate
                {
                    Nombre = data.Nombre,
                    Apellidos = data.Apellidos,
                    DocumentoCui = data.DocumentoCui,
                    FechaIngreso = data.FechaIngreso,
                    SalarioActual = data.SalarioActual,
                    FechaUltimoAumento = data.FechaUltimoAumento,
                    FechaBaja = data.FechaBaja,
                    Puesto = data.Puesto,
                    Jerarquia = data.Jerarquia,
                    DepartamentoId = data.DepartamentoId
                };

                await LoadDepartamentosAsync(upda.DepartamentoId);
                return View(upda);
            }

            // POST /Empleados/Update
            [HttpPost, ValidateAntiForgeryToken]
            public async Task<IActionResult> Update(EmpleadoUpdate upda)
            {
                if (!ModelState.IsValid)
                {
                    await LoadDepartamentosAsync(upda.DepartamentoId);
                    return View(upda);
                }

                var client = _api.Create();
                var body = JsonSerializer.Serialize(new
                {
                    Nombre = upda.Nombre,
                    Apellidos = upda.Apellidos,
                    DocumentoCui = upda.DocumentoCui,
                    FechaIngreso = upda.FechaIngreso,
                    SalarioActual = upda.SalarioActual,
                    FechaUltimoAumento = upda.FechaUltimoAumento,
                    FechaBaja = upda.FechaBaja,
                    Puesto = upda.Puesto,
                    Jerarquia = upda.Jerarquia,
                    DepartamentoId = upda.DepartamentoId
                });

                var response = await client.PutAsync(
                    $"api/Empleados/{upda.DocumentoCui}",
                    new StringContent(body, Encoding.UTF8, "application/json"));

                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    ModelState.AddModelError("", "Conflicto: CUI duplicado u otra restricción.");
                    await LoadDepartamentosAsync(upda.DepartamentoId);
                    return View(upda);
                }
                if (response.StatusCode == HttpStatusCode.NotFound) return NotFound();
                if (!response.IsSuccessStatusCode)
                {
                    var errorText = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error API {(int)response.StatusCode}: {errorText}");
                    await LoadDepartamentosAsync(upda.DepartamentoId);
                    return View(upda);
                }

                return RedirectToAction(nameof(Index));
            }

            // POST /Empleados/Delete/5  (solo Admin)
            [Authorize(Policy = "SoloAdmin")]
            [HttpPost, ValidateAntiForgeryToken]
            public async Task<IActionResult> Delete(string id)
            {
                var client = _api.Create();
                var response = await client.DeleteAsync($"api/Empleados/{id}");

                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    TempData["Error"] = "No se puede eliminar: empleado en uso o con dependencias.";
                    return RedirectToAction(nameof(Index));
                }
                if (response.StatusCode == HttpStatusCode.NotFound) return NotFound();
                if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);

                return RedirectToAction(nameof(Index));
            }
    }
}
