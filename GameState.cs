namespace ConnectFour_MCTS
{
    public class GameState
    {
        public readonly char[,] boardArray;
        public int? latestMovement;
        public int latestPlayer {get;set;}
        public GameState(char[,] _boardArray, int? _latestMovement, int _latestPlayer)
        {
            this.boardArray = (char[,])_boardArray.Clone();
            this.latestMovement = _latestMovement;
            this.latestPlayer = _latestPlayer;
        }
    }
}