namespace BrickRace
{
    public sealed class Obstaculo
    {
        public Obstaculo(int linhaInicial, int pista)
        {
            Linha = linhaInicial;
            Pista = Math.Clamp(pista, 0, 1);
            Ativo = true;
            JaContabilizado = false;
        }

        public bool Ativo { get; set; }
        public bool JaContabilizado { get; set; }
        public int Linha { get; set; }
        public int Pista { get; set; }

        public void Descer()
        {
            Linha++;
        }

        public bool ForaDaTela()
        {
            return Linha >= Constantes.ALTURA_PISTA;
        }
    }
}
