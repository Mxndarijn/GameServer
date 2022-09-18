using System.Net.Sockets;
using ClientSide.VR;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;

namespace GameClient;

public class Question
{
    public int Id { get; }
    private string question;
    private Thread thread;
    private NetworkStream stream;
    private bool correct = false;
    private bool ended = false;
    public Question(JObject ob, NetworkStream stream)
    {
        Id = IntegerType.FromString(ob["data"]["question-id"].ToObject<string>());
        question = ob["data"]["question"].ToObject<string>();
        this.stream = stream;

        thread = new Thread(Run);
        thread.Start();

    }

    public void Run()
    {
        // while (true)
        // {
            Console.WriteLine($"Question: \n {question}");
            Console.WriteLine("Type your answer: ");
            string answer = Console.ReadLine();
            if (!ended)
            {
                DataCommunication.SendData(stream, (JsonFileReader.GetObjectAsString("Client\\Answer", new Dictionary<string, string>()
                {
                    {"_answer_", answer},
                    {"_questionId_", Id.ToString()}
                })));
            }
       // }
    }

    public void HandleResponse(JObject json)
    {
        if (json["data"]["answerResponse"].ToObject<string>().ToLower().Equals("correct"))
        {
            Console.WriteLine("You answered correctly!");
            correct = true;
        }
        else if (json["data"]["answerResponse"].ToObject<string>().ToLower().Equals("wrong"))
        {
            Console.WriteLine("Your answer was wrong. Try again!");
            thread = new Thread(Run);
            thread.Start();
        }
        else if (json["data"]["answerResponse"].ToObject<string>().ToLower().Equals("no-time"))
        {
            ended = true;
            if (!correct)
            {
                Console.WriteLine("Time's up!");
                try
                {
                    thread.Abort();
                }
                catch {}
            }
        }
    }
}