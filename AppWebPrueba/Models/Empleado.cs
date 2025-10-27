
namespace AppWebPrueba.Models
{
    public class EmpleadoResponse
    {
        public int IdEmpleado { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string DocumentoCui { get; set; } = string.Empty;
        public DateTime FechaIngreso { get; set; }
        public decimal SalarioActual { get; set; }
        public DateTime? FechaUltimoAumento { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string? Puesto { get; set; }
        public string? Jerarquia { get; set; }
        public int DepartamentoId { get; set; }
        public string? NombreDepartamento { get; set; }
    }

    public class EmpleadoCreate
    {

        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string DocumentoCui { get; set; } = string.Empty;
        public DateTime FechaIngreso { get; set; } = DateTime.Today;
        public decimal SalarioActual { get; set; }
        public DateTime? FechaUltimoAumento { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string? Puesto { get; set; }
        public string? Jerarquia { get; set; }
        public int DepartamentoId { get; set; }
    }

    public class EmpleadoUpdate : EmpleadoCreate
    {
        public int IdEmpleado { get; set; }
    }

    // Para llenar el dropdown
    public class DepartamentoItem
    {
        public int IdDepartamento { get; set; }
        public string NombreDepto { get; set; } = string.Empty;
    }
}
