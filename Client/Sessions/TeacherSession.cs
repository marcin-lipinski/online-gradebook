using System.IO.Pipes;
using Client.RequestService;

namespace Client.Sessions;

public class TeacherSession : Session
{
    public TeacherSession(string name, UserType userType, string subaccount, NamedPipeClientStream client) : base(name,
        userType, subaccount, client)
    {
    }

    public override void Handle()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Uzytkownik: {0}", Name);
            Console.WriteLine(
                $"Wybierz polecenie:{Environment.NewLine}1.) Wyswietl oceny klasy{Environment.NewLine}2.) Wyloguj");

            switch (Console.ReadLine())
            {
                case "1":
                    StartBrowsing();
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

    private void StartBrowsing()
    {
        int chosenClassIndex, chosenSubjectIndex, chosenStudentIndex;

        var listOfClasses = GetListOfTaughtClasses();
        if (listOfClasses == null) return;
        if ((chosenClassIndex = ChooseClass(listOfClasses)) == -1) return;

        var listOfSubjects = listOfClasses[chosenClassIndex - 1].Item3;
        if ((chosenSubjectIndex = ChooseSubject(listOfSubjects)) == -1) return;

        var studentsGrades = GetStudentAndGrades(listOfClasses[chosenClassIndex - 1].Item1,
            listOfSubjects[chosenSubjectIndex - 1].Item1);
        if ((chosenStudentIndex = ChooseStudent(listOfClasses[chosenClassIndex - 1].Item2,
                listOfSubjects[chosenSubjectIndex - 1].Item2, studentsGrades)) == -1) return;

        var grades = studentsGrades?[chosenStudentIndex - 1].Item4;
        ProccedOperation(studentsGrades?[chosenStudentIndex - 1], grades, listOfSubjects[chosenSubjectIndex - 1].Item1);
    }

    private List<Tuple<string, string, List<Tuple<string, string>>>>? GetListOfTaughtClasses()
    {
        var getListOfClassRequest = new Request(RequestType.GetListOfClasses, new List<string>() { this.Subaccount });
        SendRequest(getListOfClassRequest);
        return GetResult<GetListOfClassesRequestResult>()?.Classes;
    }

    private List<Tuple<string, string, string, List<Tuple<string, string>>>>? GetStudentAndGrades(string schoolClass,
        string subject)
    {
        var studentsGradesRequest = new Request(RequestType.GetStudentsAndTheirGradesByClassAndSubject,
            new List<string> { schoolClass, subject });
        SendRequest(studentsGradesRequest);
        return GetResult<GetStudentsAndTheirGradesByClassAndSubjectRequestResult>()?.StudentsAndTheirGrades;
    }

    private int ChooseClass(List<Tuple<string, string, List<Tuple<string, string>>>>? listOfClasses)
    {
        int chosenClassIndex;

        Console.WriteLine("Prowadzone klasy:");
        for (var i = 0; i < listOfClasses!.Count; i++)
        {
            Console.WriteLine("{0}.) {1}", i + 1, listOfClasses[i].Item2);
        }

        Console.WriteLine("Wprowadz numer klasy (inny numer spowoduje powrot do menu)");

        try
        {
            chosenClassIndex = Console.ReadLine().Number();
        }
        catch (FormatException)
        {
            return -1;
        }

        Console.Clear();

        if (chosenClassIndex < 1 || chosenClassIndex > listOfClasses.Count) return -1;
        return chosenClassIndex;
    }

    private int ChooseSubject(List<Tuple<string, string>>? listOfSubjects)
    {
        int chosenSubjectIndex;

        Console.WriteLine("Prowadzone przedmioty:");
        if (listOfSubjects != null)
            for (var i = 0; i < listOfSubjects.Count; i++)
            {
                Console.WriteLine("{0}.) {1}", i + 1, listOfSubjects[i].Item2);
            }

        Console.WriteLine("Wprowadz numer przedmiotu (inny numer spowoduje powrot do menu)");

        try
        {
            chosenSubjectIndex = Console.ReadLine().Number();
        }
        catch (FormatException)
        {
            return -1;
        }

        Console.Clear();

        if (chosenSubjectIndex < 1 || chosenSubjectIndex > listOfSubjects!.Count) return -1;
        return chosenSubjectIndex;
    }

    private int ChooseStudent(string schoolClass, string subject,
        List<Tuple<string, string, string, List<Tuple<string, string>>>>? studentsGrades)
    {
        int chosenStudentIndex;

        Console.WriteLine(schoolClass + " - " + subject + ": ");
        for (var i = 0; i < studentsGrades!.Count; i++)
        {
            Console.Write((i + 1) + ". " + studentsGrades[i].Item2 + " " + studentsGrades[i].Item3);
            studentsGrades[i].Item4.ForEach(item => Console.Write(" " + item.Item2));
            Console.WriteLine();
        }

        Console.WriteLine("Wprowadz numer ucznia (inny numer spowoduje powrot do menu)");

        try
        {
            chosenStudentIndex = Console.ReadLine().Number();
        }
        catch (FormatException)
        {
            return -1;
        }

        Console.Clear();

        if (chosenStudentIndex < 1 || chosenStudentIndex > studentsGrades.Count) return -1;
        return chosenStudentIndex;
    }

    private void ProccedOperation(Tuple<string, string, string, List<Tuple<string, string>>>? student,
        List<Tuple<string, string>>? grades, string subjectId)
    {
        if (student == null || grades == null) return;
        var gradeOption = -1;
        while (gradeOption < 1 || gradeOption > 3)
        {
            Console.WriteLine("Oceny ucznia: " + student.Item2 + " " + student.Item3);
            for (var i = 0; i < grades.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + grades[i].Item2);
            }

            Console.WriteLine(
                $"Co chcesz zrobic{Environment.NewLine}1. Dodaj ocenę{Environment.NewLine}2. Usun ocene{Environment.NewLine}3. Wroc do menu");

            try
            {
                gradeOption = Console.ReadLine().Number();
            }
            catch (FormatException)
            {
                return;
            }


            Console.Clear();
        }

        switch (gradeOption)
        {
            case 1:
                AddGrade(student, subjectId);
                break;
            case 2:
                DeleteGrade(student);
                break;
            case 3:
                break;
        }
    }

    private void AddGrade(Tuple<string, string, string, List<Tuple<string, string>>> student, string subject)
    {
        int chosenGrade;

        Console.Clear();
        Console.WriteLine("Wpisz ocene z zakresu 1-6 aby ja dodac (wybor innej liczby spowoduje powrot do menu): ");

        try
        {
            chosenGrade = Console.ReadLine().Number();
        }
        catch (FormatException)
        {
            return;
        }

        if (chosenGrade < 1 || chosenGrade > 6) return;

        var addGradeRequest = new Request(RequestType.AddGrade,
            new List<string>() { student.Item1, subject, chosenGrade.ToString() });
        SendRequest(addGradeRequest);
        var addGradeRequestResult = GetResult<AddGradeRequestResult>();

        if (addGradeRequestResult!.Status == Status.Succeed)
        {
            Console.WriteLine("Ocena dodana!");
        }
    }

    private void DeleteGrade(Tuple<string, string, string, List<Tuple<string, string>>> student)
    {
        int chosenGrade = 0;

        while (chosenGrade < 1 || chosenGrade > student.Item4.Count)
        {
            for (var i = 0; i < student.Item4.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + student.Item4[i].Item2);
            }

            Console.WriteLine("Wybierz ocene do usuniecia (wybor innej opcji spowoduje powrot do menu): ");

            try
            {
                chosenGrade = Console.ReadLine().Number();
            }
            catch (FormatException)
            {
                return;
            }

            if (chosenGrade < 1 || chosenGrade > student.Item4.Count)
            {
                Console.WriteLine("Niepoprawny indeks oceny!");

                Console.ReadLine();

                Console.Clear();
            }
        }

        var deleteGradeRequest = new Request(RequestType.DeleteGrade,
            new List<string> { student.Item4[chosenGrade - 1].Item1 });
        SendRequest(deleteGradeRequest);
        var deleteGradeRequestResult = GetResult<DeleteGradeRequestResult>();

        if (deleteGradeRequestResult!.Status == Status.Succeed)
        {
            Console.WriteLine("Ocena usunieta!");
        }
    }
}