using ConnectFour_MCTS;
using System.Linq;
public class Engine
{
    public static int rows;
    public static int columns;
    public static readonly char EMPTY = Char.Parse("-");
    public static char player1_mark;
    public static  char player2_mark;
    public char[,] board {get;set;}
    public Engine()
    {
        board = NewClearBoard();
    }
    public void MakeMove(int selectedSlot, int playerID)
    {
        for(int y=0;y<=rows-1;y++)
        {
            if(this.board[selectedSlot,y] == EMPTY){
                this.board[selectedSlot,y] = playerID==1?player1_mark:player2_mark;
                break;
            }
        }
    }
    public static void MakeMove(int selectedSlot, int playerID, char[,] _boardCopy)
    {
        for(int y=0;y<=rows-1;y++)
        {
            if(_boardCopy[selectedSlot,y] == EMPTY){
                _boardCopy[selectedSlot,y] = playerID==1?player1_mark:player2_mark;
                break;
            }
        }
    }
    private static List<int> legalMoves = new();
    public static (bool status, char? winnerMark, string[]? winerPositions) DrawBoard(char[,] _boardCopy)
    {
        var result = IsGameEnded(_boardCopy);
        if(result.winerPositions != null){
            List<string> winCoordinates = result.winerPositions.ToList();
            bool goodMark = false;
            for (int y = rows - 1; y >= 0; y--)
            { 
                Console.Write("".PadLeft(8));
                for (int x = 0; x < columns; x++)
                {
                    goodMark = false;
                    foreach (var coord in winCoordinates) // => 1&2, 2&3, 3&4, 4&5
                    {
                        var _coord = coord.Split("&");  // => [1,2]
                        if (x.ToString() == _coord[0] && y.ToString() == _coord[1])
                        {
                            goodMark = true;
                            break;
                        }
                    }

                    if (goodMark)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"{(_boardCopy[x, y])}  ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write($"{(_boardCopy[x, y])}  ");
                    }
                }
                Console.WriteLine();
            }
        }
        else
        {
            for (int y = rows - 1; y >= 0; y--)
            {
                Console.Write("".PadLeft(8));
                for (int x = 0; x < columns; x++)
                {
                    Console.Write($"{(_boardCopy[x, y])}  ");
                }
                Console.WriteLine();
            }
        }
        return result;
    }
    internal static int[] GetLegalMovesList(char[,] board)
    {
       legalMoves.Clear();
        for (int x = 0; x < columns; x++)
        {
            if(board[x,rows-1] == EMPTY)
            {
                legalMoves.Add(x);
            }
        }

        return legalMoves.ToArray();
    }

    private char[,] NewClearBoard() 
    {
        char[,] board = new char[columns,rows];

        for(int x = 0; x<columns;x++){
            for(int y = rows-1; y>=0;y--){
                board[x,y] = Char.Parse("-");
            }
        }
        return board;
    } 
    public static Random rand = new Random();
    public static (bool status, char? winnerMark, string[]? winerPositions) Simulate(int NextPlayerWhoMakeMove, char[,] _board)
    {
        MCTS.SimulationsCount++;

        (bool status, char? winnerMark, string[]? winerPositions) result = (false,null,null);
        int[] slots;
        while(result.status==false)
        {
            slots = GetLegalMovesList(_board);
            if(slots.Length == 0){
                return (true, null, null); // remis
            }

            MakeMove(slots[rand.Next(0,slots.Length)],NextPlayerWhoMakeMove,_board);
            result = IsGameEnded(_board);
            // change player id
            NextPlayerWhoMakeMove= NextPlayerWhoMakeMove==1?2:1;
        }
        return result;
    }
    public static (bool status, char? winnerMark, string[]? winerPositions) IsGameEnded(char[,] board)
    {   
        // draw checking
        int[] slots = GetLegalMovesList(board);
                if(slots.Length == 0){
                  //  Console.WriteLine("emmm?");
                    return (true, null, null); // remis
                }
        
        int board_x_size = board.GetLength(0);
        int board_y_size = board.GetLength(1);
        string[] winerPositionsStringCode = new string[4];
        #region Sprawdzanie na przekątnych rosnących
         /*
                -   -   -   -   -   
                -   -   -   X   -  
                -   -   X   -   -  
                -   X   -   -   -  
                X   -   -   -   -    
            */

        for(int y = 0; y<board_x_size-4;y++){
            for(int x = 0; x<board_y_size-4;x++){
                if(board[x,y] == EMPTY) continue;

                if(board[x+1,y+1] == board[x,y] &&  board[x+2,y+2] == board[x,y] && board[x+3,y+3] == board[x,y])
                {
                    winerPositionsStringCode[0] = $"{x}&{y}";
                    winerPositionsStringCode[1] = $"{x+1}&{y+1}";
                    winerPositionsStringCode[2] = $"{x+2}&{y+2}";
                    winerPositionsStringCode[3] = $"{x+3}&{y+3}";

                    return (status:true,winnerMark:board[x,y], winerPositionsStringCode);
                }
            }
        }
     
        #endregion
        #region Sprawdzanie na przekątnych malejąco
        /*
                -   -   -   -   -   
                X   -   -   -   -   
                -   X   -   -   -   
                -   -   X   -   -   
                -   -   -   X   -   
            */        
        for(int y=3;y<board_y_size;y++){
            for(int x=0;x<board_x_size-4;x++){
                if(board[x,y] == EMPTY) continue;

                if(board[x+1,y-1] == board[x,y] &&  board[x+2,y-2] == board[x,y] && board[x+3,y-3] == board[x,y])
                {
                    winerPositionsStringCode[0] = $"{x}&{y}";
                    winerPositionsStringCode[1] = $"{x+1}&{y-1}";
                    winerPositionsStringCode[2] = $"{x+2}&{y-2}";
                    winerPositionsStringCode[3] = $"{x+3}&{y-3}";

                    return (status:true,winnerMark:board[x,y], winerPositionsStringCode);
                }
            }
        }

        #endregion
        #region Sprawdzanie pionowe
        
          /*
            -   -   -   -   -   
            -   X   -   -   - 
            -   X   -   -   - 
            -   X   -   -   - 
            -   X   -   -   - 
            */

         for(int y=3;y<board_y_size;y++){
            for(int x=0;x<board_x_size;x++){
                if(board[x,y] == EMPTY) continue;

                if(board[x,y-1] == board[x,y] &&  board[x,y-2] == board[x,y] && board[x,y-3] == board[x,y])
                {
                    winerPositionsStringCode[0] = $"{x}&{y}";
                    winerPositionsStringCode[1] = $"{x}&{y-1}";
                    winerPositionsStringCode[2] = $"{x}&{y-2}";
                    winerPositionsStringCode[3] = $"{x}&{y-3}";

                    return (status:true,winnerMark:board[x,y], winerPositionsStringCode);
                }
            }
        }
        #endregion
       
        #region Sprawdzanie poziome
        
          /*
            -   -   -   -   -   
            -   -   -   -   - 
            -   X   X   X   X 
            -   -   -   -   - 
            -   -   -   -   - 
            */
        for(int y=0;y<board_y_size;y++){
            for(int x=0;x<board_x_size-4;x++){
                if(board[x,y] == EMPTY) continue;

                if(board[x+1,y] == board[x,y] &&  board[x+2,y] == board[x,y] && board[x+3,y] == board[x,y])
                {
                    winerPositionsStringCode[0] = $"{x}&{y}";
                    winerPositionsStringCode[1] = $"{x+1}&{y}";
                    winerPositionsStringCode[2] = $"{x+2}&{y}";
                    winerPositionsStringCode[3] = $"{x+3}&{y}";

                    return (status:true,winnerMark:board[x,y], winerPositionsStringCode);
                }
            }
        }
        #endregion
        // checking if there is any left space on board
       
       for(int x=0,y=board_y_size-1; x<board_x_size; x++)
       {
            if(board[x,y] == EMPTY) return (status:false,winnerMark:null,null);
       }

        return (status:true,winnerMark:null,null);
    }
}