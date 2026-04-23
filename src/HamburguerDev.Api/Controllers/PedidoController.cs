using AutoMapper;
using HamburguerDev.Api.ViewModels;
using HamburguerDev.Business.Interfaces;
using HamburguerDev.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HamburguerDev.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/pedido")]
    public class PedidoController : MainController
    {
        private readonly IPedidoService _service;

        public PedidoController(
            INotificador notificador,
            IMapper mapper,
            IUser appUser,
            IPedidoService service)
            : base(notificador, mapper, appUser)
        {
            _service = service;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<PedidoResponseModel>), 200)]
        public async Task<IActionResult> Get([FromQuery] int? codigo)
        {
            var pedidos = await _service.Buscar(codigo);
            var response = _mapper.Map<IEnumerable<PedidoResponseModel>>(pedidos);
            return CustomResponse(response);
        }

        [HttpGet("{id:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PedidoDetalheResponseModel), 200)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var pedido = await _service.BuscarPorId(id);
            if (pedido is null)
            {
                NotificarErro("Pedido n�o encontrado.");
                return CustomResponse();
            }

            var response = _mapper.Map<PedidoDetalheResponseModel>(pedido);
            return CustomResponse(response);
        }

        [HttpPost("validar")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PedidoValidacaoDTO), 200)]
        public async Task<IActionResult> ValidarPedido([FromBody] PedidoValidacaoModel model)
        {
            return CustomResponse(await _service.ValidarPedido(model.ProdutosId));
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PedidoDetalheResponseModel), 200)]
        public async Task<IActionResult> Insert([FromBody] PedidoValidacaoModel model)
        {
            var pedido = await _service.InserirPedido(model.ProdutosId);

            if (!OperacaoValida())
                return CustomResponse();

            if(pedido is null)
            {
                NotificarErro("Erro ao criar pedido.");
                return CustomResponse();
            }

            return CustomResponse(_mapper.Map<PedidoDetalheResponseModel>(pedido));
        }

        [HttpPut("{id:guid}/finalizar")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PedidoDetalheResponseModel), 200)]
        public async Task<IActionResult> Finalizar([FromRoute] Guid id)
        {
            var pedido = await _service.FinalizarPedido(id);

            if (!OperacaoValida())
                return CustomResponse();

            if (pedido is null)
            {
                NotificarErro("Erro ao finalizar pedido.");
                return CustomResponse();
            }

            return CustomResponse(_mapper.Map<PedidoDetalheResponseModel>(pedido));
        }

        [HttpDelete("{id:guid}")]
        [Produces("application/json")]
        public async Task<IActionResult> Excluir([FromRoute] Guid id)
        {
            var pedidoExcluido = await _service.ExcluirPedido(id);

            return CustomResponse();
        }
    }
}
