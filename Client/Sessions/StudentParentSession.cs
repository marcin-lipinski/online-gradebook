using System.IO.Pipes;
using Client.RequestService;

namespace Client.Sessions;

public class StudentParentSession : Session
{
    public StudentParentSession(string name, UserType userType, string subaccount, NamedPipeClientStream client) : base(
        name, userType, subaccount, client)
    {
    }

    public override void Handle()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Uzytkownik: {0}", this.Name);
            Console.WriteLine(
                $"Wybierz polecenie:{Environment.NewLine}1.) Wyswietl oceny ucznia{Environment.NewLine}2.) Wyloguj");
            var polecenie = Console.ReadLine();

            switch (polecenie)
            {
                case "1":
                    GetStudentGrades();
                    break;
                case "2":
                    Console.WriteLine($"{Environment.NewLine}Nastapi wylogowanie!");
                    CancellationToken!.Cancel(false);
                    Console.ReadLine();
                    Console.Clear();
                    return;
                default:
                    Console.WriteLine($"{Environment.NewLine}Nie ma takiego polecenia!");
                    Console.ReadLine();
                    Console.Clear();
                    break;
            }
        }
    }

    private void GetStudentGrades()
    {
        var getStudentGradesRequest = new Request(RequestType.GetStudentGrades, new List<string>() { Subaccount });
        SendRequest(getStudentGradesRequest);
        var result = GetResult<GetStudentGradesRequestResult>();
        
        Console.Clear();
        Console.WriteLine($"Uczen: " + result!.StudentName + Environment.NewLine);

        foreach (var subject in result.Grades!)
        {
            Console.Write(subject.Item1 + ":");
            foreach (var grade in subject.Item2)
            {
                Console.Write(" " + grade);
            }

            Console.WriteLine();
        }

        Console.Write($"{Environment.NewLine}Wcisnij dowolny przycisk, aby wrocic do menu.");
        Console.ReadKey();
    }
}