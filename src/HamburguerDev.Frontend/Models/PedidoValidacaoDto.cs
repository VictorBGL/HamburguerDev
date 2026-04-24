namespace HamburguerDev.Frontend.Models;

public class PedidoValidacaoDto
{
    public decimal Total { get; set; }
    public decimal Subtotal { get; set; }
    public decimal? DescontoPorcentagem { get; set; }
}
