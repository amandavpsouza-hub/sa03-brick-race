using NAudio.Wave;

namespace BrickRace
{
    /// <summary>
    /// Responsável por tocar a trilha de fundo do jogo.
    /// </summary>
    public static class MusicaDeFundo
    {
        private static readonly object _sincronizacao = new();
        private static IWavePlayer? _player;
        private static WaveStream? _stream;

        public static void Tocar(string caminho)
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }

            lock (_sincronizacao)
            {
                PararInterno();

                var caminhoResolvido = ResolverCaminho(caminho);
                if (string.IsNullOrWhiteSpace(caminhoResolvido))
                {
                    return;
                }

                try
                {
                    _stream = new LoopStream(new AudioFileReader(caminhoResolvido));
                    _player = new WaveOutEvent();
                    _player.Init(_stream);
                    _player.Play();
                }
                catch
                {
                    PararInterno();
                }
            }
        }

        public static void Parar()
        {
            lock (_sincronizacao)
            {
                PararInterno();
            }
        }

        private static void PararInterno()
        {
            _player?.Stop();
            _stream?.Dispose();
            _stream = null;
            _player?.Dispose();
            _player = null;
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

        private sealed class LoopStream : WaveStream
        {
            private readonly WaveStream _source;

            public LoopStream(WaveStream source)
            {
                _source = source;
            }

            public override WaveFormat WaveFormat => _source.WaveFormat;

            public override long Length => _source.Length;

            public override long Position
            {
                get => _source.Position;
                set => _source.Position = value % _source.Length;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int totalRead = 0;

                while (totalRead < count)
                {
                    int read = _source.Read(buffer, offset + totalRead, count - totalRead);

                    if (read == 0)
                    {
                        if (_source.Position == 0)
                        {
                            break;
                        }

                        _source.Position = 0;
                    }
                    else
                    {
                        totalRead += read;
                    }
                }

                return totalRead;
            }
        }
    }
}
