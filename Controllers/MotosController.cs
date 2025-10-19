using Loggu.Application.DTOs;
using Loggu.Domain.Entity;
using Loggu.Domain.Enums;
using Loggu.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Loggu.Controllers
{
    [ApiController]
    [Route("api/motos")]
    public class MotosController : ControllerBase
    {
        private readonly IMotoRepository _repo;
        public MotosController(IMotoRepository repo) => _repo = repo;

        
        private static MotoReadDto ToResponse(Moto m) => new()
        {
            Id = m.Id,
            Placa = m.Placa,
            Chassi = m.Chassi,
            Status = (int)m.Status,
            EmPatio = m.EmPatio
        };

        
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MotoReadDto>), 200)]
        public async Task<ActionResult<IEnumerable<MotoReadDto>>> Get(
            [FromQuery] int? status = null,
            [FromQuery] string? placa = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            if (page < 1 || pageSize < 1) return BadRequest("page/pageSize inválidos.");

            var items = await _repo.GetAsync(status, placa, page, pageSize, ct);
            return Ok(items.Select(ToResponse));
        }

        
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MotoReadDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MotoReadDto>> GetById(int id, CancellationToken ct = default)
        {
            var moto = await _repo.GetByIdAsync(id, ct);
            if (moto is null) return NotFound();
            return Ok(ToResponse(moto));
        }

        
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<MotoReadDto>), 200)]
        public async Task<ActionResult<IEnumerable<MotoReadDto>>> Search([FromQuery] string? placa, CancellationToken ct = default)
        {
            var motos = await _repo.SearchByPlacaAsync(placa ?? string.Empty, ct);
            return Ok(motos.Select(ToResponse));
        }

      
        [HttpPost]
        [ProducesResponseType(typeof(MotoReadDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<MotoReadDto>> Create([FromBody] MotoCreateDto request, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = new Moto
            {
                Placa = (request.Placa ?? string.Empty).Trim().ToUpperInvariant(),
                Chassi = string.IsNullOrWhiteSpace(request.Chassi) ? null : request.Chassi.Trim().ToUpperInvariant(),
                Status = (StatusMoto)request.Status,
                EmPatio = 0 
            };

            try
            {
                var id = await _repo.CreateAsync(entity, ct);
                var response = ToResponse(entity);
                return CreatedAtAction(nameof(GetById), new { id }, response);
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Code == 11000) 
            {
                return BadRequest("Placa já cadastrada.");
            }
        }

        
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] MotoUpdateDto request, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var moto = await _repo.GetByIdAsync(id, ct);
            if (moto is null) return NotFound();

            moto.Placa = (request.Placa ?? string.Empty).Trim().ToUpperInvariant();
            moto.Chassi = string.IsNullOrWhiteSpace(request.Chassi) ? null : request.Chassi.Trim().ToUpperInvariant();
            moto.Status = (StatusMoto)request.Status;
            

            try
            {
                var ok = await _repo.UpdateAsync(moto, ct);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Code == 11000) 
            {
                return BadRequest("Placa já cadastrada.");
            }
        }

        
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