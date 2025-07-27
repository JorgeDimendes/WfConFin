using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WfConFin.Data;
using WfConFin.Models;

namespace WfConFin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CidadeController : ControllerBase
    {
        private readonly WfConFinDbContext _context;

        public CidadeController(WfConFinDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult> GetCidades()
        {
            try
            {
                //var result = _context.Cidade.Include(x => x.Estado).ToList(); //Adiciona o UF e nome do estado
                var result = _context.Cidade.ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na listagem de cidades. Exceção - {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<ActionResult> PostCidade([FromBody] Cidade cidade )
        {
            try
            {
                await _context.Cidade.AddAsync(cidade);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1) //valor é igual a inteiro onde: [ 0 = erro | 1 = sucesso ]
                {
                    return Ok("Sucesso, cidade cadastrada.");
                }
                else
                {
                    return BadRequest("Erro, cidade não incluido.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na inclusão de cidades. Exceção - {ex.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<ActionResult> PutCidade([FromBody] Cidade cidade)
        {
            try
            {
                _context.Cidade.Update(cidade);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1) //valor é igual a inteiro onde: [ 0 = erro | 1 = sucesso ]
                {
                    return Ok("Sucesso, cidade alterada.");
                }
                else
                {
                    return BadRequest("Erro, cidade não alterada.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na alteração de cidades. Exceção - {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<ActionResult> DeleteCidade([FromRoute] Guid id)
        {
            try
            {
                Cidade cidade = await _context.Cidade.FindAsync(id);
                if (cidade != null)
                {
                    _context.Cidade.Remove(cidade);

                    var valor = await _context.SaveChangesAsync();
                    if (valor == 1) //valor é igual a inteiro onde: [ 0 = erro | 1 = sucesso ]
                    {
                        return Ok("Sucesso, cidade excluia.");
                    }
                    else
                    {
                        return BadRequest("Erro, cidade não excluia.");
                    }
                }
                else
                {
                    return NotFound($"Erro, cidade não existe!!!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na exclusão de cidades. Exceção - {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetCidade([FromRoute] Guid id)
        {
            try
            {
                Cidade cidade = await _context.Cidade.FindAsync(id);
                if (cidade != null)
                {
                    return Ok(cidade);
                }
                else
                {
                    return NotFound($"Erro, cidade não existe!!!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na pesquisa de cidade. Exceção - {ex.Message}");
            }
        }

        [HttpGet("Pesquisa")]
        public async Task<ActionResult> GetCidadePesquisa([FromQuery] string valor)
        {
            try
            {
                //Query Criterio
                var lista = from o in _context.Cidade.ToList()
                            where o.Nome.ToUpper().Contains(valor.ToUpper())
                            || o.EstadoSigla.ToUpper().Contains(valor.ToUpper())
                            select o;

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, pesquisa de cidade não encontrado. Exceção - {ex.Message}");
            }
        }

        [HttpGet("Paginacao")]
        public async Task<ActionResult> GetCidadePaginacao([FromQuery] string? valor, int skip, int take, bool ordemDesc)
        {
            try
            {
                //Query Criterio
                var lista = from o in _context.Cidade.ToList()
                            select o;

                if (!String.IsNullOrEmpty(valor))
                {
                    lista = from o in lista
                            where o.Nome.ToUpper().Contains(valor.ToUpper())
                            || o.EstadoSigla.ToUpper().Contains(valor.ToUpper())
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

                var paginacaoResponse = new PaginacaoResponse<Cidade>(lista, qtde, skip, take);
                return Ok(paginacaoResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, pesquisa de cidade não encontrado. Exceção - {ex.Message}");
            }
        }
    }
}