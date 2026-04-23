
namespace HamburguerDev.Business.Models
{
    public class Produto : Entity
    {
        private Produto()
        {
            Nome = string.Empty;
        }

        public Produto(string nome, decimal preco, int codigo, bool acompanhamento)
        {
            Nome = nome;
            Preco = preco;
            Codigo = codigo;
            Acompanhamento = acompanhamento;
        }

        public string Nome { get; private set; }
        public decimal Preco { get; private set; }
        public int Codigo { get; private set; }
        public bool Acompanhamento { get; private set; }

        public virtual ICollection<PedidoProduto>? PedidosProduto { get; private set; }
    }
}
