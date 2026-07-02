using System.IO;

namespace BrickRace
{
    public static class Persistencia
    {
        private const string CaminhoRecorde = "recorde.txt";
        private const string CaminhoUltimoResultado = "ultimo_resultado.txt";

        public static int CarregarRecorde()
        {
            if (!File.Exists(CaminhoRecorde))
            {
                return 0;
            }

            var texto = File.ReadAllText(CaminhoRecorde).Trim();
            return int.TryParse(texto, out var valor) ? valor : 0;
        }

        public static void SalvarRecorde(int recorde)
        {
            File.WriteAllText(CaminhoRecorde, recorde.ToString());
        }

        public static ResultadoPartida? CarregarUltimoResultado()
        {
            if (!File.Exists(CaminhoUltimoResultado))
            {
                return null;
            }

            var texto = File.ReadAllText(CaminhoUltimoResultado);
            return ResultadoPartida.DeTexto(texto);
        }

        public static void SalvarUltimoResultado(ResultadoPartida resultado)
        {
            File.WriteAllText(CaminhoUltimoResultado, resultado.ToString());
        }
    }
}
