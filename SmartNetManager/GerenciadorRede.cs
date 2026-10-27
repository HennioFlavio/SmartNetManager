
using System;
using System.Collections.Generic;
using System.Linq;

public class GerenciadorRede
{
    // Missão 1: DHCP
    private Dictionary<string, string> reservasIP = new();

    // Missão 2: fila de autenticação
    private Queue<Dispositivo> filaAutenticacao = new();

    // Missão 3: dispositivos online
    private List<Dispositivo> dispositivosAtivos = new();

    // Missão 4: histórico de auditoria
    private Stack<string> historico = new();

    // Disponibiliza a fila e a lista para os relatórios.
    public IEnumerable<Dispositivo> Fila =>
        filaAutenticacao;

    public IEnumerable<Dispositivo> Ativos =>
        dispositivosAtivos;


    public void ReservarIP(string macAddress, string enderecoIP)
    {
        if (reservasIP.ContainsKey(macAddress))
        {
            Console.WriteLine("Erro: este MAC já possui uma reserva.");
            return;
        }

        reservasIP.Add(macAddress, enderecoIP);

        Console.WriteLine(
            $"IP {enderecoIP} reservado para o MAC {macAddress}.");
    }


    //Missão 1 — Reserva de IP (DHCP)
    public void AdicionarFila(Dispositivo dispositivo)
    {
        // Se houver reserva, utiliza o IP reservado.
        if (reservasIP.TryGetValue(
            dispositivo.MacAddress, out string? ipReservado))
        {
            dispositivo.EnderecoIP = ipReservado;
        }

        filaAutenticacao.Enqueue(dispositivo);

        Console.WriteLine(
            $"{dispositivo.Nome} adicionado à fila de autenticação.");
    }

    //Missão 2 — Fila de autenticação (QoS)
    public void ProcessarFila()
    {
        if (filaAutenticacao.Count == 0)
        {
            Console.WriteLine("A fila está vazia.");
            return;
        }

        Dispositivo dispositivo = filaAutenticacao.Dequeue();

        // Evita duplicar um aparelho que já está online.
        bool jaEstaAtivo = dispositivosAtivos.Any(
            d => d.MacAddress == dispositivo.MacAddress);

        if (jaEstaAtivo)
        {
            Console.WriteLine("Este dispositivo já está ativo.");
            return;
        }

        dispositivo.DataConexao = DateTime.Now;
        dispositivosAtivos.Add(dispositivo);

        historico.Push(
            $"Autenticado: {dispositivo.Nome} " +
            $"({dispositivo.MacAddress}) em {DateTime.Now:dd/MM/yyyy HH:mm:ss}");

        Console.WriteLine(
            $"Autenticação concluída: {dispositivo.Nome}");
    }


    // Missão 3 — Dashboard com LINQ
    public void ExibirDashboard()
    {
        Console.WriteLine("\n========== SMARTNET DASHBOARD ==========");

        double consumoTotal = dispositivosAtivos.Sum(
            d => d.ConsumoBanda);

        Console.WriteLine(
            $"Dispositivos ativos: {dispositivosAtivos.Count}");

        Console.WriteLine(
            $"Consumo total: {consumoTotal:F2} Mbps");

        Console.WriteLine("\n--- TOP 3 CONSUMIDORES ---");

        var top3 = dispositivosAtivos
            .OrderByDescending(d => d.ConsumoBanda)
            .Take(3);

        foreach (Dispositivo dispositivo in top3)
        {
            Console.WriteLine(
                $"{dispositivo.Nome}: {dispositivo.ConsumoBanda:F2} Mbps");
        }

        Console.WriteLine("\n--- DISPOSITIVOS POR FABRICANTE ---");

        var porFabricante = dispositivosAtivos
            .GroupBy(d => d.Fabricante)
            .OrderBy(g => g.Key);

        foreach (var grupo in porFabricante)
        {
            Console.WriteLine(
                $"{grupo.Key}: {grupo.Count()} dispositivo(s)");
        }

        Console.WriteLine("\n--- VERIFICAÇÃO DE ANOMALIAS ---");

        var anomalias = dispositivosAtivos
            .Where(d => d.ConsumoBanda > 500)
            .ToList();

        if (anomalias.Any())
        {
            foreach (Dispositivo dispositivo in anomalias)
            {
                Console.WriteLine(
                    $"ALERTA: {dispositivo.Nome} consumindo " +
                    $"{dispositivo.ConsumoBanda:F2} Mbps!");
            }
        }
        else
        {
            Console.WriteLine("Nenhuma anomalia detectada.");
        }

        Console.WriteLine("=========================================");
    }


    //Missão 4 — Histórico de auditoria e desfazer última ação
    public void DesfazerUltimaAcao()
    {
        if (historico.Count == 0)
        {
            Console.WriteLine("Não há ações no histórico.");
            return;
        }

        string acao = historico.Pop();

        Console.WriteLine("Última ação retirada do histórico:");
        Console.WriteLine(acao);
    }



    //Missão 5 — Gerador de relatórios universal
    public void ImprimirRelatorio(
        IEnumerable<Dispositivo> colecao)
    {
        Console.WriteLine("\n========== RELATÓRIO ==========");

        bool encontrou = false;

        foreach (Dispositivo dispositivo in colecao)
        {
            Console.WriteLine(dispositivo);
            encontrou = true;
        }

        if (!encontrou)
        {
            Console.WriteLine("Nenhum dispositivo encontrado.");
        }

        Console.WriteLine("================================");
    }



    // Missão 6 — Firewall dinâmico com Lambda

    public void AplicarFirewall(Func<Dispositivo, bool> regraBloqueio)
    {
        var bloqueados = dispositivosAtivos
            .Where(regraBloqueio)
            .ToList();

        foreach (Dispositivo dispositivo in bloqueados)
        {
            dispositivosAtivos.Remove(dispositivo);

            Console.WriteLine(
                $"Bloqueado: {dispositivo.Nome} " +
                $"({dispositivo.Fabricante})");
        }

        historico.Push(
            $"Regra de Firewall aplicada em " +
            $"{DateTime.Now:dd/MM/yyyy HH:mm:ss}. " +
            $"Dispositivos removidos: {bloqueados.Count}.");

        Console.WriteLine(
            $"Firewall aplicado. Total removido: {bloqueados.Count}");
    }

}
