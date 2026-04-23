using AutoMapper;
using HamburguerDev.Api.ViewModels;
using HamburguerDev.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HamburguerDev.Api.Controllers;

[Route("api/produto")]
public class ProdutoController : MainController
{
    private readonly IProdutoService _service;

    public ProdutoController(
        INotificador notificador,
        IMapper mapper,
        IUser appUser,
        IProdutoService service)
        : base(notificador, mapper, appUser)
    {
        _service = service;
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(PedidoDetalheResponseModel), 200)]
    public async Task<IActionResult> Get([FromQuery] int? codigo, string? nome)
    {
        return CustomResponse(_mapper.Map<IEnumerable<ProdutoResponseModel>>(await _service.Buscar(codigo, nome)));
    }
}
