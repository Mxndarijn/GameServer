namespace SharedGameLogic.GameData.Operators;

public class Multiply : Operator
{
    public int calculate(int a, int b)
    {
        return a * b;
    }
    
    public string getOperator()
    {
        return "*";
    }
}