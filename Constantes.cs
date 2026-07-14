namespace BrickRace
{
    public static class Constantes
    {
        public const int LARGURA_FAIXA = 16;
        public const int ALTURA_PISTA = 28;
        public const int ALTURA_CARRO = 3;

        public const int VIDAS_INICIAIS = 3;
        public const int VELOCIDADE_INICIAL_MS = 200; // velocidade real inicial 5x mais rápida
        public const int VELOCIDADE_MINIMA_MS = 10;
        public const double VELOCIDADE_DIMINUIR_POR_NIVEL = 0.97;
        public const double VELOCIDADE_MULTIPLICADOR_INICIAL = 3.0;
        public const int PONTOS_POR_DESVIO = 10;
        public const int PONTOS_PARA_SUBIR_NIVEL = 100;
        public const int LINHA_COLISAO = ALTURA_PISTA - ALTURA_CARRO;
        public const int OBSTACULOS_BASE = 2;
        public const int OBSTACULOS_POR_NIVEL = 1;
        public const int OBSTACULOS_MAXIMOS = 6;
        public const int MARGEM_SEGURANCA_GERACAO = 8;
        public const int TEMPO_BLOQUEIO_OPOSITO = 2;
        public const string CAMINHO_TRILHA_SONORA = "Final_Lap_Rush.mp3";
        public const string CAMINHO_SOM_PERDA_VIDA = "perdeu-vida.mp3";
        public const string CAMINHO_SOM_DERROTA = "som-perdeu-2.mp3";

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
