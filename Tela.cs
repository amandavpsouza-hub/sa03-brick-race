using System.Text;

namespace BrickRace
{
    /// <summary>
    /// Concentra toda a apresentação visual do jogo em modo texto:
    /// menu, instruções, tela de partida (pista + painel) e tela de
    /// fim de jogo. A pista é montada em uma MATRIZ de caracteres
    /// (char[,]) antes de ser impressa, o que evidencia o uso de
    /// matrizes exigido pela atividade.
    /// </summary>
    public static class Tela
    {
        public static void MostrarMenu(int recorde)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════╗");
            Console.WriteLine("║              BRICK RACE - C#                ║");
            Console.WriteLine("║           Corrida em C# - SA 03             ║");
            Console.WriteLine("╠════════════════════════════════════════════╣");
            Console.WriteLine("║ 1 - Iniciar jogo                            ║");
            Console.WriteLine("║ 2 - Instruções                              ║");
            Console.WriteLine("║ 3 - Ver último resultado                    ║");
            Console.WriteLine("║ 0 - Sair                                    ║");
            Console.WriteLine("╚════════════════════════════════════════════╝");
            Console.WriteLine($" Recorde atual: {recorde:000000}");
            Console.Write("\nEscolha uma opção: ");
        }

        public static void MostrarOpcaoInvalidaMenu()
        {
            Console.WriteLine("\nOpção inválida. Pressione qualquer tecla para voltar ao menu...");
            Console.ReadKey(true);
        }

        public static void MostrarInstrucoes()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════╗");
            Console.WriteLine("║                INSTRUÇÕES                   ║");
            Console.WriteLine("╚════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("OBJETIVO:");
            Console.WriteLine("Desvie dos obstaculos trocando entre a pista esquerda e a pista direita.");
            Console.WriteLine();
            Console.WriteLine("CONTROLES:");
            Console.WriteLine("A ou seta esquerda = mover para a esquerda");
            Console.WriteLine("D ou seta direita  = mover para a direita");
            Console.WriteLine("ESC = sair da partida");
            Console.WriteLine();
            Console.WriteLine("REGRAS:");
            Console.WriteLine("Voce comeca com 3 vidas.");
            Console.WriteLine("Cada obstaculo desviado aumenta sua pontuacao.");
            Console.WriteLine("Ao bater em um obstaculo, voce perde uma vida.");
            Console.WriteLine("Quando as vidas chegam a zero, a partida termina.");
            Console.WriteLine();
            Console.WriteLine("Pressione qualquer tecla para voltar ao menu...");
            Console.ReadKey(true);
        }

        public static void MostrarUltimoResultado(ResultadoPartida? resultado)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════╗");
            Console.WriteLine("║           ÚLTIMO RESULTADO                  ║");
            Console.WriteLine("╚════════════════════════════════════════════╝");
            Console.WriteLine();

            if (resultado is null)
            {
                Console.WriteLine("Nenhuma partida foi jogada ainda.");
            }
            else
            {
                Console.WriteLine($"Pontuacao final       : {resultado.Pontuacao:000000}");
                Console.WriteLine($"Nivel alcancado        : {resultado.Nivel:00}");
                Console.WriteLine($"Obstaculos desviados   : {resultado.ObstaculosDesviados}");
            }

            Console.WriteLine();
            Console.WriteLine("Pressione qualquer tecla para voltar ao menu...");
            Console.ReadKey(true);
        }

        public static void MostrarFimDeJogo(ResultadoPartida resultado)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════╗");
            Console.WriteLine("║                FIM DE JOGO                  ║");
            Console.WriteLine("╠════════════════════════════════════════════╣");
            Console.WriteLine($"║ Pontuacao final : {resultado.Pontuacao,6:000000}                 ║");
            Console.WriteLine($"║ Nivel alcancado : {resultado.Nivel,2:00}                       ║");
            Console.WriteLine($"║ Obstaculos desviados : {resultado.ObstaculosDesviados,3}           ║");
            Console.WriteLine("║                                              ║");
            Console.WriteLine("║ Pressione qualquer tecla para voltar        ║");
            Console.WriteLine("║ ao menu principal.                          ║");
            Console.WriteLine("╚════════════════════════════════════════════╝");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Monta a matriz de caracteres da pista (duas faixas) com o carro
        /// e os obstáculos já desenhados, e imprime junto do painel lateral.
        /// </summary>
        public static void DesenharPartida(Carro carro, List<Obstaculo> obstaculos, int pontuacao,
            int recorde, int nivel, int vidas, int velocidadeMs)
        {
            int colunasPista = (Constantes.LARGURA_FAIXA * 2) + 1; // + 1 coluna de separação
            char[,] matriz = new char[Constantes.ALTURA_PISTA, colunasPista];

            // Preenche a matriz toda com espaço em branco.
            for (int linha = 0; linha < Constantes.ALTURA_PISTA; linha++)
            {
                for (int coluna = 0; coluna < colunasPista; coluna++)
                {
                    matriz[linha, coluna] = ' ';
                }
                matriz[linha, Constantes.LARGURA_FAIXA] = '│'; // separador entre as duas faixas
            }

            // Desenha cada obstáculo ativo na matriz.
            foreach (var obstaculo in obstaculos)
            {
                if (!obstaculo.Ativo) continue;
                DesenharSprite(matriz, obstaculo.Linha, obstaculo.Pista, Constantes.FORMATO_OBSTACULO);
            }

            // Desenha o carro por último, na parte inferior da pista.
            int linhaCarro = Constantes.ALTURA_PISTA - Constantes.ALTURA_CARRO;
            DesenharSprite(matriz, linhaCarro, carro.Pista, Constantes.FORMATO_CARRO);

            ImprimirMatrizComPainel(matriz, pontuacao, recorde, nivel, vidas, velocidadeMs);
        }

        /// <summary>
        /// Escreve um "sprite" (carro ou obstáculo, 3 linhas) dentro da matriz
        /// da pista, na faixa indicada, a partir da linha de topo informada.
        /// Linhas fora do intervalo válido da matriz são simplesmente ignoradas.
        /// </summary>
        private static void DesenharSprite(char[,] matriz, int linhaTopo, int pista, string[] formato)
        {
            int colunaBase = pista == 0 ? 1 : Constantes.LARGURA_FAIXA + 2;

            for (int i = 0; i < formato.Length; i++)
            {
                int linhaAtual = linhaTopo + i;
                if (linhaAtual < 0 || linhaAtual >= Constantes.ALTURA_PISTA) continue;

                string textoLinha = formato[i];
                for (int j = 0; j < textoLinha.Length; j++)
                {
                    if (textoLinha[j] == ' ') continue; // preserva o que já estiver desenhado
                    int coluna = colunaBase + j;
                    matriz[linhaAtual, coluna] = textoLinha[j];
                }
            }
        }

        private static void ImprimirMatrizComPainel(char[,] matriz, int pontuacao, int recorde,
            int nivel, int vidas, int velocidadeMs)
        {
            var construtor = new StringBuilder();
            int linhas = matriz.GetLength(0);
            int colunas = matriz.GetLength(1);

            string[] painel =
            {
                " PONTOS  : " + pontuacao.ToString("000000"),
                " RECORDE : " + recorde.ToString("000000"),
                " NIVEL   : " + nivel.ToString("00"),
                " VIDAS   : " + vidas,
                " VELOC.  : " + velocidadeMs + " ms",
                "",
                " CONTROLES",
                " A ou seta esquerda",
                " D ou seta direita",
                " ESC = sair",
                "",
                " LEGENDA",
                " CARRO = jogador",
                " OBSTACULO = bloqueio"
            };

            construtor.AppendLine("╔══════════════════════════════╦════════════════════════════════╗");
            construtor.AppendLine("║            PISTA             ║             PAINEL              ║");
            construtor.AppendLine("╠══════════════════════════════╬════════════════════════════════╣");

            for (int linha = 0; linha < linhas; linha++)
            {
                construtor.Append("║ ");
                for (int coluna = 0; coluna < colunas; coluna++)
                {
                    construtor.Append(matriz[linha, coluna]);
                }
                construtor.Append(" ║ ");

                string textoPainel = linha < painel.Length ? painel[linha] : "";
                construtor.Append(textoPainel.PadRight(32));
                construtor.AppendLine("║");
            }

            construtor.AppendLine("╚══════════════════════════════╩════════════════════════════════╝");

            Console.SetCursorPosition(0, 0);
            Console.Write(construtor.ToString());
        }
    }
}
