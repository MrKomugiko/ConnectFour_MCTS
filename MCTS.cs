
using System.Diagnostics;

namespace ConnectFour_MCTS
{
    public class MCTS
    {
        public int AIBot_Id { get; set; }
        public int EnemyID { get; internal set; }
        public int iterations = 100_000;
        public Random rand = new Random();
        static int simulationsCount = 0;
        static int MaxDepth = 0;
        public async Task<Node> SearchAsync(char[,] _board, int _timeout)
        {
            //Console.WriteLine("Searching started...");
            MaxDepth = 0;
            simulationsCount = 0;
            // timeout settings
            var _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;
            _tokenSource.CancelAfter(_timeout);

            simulationsCount = 0;
            // create root node
            Node root = new Node(_parent: null, _board: _board, AIBot_Id == 1 ? 2 : 1);

            // search iteration
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
                int score = Rollout(node.gameState);

                // backpropagation
                Backpropagate(node, score);
            }

            // DEBUG show statistics
            // foreach(var child in root.childrens)
            // {
            //     Console.WriteLine($"Value: {child.value}\tVisits: {child.visits}\tUCB1: {child.UCB1Score}");
            //     //child.board.DrawBoard();
            // }
            Console.WriteLine("max reached depth = "+MaxDepth+" / game simulations: "+simulationsCount);
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
        // select best node basing on UCB1 formula ( explorationConst )
        private Node GetBestMove(Node _node, int exploration)
        {
            float bestScore = float.NegativeInfinity; // -oo
            List<Node> bestMoves = new();
          
                double lnOftotalVisits = Math.Log(GetRoot(_node).visits);
  
            double explorationConst = 2;
            foreach (var child in _node.childrens)
            {
        
                double averageScorePerVisitCurrentNode = (child.value) / (double)child.visits;
                double UCBScore = averageScorePerVisitCurrentNode + (explorationConst * (Math.Sqrt(lnOftotalVisits / (double)child.visits)));
 
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
            return bestMoves[rand.Next(0, bestMoves.Count)];
        }

        private Node SelectNode(Node _node)
        {
          //  Console.WriteLine("# SELECTION");

            while (!_node.IsTerminated)
            {
                if (_node.isFullyExpanded)
                    _node = GetBestMove(_node, 2);
                else
                    return Expand(_node) ?? _node;
            }

            //Engine.DrawBoard(_node.gameState.boardArray);
            return _node;
        }

        public Node Expand(Node parent)
        {           
            List<int> possibleMovements = Engine.GetLegalMovesList(parent.gameState.boardArray);
            List<int> usedMovesInChilds = parent.childrens.Select(x => (int)x.gameState.latestMovement).ToList();
            usedMovesInChilds.ForEach(x => possibleMovements.Remove(x));

            if (possibleMovements.Count == 0)
            {
                parent.isFullyExpanded = true;
                return null;
            }

            Node newNode = new Node(parent, parent.gameState.boardArray, parent.gameState.latestPlayer);
            parent.childrens.Add(newNode);
            MaxDepth = newNode.level > MaxDepth?newNode.level:MaxDepth;
            int randomUniqueMovealongChildrens = possibleMovements[rand.Next(0, possibleMovements.Count)];

            Engine.MakeMove(randomUniqueMovealongChildrens, newNode.gameState.latestPlayer, newNode.gameState.boardArray);
            newNode.gameState.latestMovement = randomUniqueMovealongChildrens;
            // check if is terminated
            var result = Engine.IsGameEnded(newNode.gameState.boardArray);
            if (result.status)
            {
                if (result.winnerMark != null)
                {
                    // check if bot win match based on winner mark
                    int WINNER_ID = result.winnerMark == Engine.player1_mark ? 1 : 2;

                    if (WINNER_ID != AIBot_Id)
                    {
                        //Console.WriteLine("Przegrałbys tak czy siak, przeciwnik moze wygrac w nastepnym ruchu na 100%");
                        newNode.parent.IsTerminated = true;
                        //return parent;
                    }
                }
            }
           // Console.WriteLine("# EXPANSION");

            //Engine.DrawBoard(newNode.gameState.boardArray);
            return newNode;
        }

        private void Backpropagate(Node node, int score)
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

        private int Rollout(GameState game)
            {

                //zrobienie kopi -> nie dzialamy na orginale, to tylko symulacja
                 //SYMULACJA+
                // Console.WriteLine("# SIMULATION / ROLLOUT");
                 var COPYIED_GameState = new GameState(game.boardArray,game.latestMovement,game.latestPlayer);

                var result = Engine.Simulate(COPYIED_GameState.latestPlayer,COPYIED_GameState.boardArray);

                MCTS.simulationsCount++;
                if(result.status == true){
                   // Console.WriteLine("gra w całości rozegrana");
                    if(result.winnerMark != null)
                    {
                      //  Console.WriteLine("zwyciezca został wybrany");
                        var winnerID = result.winnerMark==Engine.player1_mark?1:2;
                        if(winnerID == AIBot_Id)
                        {
                            return 1;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else
                    {
                       // Console.WriteLine("Brak zwyciezcy = REMIS");
                        return 0;
                    }
                } 

                throw new Exception("nie powinno sie zdażyc ? bo gra nadal trwa");
            }

            // private int GetRandomMove(GameState game, int firstPlayer)
            // {
            //     var rand = new Random();
            //     game.latestPlayer = firstPlayer;
            //     return board.availableMoves[rand.Next(0, board.availableMoves.Count)];
            // }
        }
    }
