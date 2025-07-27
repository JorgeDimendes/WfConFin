using Microsoft.AspNetCore.Mvc;
using WfConFin.Models;

namespace WfConFin.Controllers
{
    // [Route("home2")]
    [Route("[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private static List<Estado> listaEstados = new List<Estado>();
        
        [HttpGet("Estado")]
        public IActionResult GetEstados()
        {
            return Ok(listaEstados);
        }
        
        [HttpPost("Estado")]
        public IActionResult PostEstados([FromBody] Estado estado)
        {
            listaEstados.Add(estado);
            return Ok("Estado cadastrado com sucesso");
        }
        
        
        
        //-----------
        #region Exemplos HTTP
        [HttpGet]
        public IActionResult GetInformacao()
        {
            var result = "Retorno em texto agora";
            return Ok(result);
        }

        [HttpGet("info2")]
        public IActionResult GetInformacao2()
        {
            var result = "Retorno em texto agora 2";
            return Ok(result);
        }
        
        [HttpGet("info3/{valor}")] //Parametro que recebe informação 
        // public IActionResult GetInformacao3(string valor) 
        public IActionResult GetInformacao3([FromRoute] string valor) //Ou pode especificar de onde vem esse valor
        {
            var result = $"Retorno em texto agora 3 - Valor: {valor}";
            return Ok(result);
        }
        
        [HttpPost("info4")] //Parametro que recebe informação 
        public IActionResult GetInformacao4(string valor) 
        {
            
            var result = $"Retorno em texto agora 4 - Valor: {valor}";
            return Ok(result);
        }
        
        [HttpPost("info5")] //Parametro que recebe informação 
        public IActionResult GetInformacao5([FromHeader] string valor) 
        {
            
            var result = $"Retorno em texto agora 5 - Valor: {valor}";
            return Ok(result);
        }
        
        [HttpPost("info6")] //Parametro que recebe informação 
        public IActionResult GetInformacao6([FromBody] Corpo corpo) 
        {
            
            var result = $"Retorno em texto agora 6 - Valor: {corpo.valor}";
            return Ok(result);
        }
        #endregion
    }

    public class Corpo
    {
        public string valor { get; set; }
    }
}