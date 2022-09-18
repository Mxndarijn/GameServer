using ClientSide.VR;
using SharedGameLogic.GameData.Operators;

namespace SharedGameLogic.GameData;

public class Question
{
    public string Message;
    public List<User> CorrectUsers;
    private int value1;
    private int value2;
    private int answer;
    private static List<Operator> _operators = new()
    {
        new Add(),
        new Mod(),
        new Multiply(),
        new Subtract()
    }; 
    public Question()
    {
        CorrectUsers = new();
        Random r = new Random();
        value1 = r.Next(3, 50);
        value2 = r.Next(3, 50);
        Operator op = _operators[r.Next(1, 5)];
        answer = op.calculate(value1, value2);
        Message = $"{value1} {op.getOperator()} {value2}";
    }

    public bool CheckAnswer(User u, int a)
    {
        if (a == answer)
        {
            CorrectUsers.Add(u);
        }
        return a == answer;
    }

    public string GetMessageToJson()
    {
        return JsonFileReader.GetObjectAsString("Server\\Question", new Dictionary<string, string>()
        {
            { "_question_", Message }
        });
    }
}