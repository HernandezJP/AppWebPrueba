namespace AppWebPrueba.Models
{
    public class DepartamentoResponse
    {
        public int IdDepartamento { get; set; }
        public string NombreDepto { get; set; } = string.Empty;
        public decimal Presupuesto { get; set; }
    }

    public class DepartamentoCreate
    {
        public int IdDepartamento { get; set; }
        public string NombreDepto { get; set; } = string.Empty;
        public decimal Presupuesto { get; set; }
    }

    public class DepartamentoUpdate
    {
        public int IdDepartamento { get; set; }
        public string NombreDepto { get; set; } = string.Empty;
        public decimal Presupuesto { get; set; }
    }
}
