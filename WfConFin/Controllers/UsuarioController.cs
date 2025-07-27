using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WfConFin.Data;
using WfConFin.Models;
using WfConFin.Services;

namespace WfConFin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly WfConFinDbContext _context;
        private readonly TokenService _Service;


        public UsuarioController(WfConFinDbContext context, TokenService service)
        {
            _context = context;
            _Service = service;
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous] //
        public async Task<ActionResult> Login([FromBody] UsuarioLogin usuarioLogin)
        {
            var usuario = _context.Usuario.Where(x => x.Login == usuarioLogin.Login).FirstOrDefault();
            if (usuario == null) 
            {
               return NotFound("Usuario invalido..."); 
            }

            //
            var passwordHash = MD5Hash.CalcHash(usuarioLogin.Senha);
            //

            if (usuario.Senha != passwordHash)
            {
                return BadRequest("Senha Invalida");
            }

            var token = _Service.GerarToken(usuario);
            usuario.Senha = "";

            var result = new UsuarioResponse()
            {
                Usuario = usuario,
                Token = token
            };

            return Ok(result);

        }

        [HttpGet]
        public async Task<ActionResult> GetUsuario()
        {
            try
            {
                var result = await _context.Usuario.ToListAsync();
                return Ok(result);
            }
            catch (Exception Ex)
            {
                return BadRequest($"Erro, {Ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<ActionResult> PostUsuario([FromBody] Usuario usuario)
        {
            try
            {
                // Verificar se ja tem usuaio com login
                var listUsuario = _context.Usuario.Where(x => x.Login == usuario.Login).ToList();
                if(listUsuario.Count > 0)
                {
                    return BadRequest($"Erro, informação de login invalido");
                }

                //
                string passwordHash = MD5Hash.CalcHash(usuario.Senha);
                usuario.Senha = passwordHash;
                //

                await _context.Usuario.AddAsync(usuario);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1) //valor é igual a inteiro onde: [ 0 = erro | 1 = sucesso ]
                {
                    return Ok("Sucesso, usuario cadastrada.");
                }
                else
                {
                    return BadRequest("Erro, usuario não cadastrada.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro no cadastro de usuario. Exceção - {ex.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<ActionResult> PutUsuario([FromBody] Usuario usuario)
        {
            try
            {
                //
                string passwordHash = MD5Hash.CalcHash(usuario.Senha);
                usuario.Senha = passwordHash;
                //
                _context.Usuario.Update(usuario);
                var valor = await _context.SaveChangesAsync();
                if (valor == 1) //valor é igual a inteiro onde: [ 0 = erro | 1 = sucesso ]
                {
                    return Ok("Sucesso, usuario alterada.");
                }
                else
                {
                    return BadRequest("Erro, usuario não alterada.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na alteração do usuario. Exceção - {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<ActionResult> DeleteUsuario([FromRoute] Guid id)
        {
            try
            {
                Usuario usuario = await _context.Usuario.FindAsync(id);

                if (usuario != null)
                {
                    _context.Usuario.Remove(usuario);

                    var valor = await _context.SaveChangesAsync();
                    if (valor == 1) //valor é igual a inteiro onde: [ 0 = erro | 1 = sucesso ]
                    {
                        return Ok("Sucesso, usuario excluia.");
                    }
                    else
                    {
                        return BadRequest("Erro, usuario não excluia.");
                    }
                }
                else
                {
                    return NotFound($"Erro, usuario não existe!!!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na exclusão de usuario. Exceção - {ex.Message}");
            }
        }
    }
}
