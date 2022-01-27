
using System.Diagnostics;
using System.Text;

namespace ConnectFour_MCTS
{
    public class MCTS
    {
        public int FirstPlayer { get; set; }
        public int SecondPlayer { get; internal set; }
        public int iterations = 1_000_000_000;
        public Random rand = new Random();
        public static int SimulationsCount = 0;
        public static int MaxDepth = 0;

        public static bool CalculationsInProgress = false;
        public static List<int> SimulationsCounterRecords = new List<int>();
        public async Task<Node> SearchAsync(char[,] _board, int _timeout)
        {
            CalculationsInProgress = true;
            MaxDepth = 0;
            SimulationsCount = 0;
            
            Task LoadingTask = new Task(()=>Loading(_timeout));
            LoadingTask.Start();

            Node root = new Node(_parent: null, _board: _board, FirstPlayer == 1 ? 2 : 1);
            // timeout settings
            var _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;
            _tokenSource.CancelAfter(_timeout);

            for (int i = 0; i < iterations; i++)
            {
                if (token.IsCancellationRequested)
                {
                    // Console.WriteLine("Szukanie anulowane. Zwracanie aktualnego wygenerowanego wyniku.");
                    break;
                }
                // select node
                Node node = SelectNode(root);

                // rollout
                double score = Rollout(node.gameState);

                Backpropagate(node, score);
                // backpropagation
            }

            CalculationsInProgress = false;
            LoadingTask.Wait();
            // DEBUG show statistics
            // foreach(var child in root.childrens)
            // {
            //     Console.WriteLine($"Value: {child.value}\tVisits: {child.visits}\tUCB1: {child.UCB1Score}");
            //     Engine.DrawBoard(child.gameState.boardArray);
            // }
            // Console.WriteLine();

            SimulationsCounterRecords.Add(SimulationsCount);

            return GetBestMove(root, 0);
        }
        private Node GetRoot(Node child)
        {
            if (child.parent != null)
            {
                return GetRoot(child.parent);
            }
            else
                return child;
        }
        private Node GetBestMove(Node _node, int exploration)
        { 
            float bestScore = float.NegativeInfinity; // -oo
            List<Node> bestMoves = new();

            double lnOftotalVisits = Math.Log(GetRoot(_node).visits);

            double explorationConst = exploration;
            foreach (var child in _node.childrens)
            {
                int player = child.gameState.latestPlayer==FirstPlayer?1:-1;
                double averageScorePerVisitCurrentNode = (child.value) / (double)child.visits;
                double UCBScore = player * averageScorePerVisitCurrentNode + (explorationConst * (Math.Sqrt(lnOftotalVisits / (double)child.visits)));

                child.UCB1Score = (float)UCBScore;

                if ((float)UCBScore > bestScore)
                {
                    bestScore = (float)UCBScore;
                    bestMoves = new() { child };
                }
                else if (UCBScore == bestScore)
                {
                    bestMoves.Add(child);
                }
            }

            if(bestMoves.Count==0)
            {
                Console.WriteLine("???????? ");
            }
            return bestMoves[rand.Next(0, bestMoves.Count)];

        }
        private Node SelectNode(Node _node)
        {
            while (!_node.IsTerminated)
            {
                if (_node.isFullyExpanded)
                    _node = GetBestMove(_node, 2);
                else
                    return Expand(_node);
            }
            return _node;
        }
        public Node Expand(Node parent)
        {
            List<int> possibleMovements = Engine.GetLegalMovesList(parent.gameState.boardArray).ToList();
            List<int> usedMovesInChilds = parent.childrens.Select(x => (int)x.gameState.latestMovement).ToList();
            usedMovesInChilds.ForEach(x => possibleMovements.Remove(x));

            if (possibleMovements.Count == 0)
            {
                parent.isFullyExpanded = true;
                return parent;
            }

            Node newNode = new Node(parent, parent.gameState.boardArray, parent.gameState.latestPlayer);
            parent.childrens.Add(newNode);
            MaxDepth = newNode.level > MaxDepth ? newNode.level : MaxDepth;
            int randomUniqueMovealongChildrens = possibleMovements[rand.Next(0, possibleMovements.Count)];

            Engine.MakeMove(randomUniqueMovealongChildrens, newNode.gameState.latestPlayer, newNode.gameState.boardArray);
            newNode.gameState.latestMovement = randomUniqueMovealongChildrens;
            // check if is terminated
            var result = Engine.IsGameEnded(newNode.gameState.boardArray);
            if (result.status)
            {
                if (result.winnerMark != null)
                {
                    int WINNER_ID = result.winnerMark == Engine.player1_mark ? 1 : 2;

                    if (WINNER_ID != FirstPlayer)
                    {
                        //Console.WriteLine("Przegrałbys tak czy siak, przeciwnik moze wygrac w nastepnym ruchu na 100%");
                        parent.IsTerminated = true;
                        parent.value = -1_000_000;
                        return newNode;
                    }
                }
            }
            return newNode;
        }
        private void Backpropagate(Node node, double score)
        {
            //  Console.WriteLine("# BACKPROPAGATION");
            while (node.parent != null)
            {
                node.visits += 1;
                node.value += score;

                node.victoriesCount += score == 1 ? 1 : 0;
                node.drawsCount += score == 0 ? 1 : 0;
                node.losesCount += score == -1 ? 1 : 0;

                node = node.parent;
            }
            node.visits += 1;
            node.value += score;
        }
        private double Rollout(GameState game)
        {
            var COPYIED_GameState = new GameState(game.boardArray, game.latestMovement, game.latestPlayer);
            int nextMovePlayerId = COPYIED_GameState.latestPlayer==1?2:1;
            var result = Engine.Simulate(nextMovePlayerId, COPYIED_GameState.boardArray);

            if (result.status)
            {
                if (result.winnerMark != null)
                {
                    var winnerID = result.winnerMark == Engine.player1_mark ? 1 : 2;
                    if (winnerID == FirstPlayer)
                        return 1;
                    else
                        return -1;
                }
                else
                {
                    return 0;
                }
            }

            throw new Exception("nie powinno sie zdażyc ? bo gra nadal trwa");
        }
        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
        public static async Task Loading(int ms)
        {
            if(ms >=1000)
            {
                char empty_mark = Char.Parse("░");
                char full_mark = Char.Parse("▓");
                int splitValue = 28;
                int interval = (ms) / splitValue;
                int counter = 1;        // 120/1000
                StringBuilder progressBar = new StringBuilder("", 28);
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("╔═══════════════════════════════════╗");
                Console.WriteLine();
                while (MCTS.CalculationsInProgress)
                {
                    progressBar.Clear();
                    var progress =Math.Round(counter/(double)splitValue,2);
                    progressBar.Append(new string(full_mark, (int)(progress*splitValue)));
		        	progressBar.Append(new string(empty_mark, splitValue-(int)(progress*splitValue)));

                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    counter++;
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($"    {progressBar.ToString()}   ");
                    Console.ResetColor();
                    Thread.Sleep(interval);
                    if (counter >= splitValue)
                    {
                        break;
                    }

                }
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                ClearCurrentConsoleLine();
            }
            else
            {
                return;
            }
        }
    }
}
