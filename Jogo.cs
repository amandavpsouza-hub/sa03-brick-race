using System.Threading;

namespace BrickRace
{
    /// <summary>
    /// Controla uma partida completa do Brick Race: cria o carro, gera e
    /// movimenta os obstáculos, verifica colisão, atualiza pontuação,
    /// nível e velocidade, e desenha a tela continuamente até o jogador
    /// perder todas as vidas ou apertar ESC.
    /// </summary>
    public class Jogo
    {
        private readonly Carro _carro = new(pistaInicial: 0);
        private readonly List<Obstaculo> _obstaculos = new(); // VETOR/lista de obstáculos ativos
        private readonly Random _sorteio = new();

        private int _pontuacao = 0;
        private int _nivel = 1;
        private int _vidas = Constantes.VIDAS_INICIAIS;
        private int _velocidadeMs = Constantes.VELOCIDADE_INICIAL_MS;
        private int _obstaculosDesviados = 0;
        private int _rodadasBloqueioOposto = 0;
        private int _pistaUltimoGerado = -1;
        private bool _sairSolicitado = false;

        /// <summary>
        /// Executa a partida do início ao fim e retorna o resultado final,
        /// já persistido em arquivo (recorde e último resultado).
        /// </summary>
        public ResultadoPartida Iniciar()
        {
            int recordeAtual = Persistencia.CarregarRecorde();

            Console.CursorVisible = false;
            MusicaDeFundo.Tocar(Constantes.CAMINHO_TRILHA_SONORA);

            while (_vidas > 0 && !_sairSolicitado)
            {
                AtualizarObstaculos();
                GerarNovosObstaculosSeNecessario();

                Tela.DesenharPartida(_carro, _obstaculos, _pontuacao, recordeAtual, _nivel, _vidas, _velocidadeMs);

                // Aguarda o intervalo da velocidade atual, mas continua lendo
                // teclas durante a espera: a tela nunca fica bloqueada
                // esperando o jogador digitar algo.
                EsperarLendoTeclado(_velocidadeMs);
            }

            Console.CursorVisible = true;
            
            // Toca som de derrota se o jogador perdeu (não foi ESC)
            if (_vidas == 0)
            {
                EfeitosSonoros.TocarDerrota();
            }
            else
            {
                MusicaDeFundo.Parar();
            }

            if (_pontuacao > recordeAtual)
            {
                recordeAtual = _pontuacao;
                Persistencia.SalvarRecorde(recordeAtual);
            }

            var resultado = new ResultadoPartida
            {
                Pontuacao = _pontuacao,
                Nivel = _nivel,
                ObstaculosDesviados = _obstaculosDesviados
            };

            Persistencia.SalvarUltimoResultado(resultado);
            return resultado;
        }

        /// <summary>
        /// Lê o teclado (sem bloquear) durante "duracaoMs" milissegundos,
        /// movendo o carro ou sinalizando saída conforme a tecla pressionada.
        /// </summary>
        private void EsperarLendoTeclado(int duracaoMs)
        {
            var relogio = System.Diagnostics.Stopwatch.StartNew();
            int tempoRestante = duracaoMs;

            while (relogio.ElapsedMilliseconds < tempoRestante)
            {
                if (Console.KeyAvailable)
                {
                    var tecla = Console.ReadKey(intercept: true).Key;

                    if (tecla == ConsoleKey.A || tecla == ConsoleKey.LeftArrow)
                    {
                        _carro.MoverParaEsquerda();
                    }
                    else if (tecla == ConsoleKey.D || tecla == ConsoleKey.RightArrow)
                    {
                        _carro.MoverParaDireita();
                    }
                    else if (tecla == ConsoleKey.W || tecla == ConsoleKey.UpArrow)
                    {
                        tempoRestante = Math.Max(Constantes.VELOCIDADE_MINIMA_MS, (int)(duracaoMs * 0.5));
                    }
                    else if (tecla == ConsoleKey.Escape)
                    {
                        _sairSolicitado = true;
                        return;
                    }
                    // Qualquer outra tecla é ignorada (comportamento neutro),
                    // sem travar ou encerrar a partida.
                }
                else
                {
                    Thread.Sleep(10); // evita uso excessivo de CPU no loop de espera
                }
            }
        }

        /// <summary>
        /// Move todos os obstáculos uma linha para baixo, verifica colisão
        /// com o carro, contabiliza pontos para os que forem desviados e
        /// remove os que já saíram da tela.
        /// </summary>
        private void AtualizarObstaculos()
        {
            foreach (var obstaculo in _obstaculos)
            {
                if (!obstaculo.Ativo) continue;

                obstaculo.Descer();

                if (Colisao.EstaNaLinhaDeColisao(obstaculo) && !obstaculo.JaContabilizado)
                {
                    obstaculo.JaContabilizado = true;

                    if (Colisao.Colidiu(obstaculo, _carro))
                    {
                        RegistrarColisao(obstaculo);
                    }
                    else
                    {
                        RegistrarDesvio();
                    }
                }
            }

            // Remove da lista os obstáculos que já saíram completamente da tela.
            _obstaculos.RemoveAll(o => o.ForaDaTela());
        }

        private void RegistrarColisao(Obstaculo obstaculo)
        {
            _vidas--;
            obstaculo.Ativo = false; // obstáculo da colisão deixa de ser uma ameaça
            EfeitosSonoros.TocarColisao();
        }

        private void RegistrarDesvio()
        {
            _pontuacao += Constantes.PONTOS_POR_DESVIO;
            _obstaculosDesviados++;
            AtualizarNivelEVelocidade();
        }

        /// <summary>
        /// Sobe de nível a cada faixa de pontos atingida e reduz o
        /// intervalo de atualização (aumentando a velocidade), sem deixar
        /// o jogo impossível de ser jogado.
        /// </summary>
        private void AtualizarNivelEVelocidade()
        {
            int nivelCalculado = (_pontuacao / Constantes.PONTOS_PARA_SUBIR_NIVEL) + 1;
            int nivelVelocidade = (int)Math.Max(1, Math.Round((1000.0 / _velocidadeMs) / Constantes.VELOCIDADE_MULTIPLICADOR_INICIAL, MidpointRounding.AwayFromZero));

            if (nivelCalculado > _nivel)
            {
                _nivel = nivelCalculado;
                AjustarVelocidade();
                EfeitosSonoros.TocarSubidaDeNivel();
            }
            else if (nivelVelocidade > _nivel)
            {
                _nivel = nivelVelocidade;
            }
        }

        private void AjustarVelocidade()
        {
            double fator = Math.Pow(Constantes.VELOCIDADE_DIMINUIR_POR_NIVEL, _nivel - 1);
            int novaVelocidade = (int)(Constantes.VELOCIDADE_INICIAL_MS * fator);
            _velocidadeMs = Math.Max(novaVelocidade, Constantes.VELOCIDADE_MINIMA_MS);
        }

        /// <summary>
        /// Garante que sempre existam entre 2 e 3 obstáculos visíveis,
        /// gerando novos quando necessário, de forma que o jogo permaneça
        /// jogável (nunca bloqueando as duas pistas na mesma altura).
        /// </summary>
        private void GerarNovosObstaculosSeNecessario()
        {
            int ativos = _obstaculos.Count(o => o.Ativo);
            double velocidadeRelativa = Math.Max(1.0, (1000.0 / _velocidadeMs) / Constantes.VELOCIDADE_MULTIPLICADOR_INICIAL);
            int maxAtivos = Math.Min(Constantes.OBSTACULOS_MAXIMOS,
                Constantes.OBSTACULOS_BASE + (_nivel - 1) * Constantes.OBSTACULOS_POR_NIVEL);

            double reducaoGeracao = Math.Max(0.5, 1.0 - (velocidadeRelativa - 1) * 0.08);
            int maxAtivosPorVelocidade = Math.Max(1, (int)Math.Round(maxAtivos * reducaoGeracao));
            maxAtivos = Math.Min(maxAtivos, maxAtivosPorVelocidade);

            if (_sorteio.NextDouble() > reducaoGeracao)
            {
                return;
            }

            if (ativos >= maxAtivos) return;

            int linhaInicial = 0;
            int[] pistas = { 0, 1 };
            if (_sorteio.Next(2) == 1)
            {
                Array.Reverse(pistas);
            }

            foreach (var pista in pistas)
            {
                if (_obstaculos.Count(o => o.Ativo) >= maxAtivos)
                {
                    break;
                }

                if (_rodadasBloqueioOposto > 0 && pista != _pistaUltimoGerado)
                {
                    continue;
                }

                if (PodeGerarObstaculo(_obstaculos, pista, linhaInicial))
                {
                    _obstaculos.Add(new Obstaculo(linhaInicial, pista));
                    _pistaUltimoGerado = pista;
                    _rodadasBloqueioOposto = Constantes.TEMPO_BLOQUEIO_OPOSITO;
                    break; // Gera apenas um obstáculo por rodada para evitar meteoros simultâneos opostos
                }
            }

            if (_rodadasBloqueioOposto > 0)
            {
                _rodadasBloqueioOposto--;
            }
        }

        private bool PodeGerarObstaculo(List<Obstaculo> obstaculos, int pista, int linhaNova)
        {
            foreach (var atual in obstaculos)
            {
                if (!atual.Ativo) continue;

                bool alturaProxima = Math.Abs(atual.Linha - linhaNova) <= Constantes.MARGEM_SEGURANCA_GERACAO;

                if (atual.Pista == pista && alturaProxima)
                {
                    return false;
                }

                if (atual.Pista != pista && alturaProxima)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// FUNÇÃO RECURSIVA: verifica, percorrendo a lista de obstáculos
        /// ativos posição por posição, se já existe algum obstáculo na
        /// faixa OPOSTA à faixa desejada e próximo (dentro da margem de
        /// segurança) da linha em que o novo obstáculo nasceria. Isso
        /// evita que as duas pistas fiquem bloqueadas na mesma altura,
        /// garantindo que o jogador sempre tenha uma rota de desvio.
        ///
        /// Caso base: chegou ao fim da lista (índice >= lista.Count) → não
        /// há conflito.
        /// Caso recursivo: verifica o obstáculo atual e chama a si mesma
        /// para o próximo índice.
        /// </summary>
        private bool ExisteObstaculoConflitante(List<Obstaculo> obstaculos, int indice, int pistaNova, int linhaNova)
        {
            if (indice >= obstaculos.Count)
            {
                return false; // caso base: nenhum conflito encontrado na lista
            }

            var atual = obstaculos[indice];

            bool mesmaAlturaAproximada = Math.Abs(atual.Linha - linhaNova) <= Constantes.MARGEM_SEGURANCA_GERACAO;

            if (atual.Ativo && atual.Pista != pistaNova && mesmaAlturaAproximada)
            {
                return true; // encontrado conflito: bloquearia as duas faixas juntas
            }

            // caso recursivo: continua a verificação a partir do próximo obstáculo
            return ExisteObstaculoConflitante(obstaculos, indice + 1, pistaNova, linhaNova);
        }
    }
}
