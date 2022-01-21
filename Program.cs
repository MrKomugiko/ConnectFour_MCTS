using System.Linq;
class Program {
    public static void Main(string[] args)
    {
        var Engine = new GameBoard(
                rows:6, 
                columns:7,
                player1_mark:Char.Parse("O"),
                player2_mark:Char.Parse("X")
            );

        char[,] board = Engine.board;
        
        Console.WriteLine("empty board");
        Engine.DrawBoard(board);

        char[,] board_copy_1 = (char[,])board.Clone();
        board_copy_1[0,1] = Char.Parse("O");
        board_copy_1[0,2] = Char.Parse("O");
        board_copy_1[0,3] = Char.Parse("O");
        board_copy_1[0,4] = Char.Parse("O");

        Console.WriteLine("test 1:");
        Engine.DrawBoard(board_copy_1);
        //------------------------------------------------------------
        char[,] board_copy_2 = (char[,])board.Clone();
        board_copy_2[1,1] = Char.Parse("O");
        board_copy_2[2,1] = Char.Parse("O");
        board_copy_2[3,1] = Char.Parse("O");
        board_copy_2[4,1] = Char.Parse("O");

        Console.WriteLine("test 2:");
        Engine.DrawBoard(board_copy_2);

 //------------------------------------------------------------
        char[,] board_copy_3 = (char[,])board.Clone();
        board_copy_3[1,1] = Char.Parse("O");
        board_copy_3[2,2] = Char.Parse("O");
        board_copy_3[3,3] = Char.Parse("O");
        board_copy_3[4,4] = Char.Parse("O");

        Console.WriteLine("test 3:");
        Engine.DrawBoard(board_copy_3);

 //------------------------------------------------------------
        char[,] board_copy_4 = (char[,])board.Clone();
        board_copy_4[5,2] = Char.Parse("O");
        board_copy_4[4,3] = Char.Parse("O");
        board_copy_4[3,4] = Char.Parse("O");
        board_copy_4[2,5] = Char.Parse("O");

        Console.WriteLine("test 4:");
        Engine.DrawBoard(board_copy_4);

 //------------------------------------------------------------
        char[,] board_copy_5 = (char[,])board.Clone();
        board_copy_5[1,0] = Char.Parse("O");
        board_copy_5[1,1] = Char.Parse("X");
        board_copy_5[2,0] = Char.Parse("X");
        board_copy_5[2,1] = Char.Parse("X");
        board_copy_5[2,2] = Char.Parse("X");
        board_copy_5[3,0] = Char.Parse("O");
        board_copy_5[3,1] = Char.Parse("X");
        board_copy_5[3,2] = Char.Parse("O");
        board_copy_5[4,0] = Char.Parse("X");
        board_copy_5[4,1] = Char.Parse("O");
        board_copy_5[4,2] = Char.Parse("O");
        board_copy_5[4,3] = Char.Parse("O");
        board_copy_5[5,0] = Char.Parse("X");
        board_copy_5[5,1] = Char.Parse("O");
        board_copy_5[5,2] = Char.Parse("X");
        board_copy_5[5,3] = Char.Parse("X");
        board_copy_5[5,4] = Char.Parse("O");
        board_copy_5[5,5] = Char.Parse("X");
        board_copy_5[6,0] = Char.Parse("O");
        board_copy_5[6,1] = Char.Parse("X");
        board_copy_5[6,2] = Char.Parse("X");
        board_copy_5[6,3] = Char.Parse("O");
        board_copy_5[6,4] = Char.Parse("X");
        board_copy_5[6,5] = Char.Parse("O");
        
        Console.WriteLine("test 5:");
        Engine.DrawBoard(board_copy_5);
        
        Console.ReadLine();
    }
}
