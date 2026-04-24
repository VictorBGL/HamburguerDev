namespace HamburguerDev.Frontend.Models;

public class ProdutoResponseModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Codigo { get; set; }
    public bool Acompanhamento { get; set; }
}
