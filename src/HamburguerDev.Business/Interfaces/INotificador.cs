using HamburguerDev.Business.Notificacoes;

namespace HamburguerDev.Business.Interfaces;

public interface INotificador
{
    bool TemNotificacao();
    List<Notificacao> ObterNotificacoes();
    void Handle(Notificacao notificacao);
}