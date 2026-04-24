
namespace HamburguerDev.Business.Models
{
    public class Pedido : Entity
    {
        private Pedido()
        {
            Status = string.Empty;
        }

        public Pedido(
            string status,
            int codigo,
            decimal total,
            decimal subtotal,
            decimal? descontoPorcentagem)
        {
            Status = status;
            Codigo = codigo;
            Total = total;
            Subtotal = subtotal;
            DescontoPorcentagem = descontoPorcentagem;
            DataCriacao = DateTime.Now;
        }

        public string Status { get; private set; }
        public int Codigo { get; private set; }
        public decimal Total { get; private set; }
        public decimal Subtotal { get; private set; }
        public decimal? DescontoPorcentagem { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataFinalizacao { get; private set; }

        public virtual ICollection<PedidoProduto>? PedidoProdutos { get; private set; }

        public void Finalizar()
        {
            Status = StatusPedidoEnum.FINALIZADO.ToString();
            DataFinalizacao = DateTime.Now;
        }

        public void AtualizarValores(decimal total, decimal subtotal, decimal? descontoPorcentagem)
        {
            Total = total;
            Subtotal = subtotal;
            DescontoPorcentagem = descontoPorcentagem;
        }
    }
}
