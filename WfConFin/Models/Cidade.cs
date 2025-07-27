using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WfConFin.Models;

public class Cidade
{
    [Key]
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "O campo nome é obrigatorio!!!")]
    [Display(Name = "Nome do Estado")] // Opcional - Nome amigável para exibição visual.
    [StringLength(200, MinimumLength = 3, ErrorMessage = "O campo nome deve ter entre 3 a 200 caracteres")]
    public string Nome { get; set; }
    
    //
    [Required(ErrorMessage = "O campo Estado é obrigatorio!!!")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "O campo estado deve ter 2 caracteres")]
    public string EstadoSigla { get; set; }

    public Cidade()
    {
        Id = Guid.NewGuid();
    }

    // Relacionamento Entity Framework
    [JsonIgnore]
    public Estado Estado { get; set; }
}