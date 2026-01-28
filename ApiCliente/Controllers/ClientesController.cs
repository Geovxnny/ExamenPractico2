using Microsoft.AspNetCore.Mvc;
using ApiCliente.Models;

namespace ApiCliente.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private static List<Cliente> _clientes = new()
    {
        new Cliente { Id = 1, Nombre = "Juan", Apellido = "Pérez", Telefono = "0991234567" },
        new Cliente { Id = 2, Nombre = "María", Apellido = "García", Telefono = "0987654321" },
        new Cliente { Id = 3, Nombre = "Carlos", Apellido = "López", Telefono = "0998765432" }
    };

    [HttpGet]
    public ActionResult<IEnumerable<Cliente>> GetAll()
    {
        return Ok(_clientes);
    }

    [HttpGet("{id}")]
    public ActionResult<Cliente> GetById(int id)
    {
        var cliente = _clientes.FirstOrDefault(c => c.Id == id);
        if (cliente == null)
            return NotFound(new { message = $"Cliente con id {id} no encontrado" });
        return Ok(cliente);
    }

    [HttpPost]
    public ActionResult<Cliente> Create([FromBody] Cliente cliente)
    {
        cliente.Id = _clientes.Max(c => c.Id) + 1;
        _clientes.Add(cliente);
        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id}")]
    public ActionResult<Cliente> Update(int id, [FromBody] Cliente cliente)
    {
        var existingCliente = _clientes.FirstOrDefault(c => c.Id == id);
        if (existingCliente == null)
            return NotFound(new { message = $"Cliente con id {id} no encontrado" });

        existingCliente.Nombre = cliente.Nombre;
        existingCliente.Apellido = cliente.Apellido;
        existingCliente.Telefono = cliente.Telefono;
        return Ok(existingCliente);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var cliente = _clientes.FirstOrDefault(c => c.Id == id);
        if (cliente == null)
            return NotFound(new { message = $"Cliente con id {id} no encontrado" });

        _clientes.Remove(cliente);
        return NoContent();
    }
}
