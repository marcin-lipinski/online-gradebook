using System.IO.Pipes;
using System.Text;
using Client.RequestService;
using Newtonsoft.Json;

namespace Client.Sessions;

public abstract class Session
{
    public string Name { get; set; }
    public UserType UserType { get; set; }
    protected readonly string Subaccount = null!;
    private NamedPipeClientStream _client = null!;
    protected readonly CancellationTokenSource? CancellationToken;

    protected Session(string name, UserType userType, string subaccount, NamedPipeClientStream client)
    {
        Name = name;
        UserType = userType;
        _client = client;
        Subaccount = subaccount;
        CancellationToken = new CancellationTokenSource();
        Task.Run(() => SessionClock(CancellationToken.Token));
    }

    protected Session()
    {
        Name = "Unknown";
    }

    public abstract void Handle();

    protected void SendRequest(Request request)
    {
        var serialised = JsonConvert.SerializeObject(request);
        var messageBytes = Encoding.UTF8.GetBytes(serialised);
        if (messageBytes.Length % 10 == 0) Array.Resize(ref messageBytes, messageBytes.Length + 1);
        _client.Write(messageBytes, 0, messageBytes.Length);
    }

    protected T? GetResult<T>()
    {
        int readBytes;
        var messageBuilder = new StringBuilder();
        do
        {
            var messageBuffer = new byte[10];
            readBytes = _client.Read(messageBuffer, 0, messageBuffer.Length);
            messageBuilder.Append(Encoding.UTF8.GetString(messageBuffer));
        } while (readBytes == 10);

        return JsonConvert.DeserializeObject<T>(messageBuilder.ToString());
    }

    private void SessionClock(CancellationToken cancelToken)
    {
        for (int time = 0;; time++)
        {
            if (cancelToken.IsCancellationRequested)
            {
                if (time >= 600)
                {
                    Console.WriteLine("Czas sesji: {0}m {1}s", time/600, (time%600)/10);
                }
                else
                {
                    Console.WriteLine("Czas sesji: {0}s", time/10);
                }
                return;
            }

            Thread.Sleep(100);
        }
    }
}

public static class SessionExtensions
{
    public static int Number(this string? input)
    {
        return Convert.ToInt32(input);
    }
}