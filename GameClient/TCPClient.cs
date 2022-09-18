using System.Net.Sockets;
using System.Text;

class Program
{
    static TcpClient client = new TcpClient("127.0.0.1", 2460);

    public static void Main(string[] args)
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("Type your message to send to the server: ");
            string message = Console.ReadLine();

            WriteMessageToServer(message);

            string response = ReadResponseMessage();
            Console.WriteLine($"Response from server: {response}");
            if (response.ToLower().Equals("bye"))
            {
                running = false;
            }
        }
    }

    public static void WriteMessageToServer(string message)
    {
        var stream = new StreamWriter(client.GetStream(), Encoding.ASCII);
        {
            stream.WriteLine(message);
            stream.Flush();
        }
    }

    public static string ReadResponseMessage()
    {
        var stream = new StreamReader(client.GetStream(), Encoding.ASCII);
        {
            return stream.ReadLine();
        }
    }
}