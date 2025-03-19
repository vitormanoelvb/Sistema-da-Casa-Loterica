using System;
using System.Collections.Generic;
using System.IO;

namespace Caixa
{
    class Programa
    {
        public struct Documento
        {
            public int UserId { get; }
            public string Nome { get; set; }
            public string Tipo { get; set; } // "Pagar", "Receber", "Jogo", "Preferencial"
            public double Valor { get; set; }

            public Documento(int clientId, string nome, string tipo, double valor)
            {
                UserId = clientId;
                Nome = nome;
                Tipo = tipo;
                Valor = valor;
            }
        }

        public struct SenhaCli
        {
            public int Senha { get; set; }
            public string Tipo { get; set; }
            public double Valor { get; set; }
            // 1 a 4 (null se não houver preferência)
            public int? AtendentePreferido { get; set; }
            // Nome do atendente que realizou o atendimento
            public string Atendente { get; set; }
        }

        // Filas de atendimento
        private static Queue<SenhaCli> fila = new Queue<SenhaCli>();
        private static Queue<SenhaCli> filaPreferencial = new Queue<SenhaCli>();

        // Contador para gerar senhas
        private static int senhaAtual = 0;

        // Quatro caixas – cada um armazenará os atendimentos realizados
        private static Stack<SenhaCli> caixa1 = new Stack<SenhaCli>();
        private static Stack<SenhaCli> caixa2 = new Stack<SenhaCli>();
        private static Stack<SenhaCli> caixa3 = new Stack<SenhaCli>();
        private static Stack<SenhaCli> caixa4 = new Stack<SenhaCli>();

        // Lista global de atendimentos para relatórios
        private static List<SenhaCli> atendimentosRegistrados = new List<SenhaCli>();

        // Senha administrativa
        private static string senhaAdmin = "univale";

        static void Main()
        {
            int opcao;
            do
            {
                MostrarCabecalho();
                Console.WriteLine("1 - Retire aqui a senha do seu atendimento");
                Console.WriteLine("2 - Área Administrativa");
                Console.WriteLine("3 - Créditos");
                Console.WriteLine("0 - Sair");
                Console.Write("Escolha uma opção: ");

                if (!int.TryParse(Console.ReadLine(), out opcao))
                {
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    MostrarLogo();
                    Console.ReadKey();
                    continue;
                }

                switch (opcao)
                {
                    case 1:
                        MenuNovaSenha();
                        break;
                    case 2:
                        AreaAdministrativa();
                        break;
                    case 3:
                        ExibirCreditos();
                        break;
                }

            } while (opcao != 0);
        }

        /// <summary>
        /// Exibe o cabeçalho com as cores solicitadas.
        /// </summary>
        private static void MostrarCabecalho()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("=====================================================");
            Console.WriteLine("=========== - Casa Lotérica / Univale - =============");
            Console.WriteLine("=========== - Seja Sempre   Bem Vindo - =============");
            Console.WriteLine("=====================================================");
            Console.ResetColor();
        }

        /// <summary>
        /// Exibe o logo "v&w/soluções" no final de cada tela.
        /// </summary>
        private static void MostrarLogo()
        {
            Console.WriteLine();
            Console.WriteLine("v&w/soluções");
        }

        /// <summary>
        /// Exibe os créditos de desenvolvimento.
        /// </summary>
        private static void ExibirCreditos()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("===========================================");
            Console.WriteLine("======= Créditos de Desenvolvimento =======");
            Console.WriteLine("===========================================");
            Console.ResetColor();
            Console.WriteLine("Sistema - Casa Lotérica");
            Console.WriteLine("Este sistema foi criado como um dividendo do trabalho atividade prática da disciplina");
            Console.WriteLine("Estrutura de Dados ministrada no 3º período do curso de Sistemas de Informação da");
            Console.WriteLine("Universidade Vale do Rio Doce (Univale).");
            Console.WriteLine("Desenvolvedores: Vitor Manoel Vidal Braz, Wauclidson Alves Dias");
            Console.WriteLine("Objetivo do projeto: Simular o funcionamento de uma Casa Lotérica de quatro caixas, usando-se");
            Console.WriteLine("estruturas de dados para gerenciar as suas filas de clientes, pilhas de documentos a serem");
            Console.WriteLine("processados e, ao final do expediente, analisar suas finanças.");
            Console.WriteLine("Orientação: Henrique Bianor Freitas Silva");
            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
            MostrarLogo();
            Console.ReadKey();
        }

        // ------------------------------------------------------------
        // 1) Retirada de Senha (Cliente)
        // ------------------------------------------------------------
        private static void MenuNovaSenha()
        {
            MostrarCabecalho();
            Console.WriteLine("Escolha o tipo de atendimento:");
            Console.WriteLine("1 - Pagar");
            Console.WriteLine("2 - Receber");
            Console.WriteLine("3 - Jogos");
            Console.WriteLine("4 - Preferencial");
            Console.Write("Opção: ");
            string tipo = Console.ReadLine();
            switch (tipo)
            {
                case "1":
                    tipo = "P";
                    break;
                case "2":
                    tipo = "R";
                    break;
                case "3":
                    tipo = "J";
                    break;
                case "4":
                    tipo = "PR";
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    MostrarLogo();
                    Console.ReadKey();
                    return;
            }
            NovaSenha(tipo);
        }

        private static void NovaSenha(string tipo)
        {
            MostrarCabecalho();
            Console.Write("Digite o valor: ");
            if (!double.TryParse(Console.ReadLine(), out double valor))
            {
                Console.WriteLine("Valor inválido.");
                MostrarLogo();
                Console.ReadKey();
                return;
            }

            senhaAtual++;
            SenhaCli novaSenha = new SenhaCli
            {
                Senha = senhaAtual,
                Tipo = tipo,
                Valor = valor,
                AtendentePreferido = null,
                Atendente = ""
            };

            // Pergunta se o cliente deseja ser atendido por um atendente específico
            Console.Write("Você deseja ser atendido por um atendente específico? (S/N): ");
            string resposta = Console.ReadLine().Trim().ToUpper();
            if (resposta == "S")
            {
                Console.WriteLine("Escolha o atendente desejado:");
                Console.WriteLine("1 - Caixa 1 (Wauclidson)");
                Console.WriteLine("2 - Caixa 2 (Vitor)");
                Console.WriteLine("3 - Caixa 3 (Henrique)");
                Console.WriteLine("4 - Caixa 4 (Anderson)");

                if (int.TryParse(Console.ReadLine(), out int escolha) && escolha >= 1 && escolha <= 4)
                {
                    novaSenha.AtendentePreferido = escolha;
                }
                else
                {
                    Console.WriteLine("Opção inválida. Sem preferência.");
                    novaSenha.AtendentePreferido = null;
                }
            }

            // Enfileira conforme o tipo de atendimento
            if (tipo == "PR")
                filaPreferencial.Enqueue(novaSenha);
            else
                fila.Enqueue(novaSenha);

            // Informa ao cliente sua senha
            Console.WriteLine("Senha gerada com sucesso!");
            Console.WriteLine($"Sua senha é: {novaSenha.Senha}");
            Console.WriteLine("Aguarde, o atendente chamará seu número na tela em breve.");
            Console.WriteLine("Pressione qualquer tecla para retornar ao menu principal.");
            MostrarLogo();
            Console.ReadKey();
        }

        // ------------------------------------------------------------
        // 2) Área Administrativa
        // ------------------------------------------------------------
        private static void AreaAdministrativa()
        {
            MostrarCabecalho();
            Console.Write("Digite a senha administrativa: ");
            string senha = Console.ReadLine();
            if (senha != senhaAdmin)
            {
                Console.WriteLine("Acesso negado.");
                MostrarLogo();
                Console.ReadKey();
                return;
            }

            int opcaoAdm;
            do
            {
                MostrarCabecalho();
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("=====================================================");
                Console.WriteLine("================ Área Administrativa ================");
                Console.WriteLine("=========== - Faça o seu Melhor Sempre!! - ==========");
                Console.WriteLine("=====================================================");
                Console.ResetColor();

                Console.WriteLine("1 - Chamar a próxima senha");
                Console.WriteLine("2 - Relatório de Atendimentos");
                Console.WriteLine("3 - Relatório de Encaminhamento de Caixa");
                Console.WriteLine("4 - Fechamento Total da Lotérica");
                Console.WriteLine("0 - Retornar ao Menu Principal");
                Console.Write("Opção: ");

                if (!int.TryParse(Console.ReadLine(), out opcaoAdm))
                {
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    MostrarLogo();
                    Console.ReadKey();
                    continue;
                }

                switch (opcaoAdm)
                {
                    case 1:
                        NovoAtendimento();
                        break;
                    case 2:
                        RelatorioAtendimentos();
                        break;
                    case 3:
                        RelatorioEncaminhamentoCaixa();
                        break;
                    case 4:
                        FechamentoTotalLoterica();
                        break;
                }

            } while (opcaoAdm != 0);
        }

        // Atendimento (chama a próxima senha)
        private static void NovoAtendimento()
        {
            MostrarCabecalho();
            if (filaPreferencial.Count == 0 && fila.Count == 0)
            {
                Console.WriteLine("Não há clientes na fila.");
                MostrarLogo();
                Console.ReadKey();
                return;
            }

            // Seleciona o próximo atendimento, priorizando filaPreferencial
            SenhaCli atendimento = (filaPreferencial.Count > 0) ? filaPreferencial.Dequeue() : fila.Dequeue();

            int numCaixa = 0;
            // Se o cliente já escolheu um atendente, utiliza essa preferência
            if (atendimento.AtendentePreferido.HasValue)
            {
                numCaixa = atendimento.AtendentePreferido.Value;
                Console.WriteLine($"Cliente preferiu atendimento no Caixa {numCaixa}.");
            }
            else
            {
                // Caso contrário, pergunta ao gerente qual caixa vai atender
                Console.WriteLine("Selecione o caixa desejado para atendimento:");
                Console.WriteLine("1 - Caixa 1 (Wauclidson)");
                Console.WriteLine("2 - Caixa 2 (Vitor)");
                Console.WriteLine("3 - Caixa 3 (Henrique)");
                Console.WriteLine("4 - Caixa 4 (Anderson)");
                if (!int.TryParse(Console.ReadLine(), out numCaixa) || numCaixa < 1 || numCaixa > 4)
                {
                    Console.WriteLine("Opção inválida.");
                    MostrarLogo();
                    Console.ReadKey();
                    return;
                }
            }

            // Define o nome do atendente
            string atendenteNome = "";
            switch (numCaixa)
            {
                case 1:
                    atendenteNome = "Wauclidson";
                    break;
                case 2:
                    atendenteNome = "Vitor";
                    break;
                case 3:
                    atendenteNome = "Henrique";
                    break;
                case 4:
                    atendenteNome = "Anderson";
                    break;
            }
            atendimento.Atendente = atendenteNome;

            // Registra o atendimento no caixa correspondente
            switch (numCaixa)
            {
                case 1:
                    caixa1.Push(atendimento);
                    break;
                case 2:
                    caixa2.Push(atendimento);
                    break;
                case 3:
                    caixa3.Push(atendimento);
                    break;
                case 4:
                    caixa4.Push(atendimento);
                    break;
            }

            // Adiciona ao relatório global
            atendimentosRegistrados.Add(atendimento);

            Console.WriteLine($"Atendimento realizado no Caixa {numCaixa} - {atendenteNome}.");
            Console.WriteLine($"Chamar o cliente com a senha: {atendimento.Senha}");
            Console.WriteLine($"Tipo: {atendimento.Tipo}, Valor: R$ {atendimento.Valor:F2}");
            Console.WriteLine("Pressione qualquer tecla para continuar.");
            MostrarLogo();
            Console.ReadKey();
        }

        // Relatório de Atendimentos: lista todos os atendimentos registrados
        private static void RelatorioAtendimentos()
        {
            MostrarCabecalho();
            Console.WriteLine("=== Relatório de Atendimentos ===");
            if (atendimentosRegistrados.Count == 0)
            {
                Console.WriteLine("Nenhum atendimento registrado até o momento.");
            }
            else
            {
                foreach (var atendimento in atendimentosRegistrados)
                {
                    Console.WriteLine($"Senha: {atendimento.Senha}, Tipo: {atendimento.Tipo}, Valor: R$ {atendimento.Valor:F2}, Atendido por: {atendimento.Atendente}");
                }
            }
            Console.WriteLine("Pressione qualquer tecla para retornar.");
            MostrarLogo();
            Console.ReadKey();
        }

        // Relatório de Encaminhamento de Caixa: mostra quantos atendimentos e o total por caixa
        private static void RelatorioEncaminhamentoCaixa()
        {
            MostrarCabecalho();
            Console.WriteLine("=== Relatório de Encaminhamento de Caixa ===");

            double total1 = 0, total2 = 0, total3 = 0, total4 = 0;
            foreach (var ticket in caixa1) total1 += ticket.Valor;
            foreach (var ticket in caixa2) total2 += ticket.Valor;
            foreach (var ticket in caixa3) total3 += ticket.Valor;
            foreach (var ticket in caixa4) total4 += ticket.Valor;

            Console.WriteLine($"Caixa 1 (Wauclidson): {caixa1.Count} atendimentos, Total: R$ {total1:F2}");
            Console.WriteLine($"Caixa 2 (Vitor): {caixa2.Count} atendimentos, Total: R$ {total2:F2}");
            Console.WriteLine($"Caixa 3 (Henrique): {caixa3.Count} atendimentos, Total: R$ {total3:F2}");
            Console.WriteLine($"Caixa 4 (Anderson): {caixa4.Count} atendimentos, Total: R$ {total4:F2}");
            Console.WriteLine("Pressione qualquer tecla para retornar.");
            MostrarLogo();
            Console.ReadKey();
        }

        // Fechamento Total da Lotérica: soma todos os atendimentos e permite imprimir em TXT
        private static void FechamentoTotalLoterica()
        {
            MostrarCabecalho();
            Console.WriteLine("=== Fechamento Total da Lotérica ===");

            double total1 = 0, total2 = 0, total3 = 0, total4 = 0;
            foreach (var ticket in caixa1) total1 += ticket.Valor;
            foreach (var ticket in caixa2) total2 += ticket.Valor;
            foreach (var ticket in caixa3) total3 += ticket.Valor;
            foreach (var ticket in caixa4) total4 += ticket.Valor;

            int totalAtendimentos = caixa1.Count + caixa2.Count + caixa3.Count + caixa4.Count;
            double totalGeral = total1 + total2 + total3 + total4;

            Console.WriteLine($"Total de Atendimentos: {totalAtendimentos}");
            Console.WriteLine($"Total Geral: R$ {totalGeral:F2}");
            Console.WriteLine();
            Console.WriteLine("Pressione 'I' para imprimir relatório em TXT ou qualquer outra tecla para retornar.");

            char op = Char.ToUpper(Console.ReadKey().KeyChar);
            if (op == 'I')
            {
                ImprimirRelatorioTXT(totalAtendimentos, totalGeral, total1, total2, total3, total4);
                Console.WriteLine("\nRelatório impresso com sucesso (RelatorioLoterica.txt).");
                MostrarLogo();
                Console.ReadKey();
            }
        }

        // Cria arquivo TXT com o fechamento total
        private static void ImprimirRelatorioTXT(
            int totalAtendimentos,
            double totalGeral,
            double total1,
            double total2,
            double total3,
            double total4)
        {
            string caminho = "RelatorioLoterica.txt";
            using (StreamWriter sw = new StreamWriter(caminho))
            {
                sw.WriteLine("=== Fechamento Total da Lotérica ===");
                sw.WriteLine($"Total de Atendimentos: {totalAtendimentos}");
                sw.WriteLine($"Total Geral: R$ {totalGeral:F2}");
                sw.WriteLine();
                sw.WriteLine("Detalhamento por Caixa:");
                sw.WriteLine($"Caixa 1 (Wauclidson): {caixa1.Count} atendimentos, Total: R$ {total1:F2}");
                sw.WriteLine($"Caixa 2 (Vitor): {caixa2.Count} atendimentos, Total: R$ {total2:F2}");
                sw.WriteLine($"Caixa 3 (Henrique): {caixa3.Count} atendimentos, Total: R$ {total3:F2}");
                sw.WriteLine($"Caixa 4 (Anderson): {caixa4.Count} atendimentos, Total: R$ {total4:F2}");
            }
        }
    }
}