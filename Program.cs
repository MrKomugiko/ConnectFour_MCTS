using System.Linq;

namespace ConnectFour_MCTS
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            for(int it=0;it<20;it++)
            {

           
            Console.WriteLine($"-------------------GAME[{it}]-----------------------");


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
                if(GameOver.status == true) break;
                //----------------------------- [PLAYER 1 = 'O'] -----------------------------
                //Console.WriteLine("Player 1 ['O'] = BOT");
                // create working copy of current board

                var searching = await _mcts_1.SearchAsync(Game.board, _timeout: 250);
                //Statistics(searching);
                var botMove = searching.gameState.latestMovement ?? throw new Exception("nie ma ruchu ?");

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
                while(true)
                {
                    Console.WriteLine($"DOSTĘPNE RUCHY: {choices} : ");
                    if(Int32.TryParse(Console.ReadLine(),out playerMove) == false)
                    {
                        Console.WriteLine("Błąd:niedozwolony znak!");
                        continue;
                    }

                    if(availableMoves.Contains(playerMove) == false)
                    {
                        Console.WriteLine($"Błąd:[{playerMove}] jest niedozwolonym ruchem.");
                        continue;
                    }

                    break;
                }

                Game.MakeMove(playerMove,_mcts_1.EnemyID);
                Engine.DrawBoard(Game.board);
                Console.WriteLine();

            }
            Engine.DrawBoard(Game.board);
            Console.WriteLine();
       }
            Console.ReadLine();
        }

        private static void Statistics(Node searching)
        {
            foreach (var child in searching.parent.childrens)
            {
                Console.Write($"Value:{child.value} ({child.victoriesCount}/{child.visits}) [win/visit]");
                Engine.DrawBoard(child.gameState.boardArray);
            }
            Console.WriteLine("\n\n------------------------------------");
            Console.WriteLine($"liczba symulacji:{searching.parent.visits} (1000ms)");
            Console.WriteLine("------------------------------------");
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