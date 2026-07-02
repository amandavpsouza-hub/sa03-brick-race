namespace BrickRace
{
    /// <summary>
    /// Ponto de entrada do jogo Brick Race. Mantém o menu inicial em
    /// execução até que o jogador escolha a opção "Sair", tratando
    /// entradas inválidas sem encerrar o programa de forma inesperada.
    /// </summary>
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool sairDoPrograma = false;

            while (!sairDoPrograma)
            {
                int recorde = Persistencia.CarregarRecorde();
                Tela.MostrarMenu(recorde);

                string? entrada = Console.ReadLine();

                switch (entrada)
                {
                    case "1":
                        var jogo = new Jogo();
                        var resultado = jogo.Iniciar();
                        Tela.MostrarFimDeJogo(resultado);
                        break;

                    case "2":
                        Tela.MostrarInstrucoes();
                        break;

                    case "3":
                        var ultimoResultado = Persistencia.CarregarUltimoResultado();
                        Tela.MostrarUltimoResultado(ultimoResultado);
                        break;

                    case "0":
                        sairDoPrograma = true;
                        break;

                    default:
                        // Entrada inválida: avisa e retorna ao menu, sem encerrar o programa.
                        Tela.MostrarOpcaoInvalidaMenu();
                        break;
                }
            }

            Console.Clear();
            Console.WriteLine("Obrigado por jogar Brick Race!");
        }
    }
}
