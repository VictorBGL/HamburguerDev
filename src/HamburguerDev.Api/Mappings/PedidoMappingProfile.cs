using AutoMapper;
using HamburguerDev.Api.ViewModels;
using HamburguerDev.Business.Models;

namespace HamburguerDev.Api.Mappings;

public class PedidoMappingProfile : Profile
{
    public PedidoMappingProfile()
    {
        CreateMap<Produto, ProdutoResponseModel>();
        CreateMap<PedidoProduto, PedidoProdutoResponseModel>();

        CreateMap<Pedido, PedidoResponseModel>();
        CreateMap<Pedido, PedidoDetalheResponseModel>();
    }
}