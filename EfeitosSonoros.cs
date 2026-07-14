using NAudio.Wave;
using System.Threading;

namespace BrickRace
{
    /// <summary>
    /// Efeitos sonoros do jogo usando os arquivos MP3 do projeto.
    /// </summary>
    public static class EfeitosSonoros
    {
        private static IWavePlayer? _playerEfeito;
        private static WaveStream? _streamEfeito;
        private static readonly object _sincronizacao = new();

        public static void TocarColisao()
        {
            ReproduzirArquivo(Constantes.CAMINHO_SOM_PERDA_VIDA);
        }

        public static void TocarSubidaDeNivel()
        {
            // Evita sobreposição com a trilha principal do jogo.
        }

        public static void TocarDerrota()
        {
            MusicaDeFundo.Parar();
            ReproduzirArquivo(Constantes.CAMINHO_SOM_DERROTA);
            Thread.Sleep(2000); // aguarda o som tocar
        }

        private static void ReproduzirArquivo(string caminho)
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }

            var caminhoResolvido = ResolverCaminho(caminho);
            if (string.IsNullOrWhiteSpace(caminhoResolvido))
            {
                return;
            }

            lock (_sincronizacao)
            {
                try
                {
                    // Para o som anterior se ainda estiver tocando
                    _playerEfeito?.Stop();
                    _streamEfeito?.Dispose();

                    var reader = new AudioFileReader(caminhoResolvido);
                    var player = new WaveOutEvent();
                    player.Init(reader);
                    
                    // Mantém as referências para o player não ser coletado
                    _streamEfeito = reader;
                    _playerEfeito = player;
                    
                    player.Play();
                }
                catch
                {
                }
            }
        }

        private static string? ResolverCaminho(string caminho)
        {
            if (string.IsNullOrWhiteSpace(caminho))
            {
                return null;
            }

            var locais = new List<string>
            {
                Path.Combine(Directory.GetCurrentDirectory(), caminho),
                Path.Combine(AppContext.BaseDirectory, caminho),
                Path.Combine(Directory.GetCurrentDirectory(), "Sons", caminho),
                Path.Combine(AppContext.BaseDirectory, "Sons", caminho)
            };

            foreach (var local in locais)
            {
                if (File.Exists(local))
                {
                    return local;
                }
            }

            return null;
        }
    }
}
