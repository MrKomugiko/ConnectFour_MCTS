using ConnectFour_MCTS;

public class Engine{
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
    // public GameState SetMove(int selectedSlot, int playerID, char[,] _boardCopy = null)
    // {
    //     _boardCopy??=this.board;
    //     // find spot in seleccted slot
    //     for(int y=0;y<rows-1;y++)
    //     {
    //         if(_boardCopy[selectedSlot,y] == EMPTY){
    //             _boardCopy[selectedSlot,y] = playerID==1?player1_mark:player2_mark;
                
    //             break;
    //         }
    //     }
    //     return new GameState(_boardArray:_boardCopy,_latestMovement:selectedSlot,_latestPlayer:playerID);
    // }
    public void MakeMove(int selectedSlot, int playerID)
    {
        // find spot in seleccted slot
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
        // find spot in seleccted slot
        for(int y=0;y<=rows-1;y++)
        {
            if(_boardCopy[selectedSlot,y] == EMPTY){
                _boardCopy[selectedSlot,y] = playerID==1?player1_mark:player2_mark;
                break;
            }
        }
    }
    public static List<int> GetAvailableSlots(char[,] _boardCopy)
    {
        List<int> slots = new(); 
        for (int x = 0; x < columns; x++)
        {
            // check if space for select
            if(_boardCopy[x,rows-1] == EMPTY)
            {
                slots.Add(x);
            }
        }

        return slots;
    }
    public static void DrawBoard(char[,] _boardCopy)
    {
        var result = IsGameEnded(_boardCopy);
        if(result.winerPositions != null){
            List<string> winCoordinates = result.winerPositions.ToList();
       
            bool goodMark = false;
            for (int y = rows - 1; y >= 0; y--)
            {
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
                        Console.Write($" {(_boardCopy[x, y])}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write($" {(_boardCopy[x, y])}");
                    }
                }
                Console.WriteLine();
            }
        }
        else
        {
            for (int y = rows - 1; y >= 0; y--)
            {
                for (int x = 0; x < columns; x++)
                {
                    Console.Write($" {(_boardCopy[x, y])}");
                }
                Console.WriteLine();
            }
        }
        // Console.WriteLine("is game ended? = " + result.status);
        // Console.WriteLine("winner is = " + result.winnerMark);
    }

    internal static List<int> GetLegalMovesList(char[,] board)
    {
        // liczba kolumn
        List<int> legalMoves = new();
        Enumerable.Range(0,columns).ToList().ForEach(x=>{
            if(board[x,rows-1] == EMPTY)
            {
                legalMoves.Add(x);
            }
        });
        return legalMoves;
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
    public static (bool status, char? winnerMark, string[]? winerPositions) Simulate(int FirstPlayerID = 1, char[,] _boardCopy = null)
    {
        var rand = new Random();
        int playerTurn = FirstPlayerID;
        bool isGameEnded = false;
        (bool status, char? winnerMark, string[]? winerPositions) result = (false,null,null);
        //DrawBoard(_boardCopy);
        while(isGameEnded==false)
        {
            // 1. select number from 0 to (x-max)
                int[] slots = GetAvailableSlots(_boardCopy).ToArray();
                if(slots.Length == 0){
                  //  Console.WriteLine("emmm?");
                    return (true, null, null); // remis
                }

                int selectedSlot = slots[rand.Next(0,slots.Length)];
                //Console.WriteLine($"Move selected:[{selectedSlot}]");
                playerTurn = playerTurn==1?2:1;
                MakeMove(selectedSlot,playerID:playerTurn,_boardCopy);
           
                result = IsGameEnded(_boardCopy);
                isGameEnded = result.status;
                // zmiana gracza
           
        }
        //DrawBoard(_boardCopy);
        return result;
    }
    public static (bool status, char? winnerMark, string[]? winerPositions) IsGameEnded(char[,] board)
    {
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
        var max_X_scope = board_x_size-3;
        var max_Y_scope = board_y_size-3;
        char EMPTY= Char.Parse("-");
        char checkingCharacter = Char.Parse("-");
        int charactersinrow = 0;
        char currentCheckingCharacter = EMPTY;

        Enumerable.Range(0,max_Y_scope).ToList().ForEach(y=>
        {
            if(charactersinrow == 4) return;
            Enumerable.Range(0,max_X_scope).ToList().ForEach(x=>
            {
                if(charactersinrow == 4) return;
                if(board[x,y] == EMPTY) return;
                currentCheckingCharacter = board[x,y];
                charactersinrow = 0;
                Enumerable.Range(0,4).ToList().ForEach(i=>{
               // Console.Write($"[{x+i},{y+i}] ");
                    if(board[x+i,y+i] == currentCheckingCharacter)
                    {
                        winerPositionsStringCode[charactersinrow] = $"{x+i}&{y+i}";
                        charactersinrow++;
                    }
                    else return;
                });
            });
        });

        if(charactersinrow == 4) return (status:true,winnerMark:currentCheckingCharacter, winerPositionsStringCode);

        #endregion

        #region Sprawdzanie na przekątnych malejąco
        /*
                -   -   -   -   -   
                X   -   -   -   -   
                -   X   -   -   -   
                -   -   X   -   -   
                -   -   -   X   -   
            */        
        currentCheckingCharacter = EMPTY;
        checkingCharacter = Char.Parse("-");
        charactersinrow = 0;
        Enumerable.Range(3,max_Y_scope).ToList().ForEach(y=>
        {
            if(charactersinrow == 4) return;
            Enumerable.Range(0,max_X_scope).ToList().ForEach(x=>
            {
                if(charactersinrow == 4) return;
                if(board[x,y] == EMPTY) return;
                currentCheckingCharacter = board[x,y];
                charactersinrow = 0;
                Enumerable.Range(0,4).ToList().ForEach(i=>
                {
                    //Console.Write($"[{x+i},{y-i}] ");
                    if(board[x+i,y-i] == currentCheckingCharacter)
                    {
                        winerPositionsStringCode[charactersinrow] = $"{x+i}&{y-i}";
                        charactersinrow++;
                    }
                    else return;
                });
            });
        });

        if(charactersinrow == 4) return (status:true,winnerMark:currentCheckingCharacter, winerPositionsStringCode);

        #endregion
       
        #region Sprawdzanie pionowe
        
          /*
            -   -   -   -   -   
            -   X   -   -   - 
            -   X   -   -   - 
            -   X   -   -   - 
            -   X   -   -   - 
            */

        for(int i = 0; i<board_x_size;i++)
        {
            checkingCharacter = Char.Parse("-");
            currentCheckingCharacter = Char.Parse("-");

            charactersinrow = 1;
            for(int j = 0; j<board_y_size;j++)
            {
                currentCheckingCharacter = board[i,j];
                if(currentCheckingCharacter == checkingCharacter)
                {
                    if(checkingCharacter != Char.Parse("-"))   
                    {
                        winerPositionsStringCode[charactersinrow] = $"{i}&{j}";
                        charactersinrow ++;
                       // Console.WriteLine(winerPositionsStringCode[charactersinrow-1]);
                    }

                    if(charactersinrow == 4)
                    {
                        return (status:true,winnerMark:currentCheckingCharacter, winerPositionsStringCode);
                    }   
                }
                else
                {       
                    checkingCharacter = currentCheckingCharacter;
                    charactersinrow = 1; 
                    winerPositionsStringCode[0] = $"{i}&{j}";
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
        for(int i = 0; i<board_y_size;i++)
        {
            checkingCharacter = Char.Parse("-");
            currentCheckingCharacter = Char.Parse("-");

            charactersinrow = 1;
            for(int j = 0; j<board_x_size;j++)
            {
                currentCheckingCharacter = board[j,i];
                if(currentCheckingCharacter == checkingCharacter)
                {
                    if(checkingCharacter != Char.Parse("-"))   
                    {
                        winerPositionsStringCode[charactersinrow] = $"{j}&{i}";
                        charactersinrow ++;
                       // Console.WriteLine(winerPositionsStringCode[charactersinrow-1]);
                    }

                    if(charactersinrow == 4)
                    {
                        return (status:true,winnerMark:currentCheckingCharacter, winerPositionsStringCode);
                    }   
                }
                else
                {       
                    checkingCharacter = currentCheckingCharacter;
                    charactersinrow = 1; 
                    winerPositionsStringCode[0] = $"{j}&{i}";
                }
            }
        }
        #endregion
        // checking if there is any left space on board
        foreach(var el in board)
        {
            if(el.ToString() == "-")
            {
                return (status:false,winnerMark:null,null);
            }
        }
      
        return (status:true,winnerMark:null,null);
    }
}