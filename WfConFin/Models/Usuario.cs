using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WfConFin.Models;

public class Usuario
{
    [Key]
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "O campo nome é obrigatorio!!!")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "O campo nome deve ter no maximo 200 caracteres!!!")]
    public string Nome { get; set; }
    
    [Required(ErrorMessage = "O campo Login é obrigatorio!!!")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "O campo Login deve ter entre 3 a 20 caracteres!!!")]
    public string Login { get; set; }
    
    [Required(ErrorMessage = "O campo Senha é obrigatorio!!!")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O campo Senha deve ter entre 3 a 20 caracteres!!!")]
    [DataType(DataType.Password)] // Para indicar que é uma senha
    public string Senha { get; set; }
    
    [Required(ErrorMessage = "O campo Funcao é obrigatorio!!!")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "O campo Funcao deve ter entre 3 a 20 caracteres!!!")]
    public string Funcao { get; set; }
}