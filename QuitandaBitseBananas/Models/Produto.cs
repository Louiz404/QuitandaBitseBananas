using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace QuitandaBitseBananas.Models
{
    public enum UnidadeMedida
    {
        [Display(Name = "Quilograma")]
        Kg = 1,

        [Display(Name = "Unidade")]
        Unidade = 2
    }
    public class Produto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Código chave")]
        public int Id { get; set; }

        // Nome do Produto
        [MaxLength(50)]
        [Display(Name = "Produto")]
        [Required(ErrorMessage = "Por favor, preencha o campo {0}.")]
        public string Name { get; set; }

        // Preço
        [ModelBinder(BinderType = typeof(DecimalModelBinder))] // Aplica o ModelBinder
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Preço")]
        [Required(ErrorMessage = "Por favor, preencha o campo {0}.")]
        [Range(0, double.MaxValue, ErrorMessage = "O preço deve ser maior ou igual a zero.")]
        public decimal Preco { get; set; }

        // Unidade de Medida
        [Display(Name = "Unidade de Medida")]
        [Required(ErrorMessage = "Por favor, selecione a {0}.")]
        public UnidadeMedida Unidade { get; set; }

        // Quantidade
        [Display(Name = "Quantidade em estoque")]
        [Required(ErrorMessage = "Por favor, preencha o campo {0}.")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade deve ser maior ou igual a zero.")]
        public int Quantidade { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data de Validade")]
        public DateTime DataValidade { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data de Entrada")]
        public DateTime DataEntrada { get; set; }

        // ValorTotal
        [Display(Name = "Valor Total")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ValorTotal { get; set; } // Novo campo para armazenar o valor total (Preço * Quantidade)

        // RELACIONAMENTOS
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }
       
        public int FornecedorId { get; set; }
        public Fornecedor? Fornecedor { get; set; }  
        
    }

    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Nome da Categoria")]
        [Required(ErrorMessage = "O nome da categoria é obrigatório")]
        public string Name { get; set; }
        public List<Produto> Produtos { get; set; }
    }

    public class Fornecedor
    {
        [Key]
        public int Id { get; set; }
        
        [Display(Name = "Nome do Fornecedor")]
        [Required(ErrorMessage = "O nome do fornecedor é obrigatório.")]
        public string Name { get; set; }
       
        public List<Produto> Produtos { get; set; }
    }
    
    
}

