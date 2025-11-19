using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace QuitandaBitseBananas.Models
{
    public class Produto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Código chave")]
        public long Id { get; set; }

        [MaxLength(50)]
        [Display(Name = "Produto")]
        [Required(ErrorMessage = "Por favor, preencha o campo {0}.")]
        public string Name { get; set; }

        [ModelBinder(BinderType = typeof(DecimalModelBinder))] // Aplica o ModelBinder
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Preço")]
        [Required(ErrorMessage = "Por favor, preencha o campo {0}.")]
        [Range(0, double.MaxValue, ErrorMessage = "O preço deve ser maior ou igual a zero.")]
        public decimal Preco { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        [Display(Name = "Unidade de Medida")]
        [Required(ErrorMessage = "Por favor, selecione a {0}.")]
        public UnidadeMedida Unidade { get; set; }

        // Nova propriedade Quantidade
        [Display(Name = "Quantidade")]
        [Required(ErrorMessage = "Por favor, preencha o campo {0}.")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade deve ser maior ou igual a zero.")]
        public int Quantidade { get; set; }

        // Propriedade ValorTotal
        [Display(Name = "Valor Total")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ValorTotal { get; set; } // Novo campo para armazenar o valor total (Preço * Quantidade)
    }

    public class Categoria
    { 
        public int Id { get; set; }
        public string Nome { get; set; }
    }

    public class Fornecedor
    {
        public int Id { get; set; }
        public string nomeFornecedor { get; set; }
    }

    public class Estoque
    {
        public int Quantidade { get; set; }
        public Produto Produto { get; set; }
    }

    public enum UnidadeMedida
    {
        [Display(Name = "Quilograma")]
        Kg = 1,

        [Display(Name = "Unidade")]
        Unidade = 2
    }
}
