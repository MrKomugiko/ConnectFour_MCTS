namespace ConnectFour_MCTS
{
    public class Node
    {
        public Node parent { get; set; }
        public List<Node> childrens = new List<Node>();
        public GameState gameState {get;set;}
        
        public int level { get; set; } = 0;
        public double value { get; set; } = 0;

        public int victoriesCount { get; set; } = 0;
        public int drawsCount { get; set; } = 0;

        public int losesCount { get; set; } = 0;
        public int visits { get; set; } = 0;
        private bool loseGuaranteed;

        public bool isFullyExpanded { get; set; } = false;// no more available moves to add new
        public float UCB1Score { get; internal set; }
        public bool IsTerminated
        {
            get
            {
                if (loseGuaranteed == true)
                    return true;
                    
                if(parent != null)
                {
                    if (Engine.IsGameEnded(gameState.boardArray).status)
                    {
                        return true;
                    }
                }
                return false;
            }
            set {
                loseGuaranteed = value;
            }
        }
        public Node(Node _parent, char[,] _board, int _whoMakeTurnId)
        {
            if (_parent != null)
            {
                this.gameState = new GameState(_board,_parent.gameState.latestMovement,_whoMakeTurnId==1?2:1);
                this.parent = _parent;
                this.level = parent.level + 1;
            }
            else
            {
                this.gameState = new GameState(_board,null,_whoMakeTurnId);
            }
        }
    }
}