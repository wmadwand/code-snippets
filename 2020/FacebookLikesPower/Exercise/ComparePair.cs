using ExerciseGeneration;

namespace MiniGames.Games.FBLikesPower
{
    public enum PositionType
    {
        Left, Right, Both
    }

    public struct ComparePair
    {
        public Expression leftExpression;
        public Expression rightExpression;

        public ComparePair(Expression leftExpression, Expression rightExpression)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }

        public string GetCorrectSymbol()
        {
            string symbol;
            if (leftExpression.result < rightExpression.result)
            {
                symbol = "<"; 
            }
            else if (leftExpression.result > rightExpression.result)
            {
                symbol = ">"; 
            }
            else
            {
                symbol = "=";
            }

            return symbol;
        }

        public PositionType GetWinPosition()
        {
            PositionType positionType;
            if (leftExpression.result > rightExpression.result)
            {
                positionType = PositionType.Left;
            }
            else if (leftExpression.result < rightExpression.result)
            {
                positionType = PositionType.Right;
            }
            else
            {
                positionType = PositionType.Both;
            }

            return positionType;
        }
    }
}