using Microsoft.AspNetCore.Mvc;
using ApiEmpresa.Models;

namespace ApiEmpresa.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpresasController : ControllerBase
{
    private static List<Empresa> _empresas = new()
    {
        new Empresa { Id = 1, Nombre = "TechCorp", Ruc = "1234567890001", Direccion = "Av. Principal 123" },
        new Empresa { Id = 2, Nombre = "InnoSoft", Ruc = "0987654321001", Direccion = "Calle Secundaria 456" },
        new Empresa { Id = 3, Nombre = "DataSys", Ruc = "1122334455001", Direccion = "Boulevard Central 789" }
    };

    [HttpGet]
    public ActionResult<IEnumerable<Empresa>> GetAll()
    {
        return Ok(_empresas);
    }

    [HttpGet("{id}")]
    public ActionResult<Empresa> GetById(int id)
    {
        var empresa = _empresas.FirstOrDefault(e => e.Id == id);
        if (empresa == null)
            return NotFound(new { message = $"Empresa con id {id} no encontrada" });
        return Ok(empresa);
    }

    [HttpPost]
    public ActionResult<Empresa> Create([FromBody] Empresa empresa)
    {
        empresa.Id = _empresas.Max(e => e.Id) + 1;
        _empresas.Add(empresa);
        return CreatedAtAction(nameof(GetById), new { id = empresa.Id }, empresa);
    }

    [HttpPut("{id}")]
    public ActionResult<Empresa> Update(int id, [FromBody] Empresa empresa)
    {
        var existingEmpresa = _empresas.FirstOrDefault(e => e.Id == id);
        if (existingEmpresa == null)
            return NotFound(new { message = $"Empresa con id {id} no encontrada" });

        existingEmpresa.Nombre = empresa.Nombre;
        existingEmpresa.Ruc = empresa.Ruc;
        existingEmpresa.Direccion = empresa.Direccion;
        return Ok(existingEmpresa);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var empresa = _empresas.FirstOrDefault(e => e.Id == id);
        if (empresa == null)
            return NotFound(new { message = $"Empresa con id {id} no encontrada" });

        _empresas.Remove(empresa);
        return NoContent();
    }
}
