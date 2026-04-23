using HamburguerDev.Business.Interfaces;

namespace HamburguerDev.Business.Notificacoes;

public class Notificador : INotificador
{
    private readonly List<Notificacao> _notificacoes = [];

    public void Handle(Notificacao notificacao)
    {
        _notificacoes.Add(notificacao);
    }

    public List<Notificacao> ObterNotificacoes()
    {
        return _notificacoes;
    }

    public bool TemNotificacao()
    {
        return _notificacoes.Any();
    }
}