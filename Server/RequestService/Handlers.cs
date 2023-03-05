namespace Server.RequestService;

public static class Handlers
{
    public static Result HandleLogInRequest(List<string?> @params)
    {
        var account = DataBaseClient.Authorization(@params[0], @params[1]);

        if (account == null)
        {
            return new LogInRequestResult(Status.Failed)
            {
                Name = "",
                Subaccount = "",
                UserType = UserType.Unknown
            };
        }

        return new LogInRequestResult(Status.Succeed)
        {
            Name = DataBaseClient.GetNameAndSurname(account.Subaccount),
            Subaccount = account.Subaccount,
            UserType = account.Subaccount[0] switch
            {
                'S' => UserType.Student,
                'T' => UserType.Teacher,
                'P' => UserType.Parent,
                'A' => UserType.Administrator,
                _ => UserType.Unknown
            }
        };
    }

    public static Result HandleGetStudentGradesRequest(List<string?> @params)
    {
        var grades = DataBaseClient.GetStudentGrades(@params[0]);
        return new GetStudentGradesRequestResult(grades!.Count == 0 ? Status.Failed : Status.Succeed)
        {
            Grades = grades,
            StudentName = @params[0][0] switch
            {
                'S' => DataBaseClient.GetNameAndSurname(@params[0]),
                'P' => DataBaseClient.GetNameAndSurname(DataBaseClient.GetStudentParent(@params[0])!),
                _ => ""
            }
        };
    }

    public static Result HandleGetListOfClassesRequest(List<string?> @params)
    {
        var classes = @params[0][0] == 'A'
            ? DataBaseClient.GetAllClasses()
            : DataBaseClient.GetTeacherClasses(@params[0]);

        return new GetListOfClassesRequestResult((!classes.Any()) ? Status.Failed : Status.Succeed)
        {
            Classes = classes
        };
    }

    public static Result HandleGetStudentsAndTheirGradesByClassAndSubjectRequest(List<string?> @params)
    {
        return new GetStudentsAndTheirGradesByClassAndSubjectRequestResult(Status.Succeed)
        {
            StudentsAndTheirGrades = DataBaseClient.GetStudentsAndTheirGradesFromSpecificSubject(@params[0], @params[1])
        };
    }

    public static Result HandleAddStudentToClassRequest(List<string?> @params)
    {
        DataBaseClient.AddStudent(@params[0], @params[1], @params[2], @params[3], @params[4]);

        return new AddStudentToClassRequestResult(Status.Succeed);
    }

    public static Result HandleGetStudentsByClassRequest(List<string?> @params)
    {
        return new GetStudentsByClassRequestResult(Status.Succeed)
        {
            Students = DataBaseClient.GetClassStudents(@params[0])
        };
    }

    public static Result HandleDeleteStudentFromClassRequest(List<string?> @params)
    {
        DataBaseClient.DeleteStudent(@params[0]);

        return new DeleteStudentFromClassRequestResult(Status.Succeed);
    }

    public static Result HandleAddGradeRequest(List<string?> @params)
    {
        DataBaseClient.AddGrade(@params[0], @params[1], @params[2]);

        return new AddGradeRequestResult(Status.Succeed);
    }

    public static Result HandleDeleteGradeRequest(List<string?> @params)
    {
        DataBaseClient.DeleteGrade(@params[0]);

        return new DeleteGradeRequestResult(Status.Succeed);
    }
}