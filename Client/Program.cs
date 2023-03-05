using System.IO.Pipes;
using System.Text;
using System.Text.RegularExpressions;
using Client.RequestService;
using Newtonsoft.Json;
using Client.Sessions;

namespace Client
{
    class Program
    {
        static void Main()
        {
            var client = new NamedPipeClientStream("PipesOfPiece");
            client.Connect();
            client.ReadMode = PipeTransmissionMode.Byte;
            Session session = new EmptySession();

            var loginChecker = new Regex(@"\b(S|P|T|A)......\b");

            while (true)
            {
                var operation = 3;
                do
                {
                    Console.WriteLine(
                        $"Witaj Użytkowniku!{Environment.NewLine}{Environment.NewLine}Wybierz operacje:{Environment.NewLine}" +
                        $"1.) Zaloguj{Environment.NewLine}2.) Zamknij program{Environment.NewLine}Wybrana operacja: ");
                    try
                    {
                        operation = Convert.ToInt32(Console.ReadLine());
                    }
                    catch (FormatException)
                    {
                    }

                    while (!(operation == 1 || operation == 2))
                    {
                        Console.WriteLine("Bledny numer polecenia!");
                        Console.ReadLine();
                        Console.Clear();
                        Console.WriteLine(
                            $"Wybierz operacje:{Environment.NewLine}1.) Zaloguj{Environment.NewLine}2.) Zamknij program{Environment.NewLine}Wybrana operacja: ");
                        try
                        {
                            operation = Convert.ToInt32(Console.ReadLine());
                        }
                        catch (FormatException)
                        {
                        }
                    }

                    if (operation == 2)
                    {
                        var exitRequest = new Request(RequestType.ExitRequest,
                            new List<string>
                            {
                                Capacity = 0
                            });
                        SendRequest(exitRequest, client);
                        client.Close();
                        client.Dispose();
                        return;
                    }

                    Console.WriteLine($"{Environment.NewLine}Podaj login: ");
                    var userLogin = Console.ReadLine()!;

                    while (!loginChecker.IsMatch(userLogin))
                    {
                        Console.WriteLine($"{Environment.NewLine}Podaj POPRAWNY login: ");
                        userLogin = Console.ReadLine()!;
                    }

                    Console.WriteLine($"{Environment.NewLine}Podaj haslo: ");
                    var userPassword = Console.ReadLine()!;

                    while (String.IsNullOrEmpty(userPassword))
                    {
                        Console.WriteLine($"{Environment.NewLine}Podaj POPRAWNE haslo: ");
                        userPassword = Console.ReadLine()!;
                    }

                    Console.WriteLine($"{Environment.NewLine}----LOGOWANIE---{Environment.NewLine}");
                    Request logInReq = new Request(RequestType.LogIn, new List<string>() { userLogin, userPassword });
                    SendRequest(logInReq, client);

                    var logInRequestResult = GetResult<LogInRequestResult>(client)!;

                    if (logInRequestResult.Status == Status.Succeed)
                    {
                        session = logInRequestResult.UserType switch
                        {
                            UserType.Student => new StudentParentSession(logInRequestResult.Name, UserType.Student,
                                logInRequestResult.Subaccount, client),
                            UserType.Parent => new StudentParentSession(logInRequestResult.Name, UserType.Parent,
                                logInRequestResult.Subaccount, client),
                            UserType.Teacher => new TeacherSession(logInRequestResult.Name, UserType.Teacher,
                                logInRequestResult.Subaccount, client),
                            UserType.Administrator => new AdministratorSession(logInRequestResult.Name,
                                UserType.Administrator, logInRequestResult.Subaccount, client),
                            _ => new EmptySession()
                        };
                        Console.WriteLine("Witaj, {0}", logInRequestResult.Name);
                        Console.WriteLine("Witaj, {0}", logInRequestResult.UserType);
                    }
                    else
                    {
                        Console.WriteLine("Bledne dane logowania!");
                        Console.ReadLine();
                        Console.Clear();
                    }
                } while (session.Name == "Unknown");

                Console.WriteLine();
                Console.WriteLine(session.Name);
                Console.WriteLine(session.UserType.ToString());

                session.Handle();
                session = new EmptySession();
            }
        }

        private static void SendRequest(Request request, NamedPipeClientStream client)
        {
            var serialised = JsonConvert.SerializeObject(request);
            var messageBytes = Encoding.UTF8.GetBytes(serialised);
            if (messageBytes.Length % 10 == 0) Array.Resize(ref messageBytes, messageBytes.Length + 1);
            client.Write(messageBytes, 0, messageBytes.Length);
        }

        private static T? GetResult<T>(NamedPipeClientStream client)
        {
            int readBytes;
            var messageBuilder = new StringBuilder();
            do
            {
                var messageBuffer = new byte[10];
                readBytes = client.Read(messageBuffer, 0, messageBuffer.Length);
                messageBuilder.Append(Encoding.UTF8.GetString(messageBuffer));
            } while (readBytes == 10);

            return JsonConvert.DeserializeObject<T>(messageBuilder.ToString());
        }
    }
}