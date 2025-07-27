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
    public class ContaController : Controller
    {
        private readonly WfConFinDbContext _context;

        public ContaController(WfConFinDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetConta()
        {
            try
            {
                var result = _context.Conta.Include(x => x.Pessoa).ToList(); //Adiciona Pessoa
                //var result = await _context.Conta.ToListAsync(); // Método assíncrono
                return Ok(result);
            }
            catch (Exception Ex)
            {
                BadRequest($"Erro, {Ex.Message}");
            }
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<ActionResult> PostConta([FromBody] Conta conta)
        {
            try
            {
                await _context.Conta.AddAsync(conta);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1) //valor é igual a inteiro onde: [ 0 = erro | 1 = sucesso ]
                {
                    return Ok("Sucesso, conta cadastrada.");
                }
                else
                {
                    return BadRequest("Erro, conta não cadastrada.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro no cadastro de conta. Exceção - {ex.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<ActionResult> PutConta([FromBody] Conta conta)
        {
            try
            {
                _context.Conta.Update(conta);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1) //valor é igual a inteiro onde: [ 0 = erro | 1 = sucesso ]
                {
                    return Ok("Sucesso, conta alterada.");
                }
                else
                {
                    return BadRequest("Erro, conta não alterada.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na alteração de conta. Exceção - {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<ActionResult> DeleteConta([FromRoute] Guid id)
        {
            try
            {
                Conta conta = await _context.Conta.FindAsync(id);

                if (conta != null)
                {
                    _context.Conta.Remove(conta);

                    var valor = await _context.SaveChangesAsync();
                    if (valor == 1) //valor é igual a inteiro onde: [ 0 = erro | 1 = sucesso ]
                    {
                        return Ok("Sucesso, conta excluia.");
                    }
                    else
                    {
                        return BadRequest("Erro, conta não excluia.");
                    }
                }
                else
                {
                    return NotFound($"Erro, conta não existe!!!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na exclusão de conta. Exceção - {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetConta([FromRoute] Guid id)
        {
            try
            {
                Conta conta = await _context.Conta.FindAsync(id);
                if (conta != null)
                {
                    return Ok(conta);
                }
                else
                {
                    return NotFound($"Erro, conta não existe!!!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na pesquisa de conta. Exceção - {ex.Message}");
            }
        }

        [HttpGet("Pesquisa")]
        public async Task<ActionResult> GetContaPesquisa([FromQuery] string valor)
        {
            try
            {
                //Query Criterio
                var lista = from o in _context.Conta.Include(o => o.Pessoa).ToList()
                            where o.Descricao.ToUpper().Contains(valor.ToUpper())
                            || o.Pessoa.Nome.ToUpper().Contains(valor.ToUpper())
                            select o;

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, pesquisa de conta não encontrado. Exceção - {ex.Message}");
            }
        }

        [HttpGet("Paginacao")]
        public async Task<ActionResult> GetContaPaginacao([FromQuery] string? valor, int skip, int take, bool ordemDesc)
        {
            try
            {
                //Query Criterio
                var lista = from o in _context.Conta.Include(o => o.Pessoa).ToList()
                            select o;

                if (!String.IsNullOrEmpty(valor)) { 
                    lista = from o in lista
                            where o.Descricao.ToUpper().Contains(valor.ToUpper())
                            || o.Pessoa.Nome.ToUpper().Contains(valor.ToUpper())
                            select o;
                }

                if (ordemDesc)
                {
                    lista = from o in lista
                            orderby o.Descricao descending
                            select o;
                }
                else
                {
                    lista = from o in lista
                            orderby o.Descricao ascending
                            select o;
                }

                var qtde = lista.Count();

                lista = lista
                        .Skip((skip - 1) * take)
                        .Take(take)
                        .ToList();

                var paginacaoResponse = new PaginacaoResponse<Conta>(lista, qtde, skip, take);
                return Ok(paginacaoResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, pesquisa de conta não encontrado. Exceção - {ex.Message}");
            }
        }

        [HttpGet("Pessoa/{pessoaId}")]
        public async Task<ActionResult> GetContasPessoas([FromRoute] Guid pessoaId)
        {
            try
            {
                //Query Criterio
                var lista = from o in _context.Conta.Include(o => o.Pessoa).ToList()
                            where o.PessoaId == pessoaId
                            select o;

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, pesquisa de conta por pessoa não encontrado. Exceção - {ex.Message}");
            }
        }
    }
}
