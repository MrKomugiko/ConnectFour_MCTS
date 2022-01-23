using System.Linq;

namespace ConnectFour_MCTS
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Engine.player1_mark = Char.Parse("O");
            Engine.player2_mark = Char.Parse("X");
            Engine.columns = 7;
            Engine.rows = 6;
            var Game = new Engine();

            MCTS _mcts_1 = new MCTS();
            _mcts_1.AIBot_Id = 1;
            _mcts_1.EnemyID = 2;

            // MCTS _mcts_2 = new MCTS();
            // _mcts_2.AIBot_Id = 2;
            // _mcts_2.EnemyID = 1;

            (bool status, char? winnerMark, string[]? winerPositions) GameOver = (false, null, null);
            // Engine.MakeMove(3,2,Game.board);
            while(GameOver.status == false)
            {
                
                //----------------------------- [PLAYER 1 = 'O'] -----------------------------
                //Console.WriteLine("Player 1 ['O'] = BOT");
                // create working copy of current board
                int searching_timeout = 1000;
                
                var searching = await _mcts_1.SearchAsync(Game.board, _timeout: searching_timeout);
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Statistics(searching,searching_timeout);
                var botMove = searching.gameState.latestMovement ?? throw new Exception("nie ma ruchu ?");
                Console.ResetColor();

                // wykonanie ruchu przez bota
                Game.MakeMove(botMove, _mcts_1.AIBot_Id);
                Engine.DrawBoard(Game.board);
                GameOver = Engine.IsGameEnded(Game.board);
                if(GameOver.status == true) break;
                
            //    //----------------------------- [PLAYER 2 = 'X'] -----------------------------
            //     //Console.WriteLine("Player 2 ['X'] = BOT");
            //     searching = await _mcts_2.SearchAsync(Game.board, _timeout: 250);
            //     //Statistics(searching);
            //     botMove = searching.gameState.latestMovement ?? throw new Exception("nie ma ruchu ?");
            //     Game.MakeMove(botMove, _mcts_2.AIBot_Id);
            //     //Engine.DrawBoard(Game.board);
            //     GameOver = Engine.IsGameEnded(Game.board);
                
                string choices ="";
                List<int> availableMoves = Engine.GetLegalMovesList(Game.board);
                availableMoves.ForEach(x=>choices+=$"[{x}] ");
                int playerMove = -1;
                Console.WriteLine($"DOSTĘPNE RUCHY: \n{choices}: ");
                while(true)
                {
                    if(Int32.TryParse(Console.ReadLine(),out playerMove) == false)
                    {
                        Console.WriteLine("Błąd:niedozwolony znak!");
                        continue;
                    }
                    if(playerMove == 666)
                    {
                        playerMove = await Call_AI_HintAsync(currentState:Game.board, yourID:_mcts_1.EnemyID,searching_timeout);
                        continue;
                    }else if(playerMove == 112)
                    {
                        playerMove = await Call_AI_HintAsync(currentState:Game.board, yourID:_mcts_1.EnemyID,searching_timeout,true);
                        continue;
                    }
                    if(availableMoves.Contains(playerMove) == false && playerMove!=666)
                    {
                        Console.WriteLine($"Błąd:[{playerMove}] jest niedozwolonym ruchem.");
                        continue;
                    }

                    break;
                }

                Game.MakeMove(playerMove,_mcts_1.EnemyID);
                Engine.DrawBoard(Game.board);
                Console.WriteLine();
                GameOver = Engine.IsGameEnded(Game.board);
                if(GameOver.status == true) break;

            }

            Console.WriteLine();
            Console.WriteLine("Press 'x' to exit.");
            while(true)
            {
                string action = Console.ReadLine();
                if(action.ToLower() == "x"){
                    break; // EXIT
                }
            }
        }
       

        private static async Task<int> Call_AI_HintAsync(char[,] currentState, int yourID, int timeout, bool isExtended = false)
        {
            var copyBoard = (char[,])currentState.Clone();
            MCTS _mcts_2 = new MCTS();
            _mcts_2.AIBot_Id = yourID;
            _mcts_2.EnemyID = yourID == 1 ? 2 : 1;

            var searching = await _mcts_2.SearchAsync(copyBoard, timeout);
            var botMove = searching.gameState.latestMovement ?? throw new Exception("nie ma ruchu ?");
            ShowPickProbablity(copyBoard, searching, botMove, full:isExtended);
            return botMove;
        }

        private static void ShowPickProbablity(char[,] copyBoard, Node searching, int botMove, bool full)
        {
            var availableMoves = Engine.GetLegalMovesList(copyBoard);
            var totalVisits = searching.parent.visits;

            double probablity = 0.0;
            if(full)
                {
                     /*
                    CODE: 112

                    ╔═══════════════════════════════════╗
                    ║        Which slot choose::        ║
                    ║       ....      |   1 [~ 8,8%]    ║
                    ║    2 [~ 8,8%]   |   3 [~ 8,8%]    ║
                    ║    4 [~ 8,8%]   |   5 [~ 8,8%]    ║
                    ║    6 [~ 8,8%]   |      ....       ║
                    ╚═══════════════════════════════════╝

                */
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("║        Which slot choose::        ║");
                Enumerable.Range(0,Engine.columns%2==0?Engine.columns:Engine.columns+1).ToList().ForEach(x =>
                {   
                    if(x%2==0)
                    {
                        // LEWA STRONA
                        if(searching.parent.childrens.Any(child=>child.gameState.latestMovement == x))
                        {
                            probablity = (((double)(searching.parent.childrens.First(child => child.gameState.latestMovement == x).visits) / totalVisits) * 100);
                            
                            if(botMove == x)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                Console.Write($"║    ");
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write($"{x} [~{Math.Round(probablity, 1).ToString("0.0").PadLeft(4)}%]");
                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                Console.Write("   |");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                Console.Write($"║    {x} [~{Math.Round(probablity, 1).ToString("0.0").PadLeft(4)}%]   |");
                                Console.ResetColor();
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write($"║       ....      |");
                        }
                    }
                    else
                    {
                        // PRAWA STRONA
                        if(searching.parent.childrens.Any(child=>child.gameState.latestMovement == x))
                        {
                            probablity = (((double)(searching.parent.childrens.First(child => child.gameState.latestMovement == x).visits) / totalVisits) * 100);

                            if(botMove == x)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write($"    {x} [~{Math.Round(probablity, 1).ToString("0.0").PadLeft(4)}%]");
                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                Console.Write("   ║\n");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                Console.Write($"    {x} [~{Math.Round(probablity, 1).ToString("0.0").PadLeft(4)}%]   ║\n");
                                Console.ResetColor();
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write($"       ....      ║\n");
                        }
                    }
                });
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("╚═══════════════════════════════════╝");
                Console.ResetColor();
            }
            else
            {
                /*
                    CODE: 666

                    ╔═══════════════════════════════════╗
                    ║    Best option is: 4 [~ 8,8%]     ║
                    ╚═══════════════════════════════════╝

                */
                probablity = (((double)(searching.parent.childrens.First(child => child.gameState.latestMovement == botMove).visits) / totalVisits) * 100);
                      
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write($"║   Best option is: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{botMove} [~{Math.Round(probablity, 1).ToString("0.0").PadLeft(4)}%]");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("      ║\n");
                Console.WriteLine("╚═══════════════════════════════════╝");
                Console.ResetColor();
            }
        }

        private static void Statistics(Node searching, int searching_timeout)
        {
            // foreach (var child in searching.parent.childrens)
            // {
            //     Console.WriteLine($"Value:{child.value},\t({child.value+child.drawsCount}/{child.visits}),\tUCB1:{child.UCB1Score} ");
            //    // Engine.DrawBoard(child.gameState.boardArray);
            // }
            /*
            ╔═══════════════════════════════════╗
            Simulations:    25551 (3500ms)
            ╚═══════════════════════════════════╝
            */
            var simCount = searching.parent.visits;
            string intFormat = "";
            if(simCount >= 0 && simCount <= 999)
            {
                intFormat = "";
            }
            else if(simCount >= 1_000 && simCount <= 999_999)
            {
                intFormat = "0`000";
            }
            else if(simCount >= 1_000_000 && simCount <= 999_999_999)
            {
                intFormat = "0`000`000";
            }
            
            Console.WriteLine($"║  Simulations: {simCount.ToString(intFormat).PadLeft(10)} ({(searching_timeout+"ms)").ToString().PadRight(8)}║");
             Console.WriteLine("╚═══════════════════════════════════╝");
        }

        public static void TestingExamples(Engine Game)
        {
            char[,] board = Game.board;

            Console.WriteLine("empty board");
            Engine.DrawBoard(board);

            char[,] board_copy_1 = (char[,])board.Clone();
            board_copy_1[0, 1] = Char.Parse("O");
            board_copy_1[0, 2] = Char.Parse("O");
            board_copy_1[0, 3] = Char.Parse("O");
            board_copy_1[0, 4] = Char.Parse("O");

            Console.WriteLine("test 1:");
            Engine.DrawBoard(board_copy_1);

            //------------------------------------------------------------
            char[,] board_copy_2 = (char[,])board.Clone();
            board_copy_2[1, 1] = Char.Parse("O");
            board_copy_2[2, 1] = Char.Parse("O");
            board_copy_2[3, 1] = Char.Parse("O");
            board_copy_2[4, 1] = Char.Parse("O");

            Console.WriteLine("test 2:");
            Engine.DrawBoard(board_copy_2);

            //------------------------------------------------------------
            char[,] board_copy_3 = (char[,])board.Clone();
            board_copy_3[1, 1] = Char.Parse("O");
            board_copy_3[2, 2] = Char.Parse("O");
            board_copy_3[3, 3] = Char.Parse("O");
            board_copy_3[4, 4] = Char.Parse("O");

            Console.WriteLine("test 3:");
            Engine.DrawBoard(board_copy_3);

            //------------------------------------------------------------
            char[,] board_copy_4 = (char[,])board.Clone();
            board_copy_4[5, 2] = Char.Parse("O");
            board_copy_4[4, 3] = Char.Parse("O");
            board_copy_4[3, 4] = Char.Parse("O");
            board_copy_4[2, 5] = Char.Parse("O");

            Console.WriteLine("test 4:");
            Engine.DrawBoard(board_copy_4);

            //------------------------------------------------------------
            char[,] board_copy_5 = (char[,])board.Clone();
            board_copy_5[1, 0] = Char.Parse("O");
            board_copy_5[1, 1] = Char.Parse("X");
            board_copy_5[2, 0] = Char.Parse("X");
            board_copy_5[2, 1] = Char.Parse("X");
            board_copy_5[2, 2] = Char.Parse("X");
            board_copy_5[3, 0] = Char.Parse("O");
            board_copy_5[3, 1] = Char.Parse("X");
            board_copy_5[3, 2] = Char.Parse("O");
            board_copy_5[4, 0] = Char.Parse("X");
            board_copy_5[4, 1] = Char.Parse("O");
            board_copy_5[4, 2] = Char.Parse("O");
            board_copy_5[4, 3] = Char.Parse("O");
            board_copy_5[5, 0] = Char.Parse("X");
            board_copy_5[5, 1] = Char.Parse("O");
            board_copy_5[5, 2] = Char.Parse("X");
            board_copy_5[5, 3] = Char.Parse("X");
            board_copy_5[5, 4] = Char.Parse("O");
            board_copy_5[5, 5] = Char.Parse("X");
            board_copy_5[6, 0] = Char.Parse("O");
            board_copy_5[6, 1] = Char.Parse("X");
            board_copy_5[6, 2] = Char.Parse("X");
            board_copy_5[6, 3] = Char.Parse("O");
            board_copy_5[6, 4] = Char.Parse("X");
            board_copy_5[6, 5] = Char.Parse("O");

            Console.WriteLine("test 5:");
            Engine.DrawBoard(board_copy_5);

            //------------------------------------------------------------
            char[,] board_copy_6 = (char[,])board.Clone();
            board_copy_6[0, 0] = Char.Parse("O");
            board_copy_6[5, 5] = Char.Parse("X");
        }
    }
}