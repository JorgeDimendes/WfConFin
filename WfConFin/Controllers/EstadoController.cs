using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WfConFin.Data;
using WfConFin.Models;

namespace WfConFin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EstadoController : ControllerBase
    {
        private readonly WfConFinDbContext _context;

        public EstadoController(WfConFinDbContext context)
        {
            _context = context;
        }

        // ---> CRUD <---

        [HttpGet]
        public async Task<ActionResult> GetEstados()
        {
            try
            {
                var result = _context.Estado.ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na listagem de estados. Exceção - {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<ActionResult> PostEstados([FromBody] Estado estado)
        {
            try
            {
                await _context.Estado.AddAsync(estado);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1) //valor é igual a inteiro onde: [ 0 = erro | 1 = sucesso ]
                {
                    return Ok("Sucesso, estado incluido.");
                }
                else
                {
                    return BadRequest("Erro, estado não incluido.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, estado não incluido. Exceção - {ex.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<ActionResult> PutEstados([FromBody] Estado estado)
        {
            try
            {
                _context.Estado.Update(estado);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1) //valor é igual a inteiro onde: [ 0 = erro | 1 = sucesso ]
                {
                    return Ok("Sucesso, estado alerado.");
                }
                else
                {
                    return BadRequest("Erro, estado não alerado.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, estado não alerado. Exceção - {ex.Message}");
            }
        }

        [HttpDelete("{sigla}")]
        [Authorize(Roles = "Gerente")]
        public async Task<ActionResult> DeleteEstados([FromRoute] string sigla)
        {
            try
            {
                var estado = await _context.Estado.FindAsync(sigla);

                if(estado.Sigla == sigla && !string.IsNullOrEmpty(estado.Sigla))
                {
                    _context.Estado.Remove(estado);
                    var valor = await _context.SaveChangesAsync();
                    if (valor == 1) //valor é igual a inteiro onde: [ 0 = erro | 1 = sucesso ]
                    {
                        return Ok("Sucesso, estado excluido.");
                    }
                    else
                    {
                        return BadRequest("Erro, estado não excluido.");
                    }
                }
                else
                {
                    return NotFound("Erro, estado não existe");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, estado não deletado. Exceção - {ex.Message}");
            }
        }

        [HttpGet("{sigla}")]
        public async Task<ActionResult> GetEstado([FromRoute] string sigla)
        {
            try
            {
                var estado = await _context.Estado.FindAsync(sigla);

                if (estado.Sigla == sigla && !string.IsNullOrEmpty(estado.Sigla))
                {
                    return Ok(estado);
                }
                else
                {
                    return NotFound("Erro, estado não existe");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, estado não encontrado. Exceção - {ex.Message}");
            }
        }

        [HttpGet("Pesquisa")]
        public async Task<ActionResult> GetEstadoPesquisa([FromQuery] string valor)
        {
            try
            {
                //Query Criterio
                var lista = from o in _context.Estado.ToList()
                            where o.Sigla.ToUpper().Contains(valor.ToUpper())
                            || o.Nome.ToUpper().Contains(valor.ToUpper())
                            select o; 
                
                #region Alternativas

                //ou Entity
                /* lista = _context.Estado
                    .Where(o => o.Sigla.ToUpper().Contains(valor.ToUpper())
                             || o.Nome.ToUpper().Contains(valor.ToUpper())
                    )
                    .ToList();*/

                // ou plugin Expression
                /*
                Expression <Func<Estado, bool>> expressao = o => true;
                expressao = o => o.Sigla.ToUpper().Contains(valor.ToUpper())
                              || o.Nome.ToUpper().Contains(valor.ToUpper());
                lista = _context.Estado.Where(expressao).ToList();
                */
                #endregion

                return Ok(lista);

                // upper - entende Maiusculo e menusculo
                //SELECT * FROM WHERE upper(sigla) like upper('%valor%') or upper(nome) like upper('%valor%')
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, pesquisa de estado não encontrado. Exceção - {ex.Message}");
            }
        }

        [HttpGet("Paginacao")]
        public async Task<ActionResult> GetEstadoPaginacao([FromQuery] string? valor, int skip, int take, bool ordemDesc)
        {
            try
            {
                //Query Criterio
                var lista = from o in _context.Estado.ToList()
                            select o;

                if (!String.IsNullOrEmpty(valor))
                {
                    lista = from o in lista
                                where o.Sigla.ToUpper().Contains(valor.ToUpper())
                            || o.Nome.ToUpper().Contains(valor.ToUpper())
                            select o;
                }

                if (ordemDesc)
                {
                    lista = from o in lista
                        orderby o.Nome descending
                        select o;
                }
                else
                {
                    lista = from o in lista
                        orderby o.Nome ascending
                        select o;
                }
                
                var qtde = lista.Count();

                lista = lista
                        .Skip((skip - 1) * take)
                        .Take(take)
                        .ToList();

                var paginacaoResponse = new PaginacaoResponse<Estado>(lista, qtde, skip, take);
                return Ok(paginacaoResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, pesquisa de estado não encontrado. Exceção - {ex.Message}");
            }
        }
    }
}
