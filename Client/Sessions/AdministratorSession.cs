using System.IO.Pipes;
using Client.RequestService;

namespace Client.Sessions;

public class AdministratorSession : Session
{
    public AdministratorSession(string name, UserType userType, string subaccount, NamedPipeClientStream client) : base(
        name, userType, subaccount, client)
    {
    }

    public override void Handle()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Uzytkownik: {0}", this.Name);
            Console.WriteLine("Wybierz polecenie:{0}1.) Dodaj ucznia do klasy{1}2.) Usun ucznia z klasy{2}3.) Wyloguj",
                Environment.NewLine, Environment.NewLine, Environment.NewLine);

            var command = Console.ReadLine();

            Console.Clear();


            switch (command)
            {
                case "1":
                    AddStudentToClass();
                    break;
                case "2":
                    DeleteStudentFromClass();
                    break;
                case "3":
                    Console.WriteLine("{0}Nastapi wylogowanie!", Environment.NewLine);
                    CancellationToken?.Cancel(false);
                    Console.ReadLine();
                    Console.Clear();
                    return;
                default:
                    Console.WriteLine("{0}Nie ma takiego polecenia!", Environment.NewLine);
                    Console.ReadLine();
                    Console.Clear();
                    break;
            }
        }
    }

    private void DeleteStudentFromClass()
    {
        var getListOfClassesRequest = new Request(RequestType.GetListOfClasses, new List<string>() { this.Subaccount });
        SendRequest(getListOfClassesRequest);

        var listOfClasses = GetResult<GetListOfClassesRequestResult>()?.Classes;


        Console.WriteLine("Klasy:");
        for (var i = 0; i < listOfClasses!.Count; i++)
        {
            Console.WriteLine("{0}.) {1}", i + 1, listOfClasses[i].Item2);
        }

        Console.WriteLine("{0}Wybrana klasa: ", Environment.NewLine);

        var chosenClassIndex = Convert.ToInt32(Console.ReadLine());

        while (chosenClassIndex < 1 || chosenClassIndex > listOfClasses.Count)
        {
            Console.WriteLine($"Bledna liczba!");

            Console.ReadLine();

            Console.Clear();
            Console.WriteLine(
                $"Wybierz operacje:{Environment.NewLine}1.) Wybierz klase ponownie{Environment.NewLine}Cokolwiek innego.) Anuluj{Environment.NewLine}");

            if (Convert.ToInt32(Console.ReadLine()) != 1)
            {
                Console.Clear();

                return;
            }


            Console.WriteLine("Klasa:");
            for (var i = 0; i < listOfClasses.Count; i++)
            {
                Console.WriteLine("{0}.) {1}", i + 1, listOfClasses[i].Item2);
            }

            Console.WriteLine($"{Environment.NewLine}Wybrana klasa: ");

            try
            {
                chosenClassIndex = Console.ReadLine().Number();
            }
            catch (FormatException)
            {
                return;
            }
        }


        Console.Clear();
        Console.WriteLine($"{Environment.NewLine}Wybrana klasa: {0}{Environment.NewLine}",
            listOfClasses[chosenClassIndex - 1].Item2);


        var getStudentsByClassRequest = new Request(RequestType.GetStudentsByClass,
            new List<string> { listOfClasses[chosenClassIndex - 1].Item1 });
        SendRequest(getStudentsByClassRequest);
        var listOfStudents = GetResult<GetStudentsByClassRequestResult>()?.Students;


        Console.WriteLine("Uczniowie:");
        for (var i = 0; i < listOfStudents!.Count; i++)
        {
            Console.WriteLine("{0}.) {1}", i + 1, listOfStudents[i].Item2);
        }

        int chosenStudentIndex;
        Console.WriteLine($"{Environment.NewLine}Wybrany uczen: ");

        try
        {
            chosenStudentIndex = Console.ReadLine().Number();
        }
        catch (FormatException)
        {
            return;
        }

        while (chosenStudentIndex < 1 || chosenStudentIndex > listOfStudents.Count)
        {
            Console.WriteLine("Bledna liczba!");

            Console.ReadLine();

            Console.Clear();
            Console.WriteLine(
                $"Wybierz operacje:{Environment.NewLine}1.) Wybierz ucznia ponownie{Environment.NewLine}Cokolwiek innego.) Anuluj{Environment.NewLine}");

            if (Convert.ToInt32(Console.ReadLine()) != 1)
            {
                Console.Clear();

                return;
            }

            try
            {
                if (Console.ReadLine().Number() != 1)
                {
                    Console.Clear();

                    return;
                }
            }
            catch (FormatException)
            {
                return;
            }


            Console.WriteLine("Uczniowie:");
            for (var i = 0; i < listOfStudents.Count; i++)
            {
                Console.WriteLine("{0}.) {1}", i + 1, listOfStudents[i].Item2);
            }

            Console.WriteLine($"{Environment.NewLine}Wybrany uczen: ");

            try
            {
                chosenStudentIndex = Console.ReadLine().Number();
            }
            catch (FormatException)
            {
                return;
            }
        }

        var deleteStudentFromClassRequest = new Request(RequestType.DeleteStudentFromClass,
            new List<string>() { listOfStudents[chosenStudentIndex - 1].Item1 });
        SendRequest(deleteStudentFromClassRequest);

        var deleteStudentFromClassRequestResult = GetResult<DeleteStudentFromClassRequestResult>();

        if (deleteStudentFromClassRequestResult!.Status == Status.Succeed)
        {
            Console.WriteLine("Uczen {0} zostal usuniety z systemu!", listOfStudents[chosenStudentIndex - 1].Item2);
        }

        Console.ReadLine();

        Console.Clear();
    }

    private void AddStudentToClass()
    {
        var getListOfClassRequest = new Request(RequestType.GetListOfClasses, new List<string>() { this.Subaccount });
        SendRequest(getListOfClassRequest);

        var listOfClasses = GetResult<GetListOfClassesRequestResult>()?.Classes;


        Console.WriteLine("Klasy:");
        for (var i = 0; i < listOfClasses!.Count; i++)
        {
            Console.WriteLine("{0}.) {1}", i + 1, listOfClasses[i].Item2);
        }

        Console.WriteLine($"{Environment.NewLine}Wybrana klasa: ");

        int chosenClassIndex;
        try
        {
            chosenClassIndex = Console.ReadLine().Number();
        }
        catch (FormatException)
        {
            return;
        }

        while (chosenClassIndex < 1 || chosenClassIndex > listOfClasses.Count)
        {
            Console.WriteLine("Bledna liczba!");

            Console.ReadLine();

            Console.Clear();
            Console.WriteLine(
                $"Wybierz operacje:{Environment.NewLine}1.) Wybierz klase ponownie{Environment.NewLine}Cokolwiek innego.) Anuluj{Environment.NewLine}");

            try
            {
                if (Console.ReadLine().Number() != 1)
                {
                    Console.Clear();

                    return;
                }
            }
            catch (FormatException)
            {
                return;
            }


            Console.WriteLine("Klasa:");
            for (var i = 0; i < listOfClasses.Count; i++)
            {
                Console.WriteLine("{0}.) {1}", i + 1, listOfClasses[i].Item2);
            }

            Console.WriteLine($"{Environment.NewLine}Wybrana klasa: ");

            try
            {
                chosenClassIndex = Console.ReadLine().Number();
            }
            catch (FormatException)
            {
                return;
            }
        }


        Console.Clear();
        Console.WriteLine("{0}Wybrana klasa: {1}{2}", Environment.NewLine, listOfClasses[chosenClassIndex - 1].Item2,
            Environment.NewLine);

        Console.WriteLine("Podaj imie nowego ucznia: ");

        var name = Console.ReadLine();
        while (String.IsNullOrEmpty(name))
        {
            Console.WriteLine("{0}Podaj POPRAWNE imie nowego ucznia: ", Environment.NewLine);

            name = Console.ReadLine();
        }


        Console.WriteLine("Podaj nazwisko nowego ucznia: ");

        var surname = Console.ReadLine();
        while (String.IsNullOrEmpty(surname))
        {
            Console.WriteLine("{0}Podaj POPRAWNE nazwisko nowego ucznia: ", Environment.NewLine);

            surname = Console.ReadLine();
        }


        Console.WriteLine("Podaj imie rodzica nowego ucznia: ");

        var parentName = Console.ReadLine();
        while (String.IsNullOrEmpty(parentName))
        {
            Console.WriteLine("{0}Podaj POPRAWNE imie rodzica nowego ucznia: ", Environment.NewLine);

            parentName = Console.ReadLine();
        }


        Console.WriteLine("Podaj nazwisko rodzica nowego ucznia: ");

        var parentSurname = Console.ReadLine();
        while (String.IsNullOrEmpty(parentSurname))
        {
            Console.WriteLine("{0}Podaj POPRAWNE nazwisko rodzica nowego ucznia: ", Environment.NewLine);

            parentSurname = Console.ReadLine();
        }

        var addStudentToClassRequest = new Request(RequestType.AddStudentToClass, new List<string>()
            { listOfClasses[chosenClassIndex - 1].Item1, name, surname, parentName, parentSurname });
        SendRequest(addStudentToClassRequest);

        var addStudentToClassRequestResult = GetResult<AddStudentToClassRequestResult>();
        if (addStudentToClassRequestResult!.Status == Status.Succeed)
        {
            Console.WriteLine("Uczen {0} {1} zostal dodany do klasy {2}!", name, surname,
                listOfClasses[chosenClassIndex - 1].Item2);
        }

        Console.ReadLine();

        Console.Clear();
    }
}