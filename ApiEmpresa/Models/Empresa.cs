namespace ApiEmpresa.Models;

public class Empresa
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Ruc { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
}
