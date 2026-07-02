namespace BrickRace
{
    /// <summary>
    /// Guarda o resultado de uma partida encerrada, usado tanto para
    /// exibir a tela de fim de jogo quanto para salvar/consultar o
    /// último resultado a partir do menu inicial.
    /// </summary>
    public class ResultadoPartida
    {
        public int Pontuacao { get; set; }
        public int Nivel { get; set; }
        public int ObstaculosDesviados { get; set; }

        public override string ToString()
        {
            // Formato simples "chave=valor;" para facilitar leitura/escrita em arquivo texto.
            return $"pontuacao={Pontuacao};nivel={Nivel};desviados={ObstaculosDesviados}";
        }

        /// <summary>
        /// Reconstrói um ResultadoPartida a partir do formato salvo por ToString().
        /// Retorna null se a linha estiver corrompida ou vazia.
        /// </summary>
        public static ResultadoPartida? DeTexto(string linha)
        {
            if (string.IsNullOrWhiteSpace(linha))
                return null;

            var resultado = new ResultadoPartida();
            var partes = linha.Split(';');

            foreach (var parte in partes)
            {
                var chaveValor = parte.Split('=');
                if (chaveValor.Length != 2) continue;

                var chave = chaveValor[0].Trim();
                var valorTexto = chaveValor[1].Trim();

                if (!int.TryParse(valorTexto, out int valor)) continue;

                switch (chave)
                {
                    case "pontuacao": resultado.Pontuacao = valor; break;
                    case "nivel": resultado.Nivel = valor; break;
                    case "desviados": resultado.ObstaculosDesviados = valor; break;
                }
            }

            return resultado;
        }
    }
}
