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
            MusicaDeFundo.Parar();

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

            while (relogio.ElapsedMilliseconds < duracaoMs)
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

                bool chegouNaLinhaDeColisao = obstaculo.Linha == Constantes.LINHA_COLISAO;

                if (chegouNaLinhaDeColisao && !obstaculo.JaContabilizado)
                {
                    obstaculo.JaContabilizado = true;

                    if (obstaculo.Pista == _carro.Pista)
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

            if (nivelCalculado > _nivel)
            {
                _nivel = nivelCalculado;

                int novaVelocidade = Constantes.VELOCIDADE_INICIAL_MS
                    - (_nivel - 1) * Constantes.REDUCAO_VELOCIDADE_POR_NIVEL_MS;

                _velocidadeMs = Math.Max(novaVelocidade, Constantes.VELOCIDADE_MINIMA_MS);
                EfeitosSonoros.TocarSubidaDeNivel();
            }
        }

        /// <summary>
        /// Garante que sempre existam entre 2 e 3 obstáculos visíveis,
        /// gerando novos quando necessário, de forma que o jogo permaneça
        /// jogável (nunca bloqueando as duas pistas na mesma altura).
        /// </summary>
        private void GerarNovosObstaculosSeNecessario()
        {
            int ativos = _obstaculos.Count(o => o.Ativo);
            if (ativos >= Constantes.MAX_OBSTACULOS_SIMULTANEOS) return;

            // Sorteia uma faixa e só cria o obstáculo se a verificação
            // recursiva de espaço livre confirmar que é seguro/justo.
            int pistaSorteada = _sorteio.Next(2);
            int linhaInicial = 0;

            bool existeConflito = ExisteObstaculoConflitante(_obstaculos, 0, pistaSorteada, linhaInicial);

            if (!existeConflito)
            {
                _obstaculos.Add(new Obstaculo(linhaInicial, pistaSorteada));
            }
            // Se houver conflito, simplesmente não gera nesta rodada;
            // a próxima chamada do loop tentará novamente.

            // Caso o número de obstáculos ainda esteja abaixo do mínimo
            // desejado, tenta complementar na faixa oposta.
            if (_obstaculos.Count(o => o.Ativo) < Constantes.MIN_OBSTACULOS_SIMULTANEOS)
            {
                int pistaOposta = 1 - pistaSorteada;
                bool conflitoOposto = ExisteObstaculoConflitante(_obstaculos, 0, pistaOposta, linhaInicial);

                if (!conflitoOposto)
                {
                    _obstaculos.Add(new Obstaculo(linhaInicial, pistaOposta));
                }
            }
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
