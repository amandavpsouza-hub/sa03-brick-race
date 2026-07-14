namespace BrickRace
{
    public class Recursividade
    {
        public bool ExisteConflito(int indice, List<Obstaculo> obstaculos, int pistaNova, int linhaNova)
        {
            if (indice >= obstaculos.Count)
            {
                return false;
            }

            Obstaculo atual = obstaculos[indice];

            bool mesmaAltura = Math.Abs(atual.Linha - linhaNova) <= Constantes.MARGEM_SEGURANCA_GERACAO;

            if (atual.Ativo && atual.Pista != pistaNova && mesmaAltura)
            {
                return true;
            }

            return ExisteConflito(indice + 1, obstaculos, pistaNova, linhaNova);
        }
    }
}