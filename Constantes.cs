namespace BrickRace
{
    public static class Constantes
    {
        public const int LARGURA_FAIXA = 16;
        public const int ALTURA_PISTA = 28;
        public const int ALTURA_CARRO = 3;

        public const int VIDAS_INICIAIS = 3;
        public const int VELOCIDADE_INICIAL_MS = 250;
        public const int VELOCIDADE_MINIMA_MS = 80;
        public const int REDUCAO_VELOCIDADE_POR_NIVEL_MS = 20;
        public const int PONTOS_POR_DESVIO = 10;
        public const int PONTOS_PARA_SUBIR_NIVEL = 50;
        public const int LINHA_COLISAO = ALTURA_PISTA - ALTURA_CARRO;
        public const int MAX_OBSTACULOS_SIMULTANEOS = 2;
        public const int MIN_OBSTACULOS_SIMULTANEOS = 1;
        public const int MARGEM_SEGURANCA_GERACAO = 6;
        public const string CAMINHO_TRILHA_SONORA = "musica.wav";

        public static readonly string[] FORMATO_OBSTACULO =
        {
            "  ***  ",
            " ***** ",
            "  ***  "
        };

        public static readonly string[] FORMATO_CARRO =
        {
            "   ^   ",
            "  /A\\  ",
            " /AAA\\ "
        };
    }
}
