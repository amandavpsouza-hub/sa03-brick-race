namespace BrickRace
{
    /// <summary>
    /// Gerencia o menu principal do jogo Brick Race, mantendo o loop
    /// de menu em execução até que o jogador escolha sair.
    /// </summary>
    public static class Menu
    {
        public static void Executar()
        {
            bool sairDoPrograma = false;

            while (!sairDoPrograma)
            {
                int recorde = Persistencia.CarregarRecorde();
                Tela.MostrarMenu(recorde);

                string? entrada = Console.ReadLine();

                switch (entrada)
                {
                    case "1":
                        IniciarJogo();
                        break;

                    case "2":
                        MostrarInstrucoes();
                        break;

                    case "3":
                        MostrarUltimoResultado();
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

        private static void IniciarJogo()
        {
            var jogo = new Jogo();
            var resultado = jogo.Iniciar();
            Tela.MostrarFimDeJogo(resultado);
        }

        private static void MostrarInstrucoes()
        {
            Tela.MostrarInstrucoes();
        }

        private static void MostrarUltimoResultado()
        {
            var ultimoResultado = Persistencia.CarregarUltimoResultado();
            Tela.MostrarUltimoResultado(ultimoResultado);
        }
    }
}
