using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WfConFin.Data;
using WfConFin.Models;

namespace WfConFin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PessoaController : ControllerBase
    {
        private readonly WfConFinDbContext _context;

        public PessoaController(WfConFinDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPessoa()
        {
            try
            {
                var result = await _context.Pessoa.ToListAsync(); // Método assíncrono
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na listagem de pessoas. Exceção: {ex.Message}");
            }
            
        }

        [HttpPost]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<ActionResult> PostPessoa([FromBody] Pessoa pessoa)
        {
            try
            {
                await _context.Pessoa.AddAsync(pessoa);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1) //valor é igual a inteiro onde: [ 0 = erro | 1 = sucesso ]
                {
                    return Ok("Sucesso, pessoa cadastrada.");
                }
                else
                {
                    return BadRequest("Erro, pessoa não cadastrada.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro no cadastro de pessoa. Exceção - {ex.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<ActionResult> PutPessoa([FromBody] Pessoa pessoa)
        {
            try
            {
                _context.Pessoa.Update(pessoa);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1) //valor é igual a inteiro onde: [ 0 = erro | 1 = sucesso ]
                {
                    return Ok("Sucesso, pessoa alterada.");
                }
                else
                {
                    return BadRequest("Erro, pessoa não alterada.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na alteração de pessoa. Exceção - {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<ActionResult> DeletePessoa([FromRoute] Guid id)
        {
            try
            {
                Pessoa pessoa = await _context.Pessoa.FindAsync(id);

                if (pessoa != null)
                {
                    _context.Pessoa.Remove(pessoa);

                    var valor = await _context.SaveChangesAsync();
                    if (valor == 1) //valor é igual a inteiro onde: [ 0 = erro | 1 = sucesso ]
                    {
                        return Ok("Sucesso, pessoa excluia.");
                    }
                    else
                    {
                        return BadRequest("Erro, pessoa não excluia.");
                    }
                }
                else
                {
                    return NotFound($"Erro, pessoa não existe!!!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na exclusão de pessoa. Exceção - {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetPessoa([FromRoute] Guid id)
        {
            try
            {
                Pessoa pessoa = await _context.Pessoa.FindAsync(id);
                if (pessoa != null)
                {
                    return Ok(pessoa);
                }
                else
                {
                    return NotFound($"Erro, pessoa não existe!!!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na pesquisa de pessoa. Exceção - {ex.Message}");
            }
        }

        [HttpGet("Pesquisa")]
        public async Task<ActionResult> GetPessoaPesquisa([FromQuery] string valor)
        {
            try
            {
                //Query Criterio
                var lista = from o in _context.Pessoa.ToList()
                            where o.Nome.ToUpper().Contains(valor.ToUpper())
                            || o.Telefone.ToUpper().Contains(valor.ToUpper())
                            || o.Email.ToUpper().Contains(valor.ToUpper())
                            select o;

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, pesquisa de pessoa não encontrado. Exceção - {ex.Message}");
            }
        }

        [HttpGet("Paginacao")]
        public async Task<ActionResult> GetPessoaPaginacao([FromQuery] string? valor, int skip, int take, bool ordemDesc)
        {
            try
            {
                //Query Criterio
                var lista = from o in _context.Pessoa.ToList()
                            select o;

                if (!String.IsNullOrEmpty(valor))
                {
                    lista = from o in lista
                            where o.Nome.ToUpper().Contains(valor.ToUpper())
                            || o.Telefone.ToUpper().Contains(valor.ToUpper())
                            || o.Email.ToUpper().Contains(valor.ToUpper())
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

                var paginacaoResponse = new PaginacaoResponse<Pessoa>(lista, qtde, skip, take);
                return Ok(paginacaoResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, pesquisa de pessoa não encontrado. Exceção - {ex.Message}");
            }
        }
    }
}
