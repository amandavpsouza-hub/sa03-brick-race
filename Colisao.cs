namespace BrickRace
{
    /// <summary>
    /// Encapsula as regras de colisão entre o carro do jogador e os
    /// obstáculos, permitindo a reutilização e a leitura do código.
    /// </summary>
    public static class Colisao
    {
        public static bool EstaNaLinhaDeColisao(Obstaculo obstaculo)
        {
            return obstaculo.Linha == Constantes.LINHA_COLISAO;
        }

        public static bool Colidiu(Obstaculo obstaculo, Carro carro)
        {
            return obstaculo.Pista == carro.Pista;
        }
    }
}
