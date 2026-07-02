namespace BrickRace
{
    public sealed class Carro
    {
        public Carro(int pistaInicial)
        {
            Pista = Math.Clamp(pistaInicial, 0, 1);
        }

        public int Pista { get; set; }

        public void MoverParaEsquerda()
        {
            Pista = Math.Max(0, Pista - 1);
        }

        public void MoverParaDireita()
        {
            Pista = Math.Min(1, Pista + 1);
        }
    }
}
