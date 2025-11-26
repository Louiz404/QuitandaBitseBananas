using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuitandaBitseBananas.Models
{
    public enum TipoMovimentacao
    {
        [Display(Name = "Entrada")]
        Entrada = 1,

        [Display(Name = "Saída")]
        Saida = 2
    }

    public enum MotivoMovimentacao
    {
        [Display(Name = "Venda ao cliente")]
        Venda = 1,

        [Display(Name = "Compra do fornecedor")]
        Compra = 2,

        [Display(Name = "Perda por validade")]
        PerdaValidade,

        [Display(Name = "Perda por dano ou outro")]
        PerdaMercadoria,

        [Display(Name = "Ajuste de estoque")]
        AjusteEstoque
    }
    public class Movimentacao
    {
        [Key]
        public int Id { get; set; }
        public DateTime DataMovimentacao { get; set; } = DateTime.Now;
        public int Quantidade { get; set; }
        public TipoMovimentacao Tipo { get; set; }
        public MotivoMovimentacao Motivo { get; set; }
        
        [MaxLength(200)]
        public string? Observacoes { get; set; }

        // Relacionamentos
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }

        [Display(Name = "Usuário responsável")]
        public string? UsuarioId { get; set; }



    }
}
