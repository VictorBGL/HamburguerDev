using HamburguerDev.Frontend.Models;
using System.Net.Http.Json;

namespace HamburguerDev.Frontend.Services;

public class PedidoApiService : IPedidoApiService
{
    private readonly HttpClient _httpClient;

    public PedidoApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(IReadOnlyList<PedidoResponseModel> Pedidos, IReadOnlyList<string> Errors)> BuscarPedidosAsync(int? codigo, CancellationToken cancellationToken = default)
    {
        var query = codigo.HasValue ? $"?codigo={codigo.Value}" : string.Empty;
        var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<PedidoResponseModel>>>($"api/pedido{query}", cancellationToken);

        if (response is null)
        {
            return ([], ["Nao foi possivel obter resposta da API."]);
        }

        if (!response.Success)
        {
            return ([], response.Errors?.ToList() ?? ["Falha ao buscar pedidos."]);
        }

        return (response.Data ?? [], []);
    }

    public async Task<(PedidoDetalheResponseModel? Pedido, IReadOnlyList<string> Errors)> BuscarPedidoPorIdAsync(Guid pedidoId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<ApiResponse<PedidoDetalheResponseModel>>($"api/pedido/{pedidoId}", cancellationToken);

        if (response is null)
        {
            return (null, ["Nao foi possivel obter o pedido."]);
        }

        if (!response.Success)
        {
            return (null, response.Errors?.ToList() ?? ["Falha ao buscar pedido."]);
        }

        return (response.Data, []);
    }

    public async Task<(IReadOnlyList<ProdutoResponseModel> Produtos, IReadOnlyList<string> Errors)> BuscarProdutosAsync(string? nome = null, int? codigo = null, CancellationToken cancellationToken = default)
    {
        var queryParts = new List<string>();

        if (codigo.HasValue)
        {
            queryParts.Add($"codigo={codigo.Value}");
        }

        if (!string.IsNullOrWhiteSpace(nome))
        {
            queryParts.Add($"nome={Uri.EscapeDataString(nome)}");
        }

        var query = queryParts.Count > 0
            ? $"?{string.Join("&", queryParts)}"
            : string.Empty;

        var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<ProdutoResponseModel>>>($"api/produto{query}", cancellationToken);

        if (response is null)
        {
            return ([], ["Nao foi possivel obter produtos."]);
        }

        if (!response.Success)
        {
            return ([], response.Errors?.ToList() ?? ["Falha ao buscar produtos."]);
        }

        return (response.Data ?? [], []);
    }

    public async Task<(PedidoValidacaoDto? Validacao, IReadOnlyList<string> Errors)> ValidarPedidoAsync(IEnumerable<Guid> produtosId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/pedido/validar", new
        {
            ProdutosId = produtosId
        }, cancellationToken);

        ApiResponse<PedidoValidacaoDto>? payload = null;

        try
        {
            payload = await response.Content.ReadFromJsonAsync<ApiResponse<PedidoValidacaoDto>>(cancellationToken);
        }
        catch
        {
            payload = null;
        }

        if (response.IsSuccessStatusCode && payload?.Success == true)
        {
            return (payload.Data, []);
        }

        if (payload?.Errors?.Any() == true)
        {
            return (null, payload.Errors.ToList());
        }

        return (null, ["Falha ao validar pedido."]);
    }

    public async Task<IReadOnlyList<string>> InserirPedidoAsync(IEnumerable<Guid> produtosId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/pedido", new
        {
            ProdutosId = produtosId
        }, cancellationToken);

        return await ExtrairErrosAsync(response, "Falha ao salvar pedido.", cancellationToken);
    }

    public async Task<IReadOnlyList<string>> AtualizarPedidoAsync(Guid pedidoId, IEnumerable<Guid> produtosId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/pedido/{pedidoId}", new
        {
            ProdutosId = produtosId
        }, cancellationToken);

        return await ExtrairErrosAsync(response, "Falha ao atualizar pedido.", cancellationToken);
    }

    public async Task<IReadOnlyList<string>> FinalizarPedidoAsync(Guid pedidoId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsync($"api/pedido/{pedidoId}/finalizar", content: null, cancellationToken);
        return await ExtrairErrosAsync(response, "Falha ao finalizar pedido.", cancellationToken);
    }

    public async Task<IReadOnlyList<string>> ExcluirPedidoAsync(Guid pedidoId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"api/pedido/{pedidoId}", cancellationToken);
        return await ExtrairErrosAsync(response, "Falha ao excluir pedido.", cancellationToken);
    }

    private static async Task<IReadOnlyList<string>> ExtrairErrosAsync(HttpResponseMessage response, string fallbackMessage, CancellationToken cancellationToken)
    {
        ApiResponse<object>? payload = null;

        try
        {
            payload = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(cancellationToken);
        }
        catch
        {
            payload = null;
        }

        if (response.IsSuccessStatusCode && (payload is null || payload.Success))
        {
            return [];
        }

        if (payload?.Errors?.Any() == true)
        {
            return payload.Errors.ToList();
        }

        return [fallbackMessage];
    }
}
