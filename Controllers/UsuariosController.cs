using Loggu.Application.DTOs;
using Loggu.Domain.Entity;
using Loggu.Domain.Enums;
using Loggu.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Loggu.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _repo;
        public UsuariosController(IUsuarioRepository repo) => _repo = repo;

        // ----- Mapper DRY (opcional) -----
        private static Usuario ToResponse(Usuario u) => new()
        {
            Id = u.Id,
            Nome = u.Nome,
            Email = u.Email,
            Perfil = u.Perfil,
            Ativo = u.Ativo
        };

        // GET api/usuarios?page=1&pageSize=20
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Usuario>), 200)]
        public async Task<ActionResult<IEnumerable<Usuario>>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            if (page < 1 || pageSize < 1) return BadRequest("page/pageSize inválidos.");

            var items = await _repo.GetAsync(page: page, pageSize: pageSize, ct: ct);
            return Ok(items.Select(ToResponse));
        }

        // GET api/usuarios/5
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Usuario), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Usuario>> GetById(int id, CancellationToken ct = default)
        {
            var u = await _repo.GetByIdAsync(id, ct);
            if (u is null) return NotFound();
            return Ok(ToResponse(u));
        }

        // POST api/usuarios
        [HttpPost]
        [ProducesResponseType(typeof(Usuario), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Usuario>> Create([FromBody] UsuarioCreateDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = new Usuario
            {
                Nome = (dto.Nome ?? string.Empty).Trim(),
                Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
                Perfil = (PerfilUsuario)dto.Perfil,
                Ativo = dto.Ativo is 0 or 1 ? dto.Ativo : 1
            };

            try
            {
                var id = await _repo.CreateAsync(entity, ct);
                return CreatedAtAction(nameof(GetById), new { id }, ToResponse(entity));
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Code == 11000) // chave única (ex.: email)
            {
                return BadRequest("E-mail já cadastrado.");
            }
        }

        // PUT api/usuarios/5
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] UsuarioUpdateDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = await _repo.GetByIdAsync(id, ct);
            if (entity is null) return NotFound();

            entity.Nome = (dto.Nome ?? string.Empty).Trim();
            entity.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
            entity.Perfil = (PerfilUsuario)dto.Perfil;
            entity.Ativo = dto.Ativo is 0 or 1 ? dto.Ativo : entity.Ativo;

            try
            {
                var ok = await _repo.UpdateAsync(entity, ct);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Code == 11000)
            {
                return BadRequest("E-mail já cadastrado.");
            }
        }

        // DELETE api/usuarios/5
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var ok = await _repo.DeleteAsync(id, ct);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
