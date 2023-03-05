using System.Text;
using System.Text.Json;

namespace Server;

public static class DataBaseClient
{
    private static readonly byte[] Bracket = Encoding.ASCII.GetBytes("\n]");
    private static readonly byte[] Coma = Encoding.ASCII.GetBytes(",\n  ");
    private const string DatabasePath = "../../../database";

    private static List<T> GetAll<T>()
    {
        var pathname = CheckTypeAndReturnPath(typeof(T).Name);
        var jsonString = File.ReadAllText(pathname!);
        return JsonSerializer.Deserialize<List<T>>(jsonString)!;
    }

    private static async void AddNewRecord<T>(T recordObject)
    {
        var text = JsonSerializer.SerializeToUtf8Bytes(recordObject);
        var pathname = CheckTypeAndReturnPath(typeof(T).Name);
        await using var fs = File.OpenWrite(pathname!);
        fs.Seek(-1, SeekOrigin.End);
        fs.Write(Coma, 0, Coma.Length);
        fs.Write(text, 0, text.Length);
        fs.Write(Bracket, 0, Bracket.Length);
        fs.SetLength(fs.Position);
    }

    private static string? CheckTypeAndReturnPath(string type)
    {
        return type switch
        {
            "Student" => $"{DatabasePath}/students.json",
            "Teacher" => $"{DatabasePath}/teachers.json",
            "Parent" => $"{DatabasePath}/parents.json",
            "SchoolClass" => $"{DatabasePath}/school_classes.json",
            "Grade" => $"{DatabasePath}/grades.json",
            "Admin" => $"{DatabasePath}/admin.json",
            "Subject" => $"{DatabasePath}/subjects.json",
            "Account" => $"{DatabasePath}/accounts.json",
            "ClassSubjectTeacher" => $"{DatabasePath}/school_classes_subjects_relations.json",
            _ => null
        };
    }

    public static Account? Authorization(string? login, string? password)
    {
        var account = GetAll<Account>().Where(item => item.Login.Equals(login) && item.Password.Equals(password))
            .ToList();
        return account.Count > 0 ? account[0] : null;
    }

    public static List<Tuple<string, string, List<Tuple<string, string>>>> GetTeacherClasses(string? teacherId)
    {
        var prevResult = (from cst in GetAll<ClassSubjectTeacher>()
            join sc in GetAll<SchoolClass>() on cst.SchoolClass equals sc.Id
            join sb in GetAll<Subject>() on cst.Subject equals sb.Id
            where cst.Teacher.Equals(teacherId)
            select (ID: sc.Id, name: sc.Name, (ID: sb.Id, name: sb.Name).ToTuple())).ToList();

        var result = (from res in prevResult
            group res.Item3 by new { res.ID, res.name }
            into gr
            select (gr.Key.ID, gr.Key.name, gr.ToList()).ToTuple()).ToList();

        return result;
    }

    public static string GetNameAndSurname(string? userId)
    {
        Person user = userId[0] switch
        {
            'S' => GetAll<Student>().First(item => item.Id.Equals(userId)),
            'T' => GetAll<Teacher>().First(item => item.Id.Equals(userId)),
            'P' => GetAll<Parent>().First(item => item.Id.Equals(userId)),
            'A' => GetAll<Admin>().First(item => item.Id.Equals(userId)),
            _ => new Person()
        };
        return user.Name + " " + user.Surname;
    }

    public static List<Tuple<string, string>> GetClassStudents(string? scId)
    {
        return (from student in GetAll<Student>()
            where student.SchoolClass.Equals(scId)
            select (ID: student.Id, student.Name + " " + student.Surname).ToTuple()).ToList();
    }

    public static List<Tuple<string, List<string>>>? GetStudentGrades(string? userId)
    {
        if (userId == null) return null;
        if (userId.Length > 0 && userId[0] == 'P') userId = GetStudentParent(userId);

        var subjects = GetAll<Subject>();
        var result = new List<Tuple<string?, List<string>>>();

        Parallel.ForEach(subjects, subject =>
        {
            var listOfGrades = (from grade in GetAll<Grade>()
                where userId.Equals(grade.Student) && subject.Id.Equals(grade.Subject)
                select grade.Value.ToString()).ToList();
            var record = new Tuple<string?, List<string>>(subject.Name, listOfGrades);
            result.Add(record);
        });

        result.Sort((tuple1, tuple2) => string.Compare(tuple1.Item1, tuple2.Item1, StringComparison.Ordinal));

        return result;
    }

    public static List<Tuple<string, string, string, List<Tuple<string, string>>>>
        GetStudentsAndTheirGradesFromSpecificSubject(string? klasaId, string? przedmiotId)
    {
        var students = (from student in GetAll<Student>()
            where student.SchoolClass.Equals(klasaId)
            select (ID: student.Id, name: student.Name, surname: student.Surname, new List<Tuple<string, string>>())
                .ToTuple()).ToList();

        //var grades = GetAll<Grade>();

        var tasks = new List<Task>();
        foreach (var task in students.Select(student => new Task(() =>
                 {
                     (from grade in GetAll<Grade>()
                             where student.Item1.Equals(grade.Student) && przedmiotId.Equals(grade.Subject)
                             select (ID: grade.Id, grade.Value.ToString()).ToTuple()).ToList()
                         .ForEach(item => student.Item4.Add(item));
                 })))
        {
            task.Start();
            tasks.Add(task);
        }

        Task.WaitAll(tasks.ToArray());
        return students;
    }

    public static string? GetStudentParent(string? parentId)
    {
        var res = (from parent in GetAll<Parent>()
            where parent.Id.Equals(parentId)
            select parent).ToArray();
        return res.Length > 0 ? res.First().Student : null;
    }

    public static void AddGrade(string? studentId, string? subjectId, string? value)
    {
        var grade = new Grade()
        {
            Id = "G" + (GetAll<Grade>().Count + 1).ToString(),
            Student = studentId,
            Subject = subjectId,
            Value = Convert.ToInt32(value)
        };
        AddNewRecord(grade);
    }

    public static void DeleteGrade(string? gradeId)
    {
        var grades = GetAll<Grade>();
        foreach (var grade in grades.Where(grade => grade.Id.Equals(gradeId)))
        {
            grades.Remove(grade);
            break;
        }

        var text = JsonSerializer.SerializeToUtf8Bytes(grades);
        var pathname = CheckTypeAndReturnPath(nameof(Grade));
        File.WriteAllBytes(pathname!, text);
    }

    public static List<Tuple<string, string, List<Tuple<string, string>>>> GetAllClasses()
    {
        var result = (from sc in GetAll<SchoolClass>()
            select (ID: sc.Id, name: sc.Name, new List<Tuple<string, string>>()).ToTuple()).ToList();
        return result;
    }

    public static void DeleteStudent(string? studentid)
    {
        var students = GetAll<Student>();
        foreach (var student in students.Where(grade => grade.Id.Equals(studentid)))
        {
            students.Remove(student);
            break;
        }

        var text = JsonSerializer.SerializeToUtf8Bytes(students);
        var pathname = CheckTypeAndReturnPath(nameof(Student));
        File.WriteAllBytes(pathname!, text);
    }

    public static void AddStudent(string? idKlasy, string? name, string? surname, string? parentName,
        string? parentSurname)
    {
        var student = new Student()
        {
            Id = "S" + (GetAll<Student>().Count + 1),
            Name = name,
            SchoolClass = idKlasy,
            Surname = surname
        };
        var parent = new Parent()
        {
            Id = "P" + (GetAll<Parent>().Count + 1),
            Name = parentName,
            Surname = parentSurname,
            Student = student.Id
        };
        AddNewRecord(student);
        AddNewRecord(parent);
    }
}