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
        private const int LARGURA_MENU = 84;
        private const int PAINEL_LARGURA = 64;

        public static void MostrarMenu(int recorde)
        {
            Console.Clear();
            EscreverTopoMenu();
            EscreverLinhaMenu("BRICK RACE - C#");
            EscreverLinhaMenu("Corrida em C# - SA 03");
            EscreverSeparadorMenu();
            EscreverLinhaMenu("1 - Iniciar jogo");
            EscreverLinhaMenu("2 - Instruções");
            EscreverLinhaMenu("3 - Ver último resultado");
            EscreverLinhaMenu("0 - Sair");
            EscreverSeparadorMenu();
            EscreverLinhaMenu($"Recorde atual: {recorde:000000}");
            EscreverRodapeMenu();
            Console.Write("\nEscolha uma opção: ");
        }

        private static void EscreverTopoMenu()
        {
            Console.WriteLine($"╔{new string('═', LARGURA_MENU)}╗");
        }

        private static void EscreverSeparadorMenu()
        {
            Console.WriteLine($"╠{new string('═', LARGURA_MENU)}╣");
        }

        private static void EscreverRodapeMenu()
        {
            Console.WriteLine($"╚{new string('═', LARGURA_MENU)}╝");
        }

        private static void EscreverLinhaMenu(string texto)
        {
            Console.WriteLine($"║ {texto.PadRight(LARGURA_MENU - 2)} ║");
        }

        public static void MostrarOpcaoInvalidaMenu()
        {
            Console.WriteLine("\nOpção inválida. Pressione qualquer tecla para voltar ao menu...");
            Console.ReadKey(true);
        }

        public static void MostrarInstrucoes()
        {
            Console.Clear();
            EscreverTopoMenu();
            EscreverLinhaMenu("INSTRUÇÕES");
            EscreverSeparadorMenu();
            EscreverLinhaMenu("OBJETIVO:");
            EscreverLinhaMenu("Desvie dos obstaculos trocando entre a pista esquerda e a pista direita.");
            EscreverLinhaMenu("");
            EscreverLinhaMenu("CONTROLES:");
            EscreverLinhaMenu("A ou seta esquerda = mover para a esquerda");
            EscreverLinhaMenu("D ou seta direita  = mover para a direita");
            EscreverLinhaMenu("ESC = sair da partida");
            EscreverLinhaMenu("");
            EscreverLinhaMenu("REGRAS:");
            EscreverLinhaMenu("Voce comeca com 3 vidas.");
            EscreverLinhaMenu("Cada obstaculo desviado aumenta sua pontuacao.");
            EscreverLinhaMenu("Ao bater em um obstaculo, voce perde uma vida.");
            EscreverLinhaMenu("Quando as vidas chegam a zero, a partida termina.");
            EscreverLinhaMenu("");
            EscreverLinhaMenu("Pressione qualquer tecla para voltar ao menu...");
            EscreverRodapeMenu();
            Console.ReadKey(true);
        }

        public static void MostrarUltimoResultado(ResultadoPartida? resultado)
        {
            Console.Clear();
            EscreverTopoMenu();
            EscreverLinhaMenu("ÚLTIMO RESULTADO");
            EscreverSeparadorMenu();

            if (resultado is null)
            {
                EscreverLinhaMenu("Nenhuma partida foi jogada ainda.");
            }
            else
            {
                EscreverLinhaMenu($"Pontuacao final       : {resultado.Pontuacao:000000}");
                EscreverLinhaMenu($"Nivel alcancado        : {resultado.Nivel:00}");
                EscreverLinhaMenu($"Obstaculos desviados   : {resultado.ObstaculosDesviados}");
            }

            EscreverLinhaMenu("");
            EscreverLinhaMenu("Pressione qualquer tecla para voltar ao menu...");
            EscreverRodapeMenu();
            Console.ReadKey(true);
        }

        public static void MostrarFimDeJogo(ResultadoPartida resultado)
        {
            Console.Clear();
            EscreverTopoMenu();
            EscreverLinhaMenu("FIM DE JOGO");
            EscreverSeparadorMenu();
            EscreverLinhaMenu($"Pontuacao final : {resultado.Pontuacao:000000}");
            EscreverLinhaMenu($"Nivel alcancado : {resultado.Nivel:00}");
            EscreverLinhaMenu($"Obstaculos desviados : {resultado.ObstaculosDesviados}");
            EscreverLinhaMenu("");
            EscreverLinhaMenu("Pressione qualquer tecla para voltar");
            EscreverLinhaMenu("ao menu principal.");
            EscreverRodapeMenu();
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
            int larguraSprite = formato[0].Length;
            int colunaBase = pista == 0
                ? (Constantes.LARGURA_FAIXA - larguraSprite) / 2
                : Constantes.LARGURA_FAIXA + 1 + (Constantes.LARGURA_FAIXA - larguraSprite) / 2;

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
            int larguraPista = colunas + 2; // espaço interno ao redor da pista

            string[] painel =
            {
                "PONTOS  : " + pontuacao.ToString("000000"),
                "RECORDE : " + recorde.ToString("000000"),
                "NIVEL   : " + nivel.ToString("00"),
                "VIDAS   : " + vidas,
                "VELOC.  : " + velocidadeMs + " ms",
                "",
                "CONTROLES",
                "A ou seta esquerda",
                "D ou seta direita",
                "ESC = sair",
                "",
                "LEGENDA",
                "CARRO = jogador",
                "OBSTACULO = bloqueio",
                "FAIXA 0 = esquerda",
                "FAIXA 1 = direita",
                "NIVEL ATUAL = " + nivel
            };

            string tituloPista = "PISTA".PadLeft((larguraPista + 4 + "PISTA".Length) / 2).PadRight(larguraPista + 4);
            string tituloPainel = "PAINEL".PadLeft((PAINEL_LARGURA + 2 + "PAINEL".Length) / 2).PadRight(PAINEL_LARGURA + 2);

            construtor.AppendLine($"╔{new string('═', larguraPista)}╦{new string('═', PAINEL_LARGURA)}╗");
            construtor.AppendLine($"║ {tituloPista} ║ {tituloPainel} ║");
            construtor.AppendLine($"╠{new string('═', larguraPista)}╬{new string('═', PAINEL_LARGURA)}╣");

            for (int linha = 0; linha < linhas; linha++)
            {
                construtor.Append("║ ");
                for (int coluna = 0; coluna < colunas; coluna++)
                {
                    construtor.Append(matriz[linha, coluna]);
                }
                construtor.Append(" ║ ");

                string textoPainel = linha < painel.Length ? painel[linha] : "";
                construtor.Append(textoPainel.PadRight(PAINEL_LARGURA));
                construtor.AppendLine(" ║");
            }

            construtor.AppendLine($"╚{new string('═', larguraPista)}╩{new string('═', PAINEL_LARGURA)}╝");

            Console.SetCursorPosition(0, 0);
            Console.Write(construtor.ToString());
        }
    }
}
