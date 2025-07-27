using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WfConFin.Models;

public class Conta
{
    [Key]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O campo Descrição é obrigatorio!!!")]
    [StringLength(200, ErrorMessage = "A descrição deve ter até 200 caracteres")]
    public string Descricao { get; set; }

    [Required(ErrorMessage = "O campo Valor é obrigatorio!!!")]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "O campo Data de Vencimento é obrigatorio!!!")]
    [DataType(DataType.DateTime)]
    public DateTime DataVencimento { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime? DataPagamento { get; set; }

    [Required(ErrorMessage = "O campo Data de Situação é obrigatorio!!!")]
    public Situacao Situacao{ get; set; }

    //Gera ID automático e único
    public Conta()
    {
        Id = Guid.NewGuid();
    }

    //
    [Required(ErrorMessage = "O campo PessoaId é obrigatorio!!!")]
    public Guid PessoaId { get; set; }
    //Relacionamento Entity Framework
    public Pessoa Pessoa{ get; set; }
}