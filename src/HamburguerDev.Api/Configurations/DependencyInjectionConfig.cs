using AutoMapper;
using HamburguerDev.Api.Extensions.Identity;
using HamburguerDev.Api.Mappings;
using HamburguerDev.Business.Interfaces;
using HamburguerDev.Business.Notificacoes;
using HamburguerDev.Business.Services;
using HamburguerDev.Data.Repositories;

namespace HamburguerDev.Api.Configurations;

public static class DependencyInjectionConfig
{
    public static WebApplicationBuilder ResolveDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
        builder.Services.AddScoped<IPedidoService, PedidoService>();
        builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
        builder.Services.AddScoped<IProdutoService, ProdutoService>();
        builder.Services.AddScoped<INotificador, Notificador>();
        builder.Services.AddScoped<IUser, AspNetUser>();

        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PedidoMappingProfile>();
        });
        builder.Services.AddSingleton(mapperConfiguration.CreateMapper());

        return builder;
    }
}
