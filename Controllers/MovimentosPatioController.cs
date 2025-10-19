using Loggu.Application.DTOs;
using Loggu.Domain.Entity;
using Loggu.Domain.Enums;
using Loggu.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Loggu.Controllers
{
    [ApiController]
    [Route("api/movimentos")]
    public class MovimentosPatioController : ControllerBase
    {
        private readonly IMovimentoPatioRepository _movRepo;
        private readonly IMotoRepository _motoRepo;

        public MovimentosPatioController(
            IMovimentoPatioRepository movRepo,
            IMotoRepository motoRepo)
        {
            _movRepo = movRepo;
            _motoRepo = motoRepo;
        }


        private static DateTime EnsureUtc(DateTime dt)
        {
            if (dt == default) return DateTime.UtcNow;
            if (dt.Kind == DateTimeKind.Utc) return dt;
            if (dt.Kind == DateTimeKind.Local) return dt.ToUniversalTime();
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        }


        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Registrar([FromBody] MovimentoPatioCreateDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

   
            var moto = await _motoRepo.GetByIdAsync(dto.MotoId, ct);
            if (moto is null) return BadRequest("Moto não encontrada.");

          
            if (dto.Tipo != (int)TipoMovimento.ENTRADA && dto.Tipo != (int)TipoMovimento.SAIDA)
                return BadRequest("Tipo inválido (0=ENTRADA, 1=SAIDA).");

           
            if (dto.Tipo == (int)TipoMovimento.ENTRADA && moto.EmPatio == 1)
                return BadRequest("Moto já está no pátio.");
            if (dto.Tipo == (int)TipoMovimento.SAIDA && moto.EmPatio == 0)
                return BadRequest("Moto já está fora do pátio.");

         
            var mov = new MovimentoPatio
            {
                MotoId = dto.MotoId,
                RealizadoPorUsuarioId = dto.RealizadoPorUsuarioId,
                Tipo = (TipoMovimento)dto.Tipo,
                Quando = EnsureUtc(dto.Quando),
                Observacao = dto.Observacao
            };

            
            var movId = await _movRepo.CreateAsync(mov, ct);

            moto.EmPatio = (mov.Tipo == TipoMovimento.ENTRADA) ? 1 : 0;
            var ok = await _motoRepo.UpdateAsync(moto, ct);
            if (!ok) return BadRequest("Não foi possível atualizar o estado da moto.");

            return CreatedAtAction(nameof(ListarPorMoto), new { motoId = dto.MotoId }, null);
        }

        
        [HttpGet("moto/{motoId:int}")]
        [ProducesResponseType(typeof(IEnumerable<MovimentoPatio>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ListarPorMoto(
            int motoId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            if (page < 1 || pageSize < 1) return BadRequest("page/pageSize inválidos.");

            var items = await _movRepo.GetAsync(
                motoId: motoId,
                realizadoPorUsuarioId: null,
                tipo: null,
                de: null,
                ate: null,
                page: page,
                pageSize: pageSize,
                ct: ct);

            return Ok(items);
        }
    }
}
