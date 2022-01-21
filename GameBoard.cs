/*
/*

    
    przekatne:
        1. 0,2                  [x,y] = ... => x+1,y+1                 
        2. 0,1                            
        3. 0,0                                
        4. 1,0                                                    
        5. 2,0                                                                
        6. 3,0                                                                      
        
        7. 6,2                  [6-x,y] = ... => x-1, y+1                                                      
        8. 6,1                                                                     
        9. 6,0                                                          
       10. 5,0                                                         
       11. 4,0                                                            
       12. 3,0                                   

    Player_1 = 'O'
    Player_2 = 'X'

    5 | -   -   -   -   -   X   O   
    4 | -   -   -   -   -   O   X    
    3 | -   -   -   -   O   X   O    
    2 | -   -   X   O   O   X   X    
    1 | -   X   X   X   O   O   X  
    0 | -   O   X   O   X   X   O  
   ---|-----------------------------
      | 0   1   2   3   4   5   6  

        board[1,0] = Char.Parse("O");
        board[1,1] = Char.Parse("X");
        
        board[2,0] = Char.Parse("X");
        board[2,1] = Char.Parse("X");
        board[2,2] = Char.Parse("X");

        board[3,0] = Char.Parse("O");
        board[3,1] = Char.Parse("X");
        board[3,2] = Char.Parse("O");

        board[4,0] = Char.Parse("X");
        board[4,1] = Char.Parse("O");
        board[4,2] = Char.Parse("O");
        board[4,3] = Char.Parse("O");

        board[5,0] = Char.Parse("X");
        board[5,1] = Char.Parse("O");
        board[5,2] = Char.Parse("X");
        board[5,3] = Char.Parse("X");
        board[5,4] = Char.Parse("O");
        board[5,5] = Char.Parse("X");

        board[6,0] = Char.Parse("O");
        board[6,1] = Char.Parse("X");
        board[6,2] = Char.Parse("X");
        board[6,3] = Char.Parse("O");
        board[6,4] = Char.Parse("X");
        board[6,5] = Char.Parse("O");
*/
public class GameBoard{

    public readonly int rows;
    public readonly int columns;
    public char player1_mark {get;set;} 
    public char player2_mark {get;set;} 
    public char[,] board {get;}
    public GameBoard(int rows, int columns, char player1_mark, char player2_mark)
    {
        this.rows = rows;
        this.columns = columns;
        this.player1_mark = player1_mark;
        this.player2_mark = player2_mark;

        board = NewBoard();
    }
    public void DrawBoard(char[,] board)
    {
        var result = isGameEnded(board);
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
                        Console.Write($" {(board[x, y])}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write($" {(board[x, y])}");
                    }

                    //Console.Write($"\tx{x}.y{y}");
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
                    Console.Write($" {(board[x, y])}");
                }
                Console.WriteLine();
            }
        }
        // Console.WriteLine("is game ended? = " + result.status);
        // Console.WriteLine("winner is = " + result.winnerMark);
        // Console.WriteLine("winner`s connected at = " + String.Join("->", result.winerPositions).Replace("&", "."));
    }
    private char[,] NewBoard() 
    {
        char[,] board = new char[columns,rows];

        for(int x = 0; x<7;x++){
            for(int y = 5; y>=0;y--){
                board[x,y] = Char.Parse("-");
            }
        }
        for(int x = 0; x<7;x++){
            for(int y = 5; y>=0;y--){
                board[x,y] = Char.Parse("-");
            }
        }
        return board;
    } 

    static int[] diagonal_x = new int[6]{0,0,0,1,2,3};
    static int[] diagonal_y = new int[6]{2,1,0,0,0,0};
    public static (bool status, char? winnerMark, string[]? winerPositions) isGameEnded(char[,] board)
    {
        // diagonal checking for 4 same in row
        int board_x_size = board.GetLength(0);
        int board_y_size = board.GetLength(1);
        string[] winerPositionsStringCode = new string[4];

        #region Sprawdzanie na przekątnych
        int diagonalOptions = diagonal_y.GetLength(0);
        for(int dCase = 0; dCase<diagonalOptions; dCase++)
        {
             /*
                -   -   -   -   -   
                -   -   -   X   -  
                -   -   X   -   -  
                -   X   -   -   -  
                X   -   -   -   -    
            */
            char checkingCharacter = Char.Parse("-");
            char currentCheckingCharacter = Char.Parse("-");

            int charactersinrow = 1;
            for(int i = 0;i < board_x_size-1; i++)
            {
                if( diagonal_x[dCase]+i > board_x_size-1 || diagonal_y[dCase]+i > board_y_size-1 ) break;
                //Console.WriteLine($"1....{diagonal_x[dCase]+i},{diagonal_y[dCase]+i}");

                currentCheckingCharacter = board[diagonal_x[dCase]+i,diagonal_y[dCase]+i];
                if(currentCheckingCharacter == checkingCharacter)
                {
                    if(checkingCharacter != Char.Parse("-"))   
                    {
                        winerPositionsStringCode[charactersinrow] = $"{diagonal_x[dCase]+i}&{diagonal_y[dCase]+i}";
                        //Console.WriteLine("1..."+winerPositionsStringCode[charactersinrow]);
                        charactersinrow ++;
                    }

                    if(charactersinrow == 4)
                        return (status:true,winnerMark:currentCheckingCharacter, winerPositionsStringCode);
                }
                else
                {       
                    winerPositionsStringCode[0] = $"{diagonal_x[dCase]+i}&{diagonal_y[dCase]+i}";
                    checkingCharacter = currentCheckingCharacter;
                    charactersinrow = 1; 
                }
            }
            
            /*
                -   -   -   -   -   
                X   -   -   -   -   
                -   X   -   -   -   
                -   -   X   -   -   
                -   -   -   X   -   
            */
            checkingCharacter = Char.Parse("-");
            currentCheckingCharacter = Char.Parse("-");

            charactersinrow = 1;
            for(int i = 0;i < board_x_size-1; i++)
            {
                if( diagonal_x[dCase]+(board_x_size-1)-i > (board_x_size-1)  || diagonal_y[dCase]+i > board_y_size-1 ) break;
                //Console.WriteLine($"2....{diagonalMinus_x[dCase]-i},{diagonal_y[dCase]+i}");

                currentCheckingCharacter = board[diagonal_x[dCase]+(board_x_size-1)-i,diagonal_y[dCase]+i];
                if(currentCheckingCharacter == checkingCharacter)
                {
                    if(checkingCharacter != Char.Parse("-"))   
                    {
                        winerPositionsStringCode[charactersinrow] = $"{diagonal_x[dCase]+(board_x_size-1)-i}&{diagonal_y[dCase]+i}";
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
                    winerPositionsStringCode[0] = $"{diagonal_x[dCase]+(board_x_size-1)-i}&{diagonal_y[dCase]+i}";
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
        for(int i = 0; i<board_x_size;i++)
        {
            char checkingCharacter = Char.Parse("-");
            char currentCheckingCharacter = Char.Parse("-");

            int charactersinrow = 1;
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
            char checkingCharacter = Char.Parse("-");
            char currentCheckingCharacter = Char.Parse("-");

            int charactersinrow = 1;
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