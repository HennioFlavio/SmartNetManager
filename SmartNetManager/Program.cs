
using System;

class Program
{
    static void Main()
    {
        GerenciadorRede rede = new GerenciadorRede();

        int opcao;

        do
        {
             Console.WriteLine("1  - Servidor DHCP (Reserva de IP)");
            Console.WriteLine("2  - Adicionar dispositivo à fila");
            Console.WriteLine("3  - Processar fila (Autenticar)");
            Console.WriteLine("4  - Exibir Dashboard");
            Console.WriteLine("5  - Relatório da fila");
            Console.WriteLine("6  - Relatório dos ativos");
            Console.WriteLine("7  - Firewall: Bloquear aparelhos da Apple");
            Console.WriteLine("8  - Firewall: Bloquear aparelhos consumindo mais de 100 Mbps");
            Console.WriteLine("9  - Desfazer última ação");
            Console.WriteLine("0  - Sair");
            Console.Write("Escolha uma opção: ");

            if (!int.TryParse(Console.ReadLine(), out opcao))
            {
                Console.WriteLine("Digite uma opção numérica válida.");
                continue;
            }

            switch (opcao)
            {
                case 1:
                    Console.Write("MAC Address: ");
                    string mac = LerTexto();

                    Console.Write("Endereço IP: ");
                    string ip = LerTexto();

                    rede.ReservarIP(mac, ip);
                    break;

                case 2:
                    Dispositivo dispositivo = CriarDispositivo();
                    rede.AdicionarFila(dispositivo);
                    break;

                case 3:
                    rede.ProcessarFila();
                    break;

                case 4:
                    rede.ExibirDashboard();
                    break;

                case 5:
                    rede.ImprimirRelatorio(rede.Fila);
                    break;

                case 6:
                    rede.ImprimirRelatorio(rede.Ativos);
                    break;

                case 7:
                    rede.AplicarFirewall(
                        d => d.Fabricante.Equals(
                            "Apple",
                            StringComparison.OrdinalIgnoreCase));
                    break;

                case 8:
                    rede.AplicarFirewall(
                        d => d.ConsumoBanda > 100);
                    break;

                case 9:
                    rede.DesfazerUltimaAcao();
                    break;

                case 0:
                    Console.WriteLine("Encerrando SmartNet Manager...");
                    break;

                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }

        } while (opcao != 0);
    }

    static Dispositivo CriarDispositivo()
    {
        Console.WriteLine("\n--- Novo dispositivo ---");

        Console.Write("MAC Address: ");
        string mac = LerTexto();

        Console.Write("Nome: ");
        string nome = LerTexto();

        Console.Write("Fabricante: ");
        string fabricante = LerTexto();

        Console.Write("Endereço IP (caso não haja reserva): ");
        string ip = LerTexto();

        double consumo;

        while (true)
        {
            Console.Write("Consumo de banda em Mbps: ");

            if (double.TryParse(
                Console.ReadLine()?.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out consumo) && consumo >= 0)
            {
                break;
            }

            Console.WriteLine("Informe um número válido, maior ou igual a zero.");
        }

        return new Dispositivo
        {
            MacAddress = mac,
            Nome = nome,
            Fabricante = fabricante,
            EnderecoIP = ip,
            ConsumoBanda = consumo,
            DataConexao = DateTime.Now
        };
    }

    static string LerTexto()
    {
        string? valor;

        do
        {
            valor = Console.ReadLine()?.Trim();

            if (!string.IsNullOrWhiteSpace(valor))
            {
                return valor;
            }

            Console.Write("O campo não pode ficar vazio. Digite novamente: ");

        } while (true);
    }
}
