using AutoMapper;
using HamburguerDev.Api.ViewModels;
using HamburguerDev.Business.Models;
using System.Linq;

namespace HamburguerDev.Api.Mappings;

public class PedidoMappingProfile : Profile
{
    public PedidoMappingProfile()
    {
        CreateMap<Produto, ProdutoResponseModel>();
        CreateMap<PedidoProduto, PedidoProdutoResponseModel>();

        CreateMap<Pedido, PedidoResponseModel>()
            .ForMember(dest => dest.Produtos, opt => opt.MapFrom(src =>
                string.Join(", ",
                    src.PedidoProdutos!
                        .Where(pp => pp.Produto != null)
                        .Select(pp => pp.Produto.Nome))));
        CreateMap<Pedido, PedidoDetalheResponseModel>();
    }
}