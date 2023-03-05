namespace Client.RequestService;

[Serializable]
public record Result
{
    protected Result(RequestType requestType, Status status)
    {
        RequestType = requestType;
        Status = status;
    }

    public RequestType RequestType { get; init; }
    public Status Status { get; init; }
}

[Serializable]
public sealed record AddGradeRequestResult(Status Status) : Result(RequestType.AddGrade, Status);

[Serializable]
public sealed record AddStudentToClassRequestResult(Status Status) : Result(RequestType.AddStudentToClass, Status);

[Serializable]
public sealed record DeleteGradeRequestResult(Status Status) : Result(RequestType.DeleteGrade, Status);

[Serializable]
public sealed record DeleteStudentFromClassRequestResult(Status Status) : Result(RequestType.DeleteStudentFromClass,
    Status);

[Serializable]
public sealed record GetListOfClassesRequestResult(Status Status) : Result(RequestType.GetListOfClasses, Status)
{
    public List<Tuple<string, string, List<Tuple<string, string>>>> Classes { get; init; } = null!;
}

[Serializable]
public sealed record GetStudentGradesRequestResult(Status Status) : Result(RequestType.GetStudentGrades, Status)
{
    public string StudentName { get; init; } = null!;
    public List<Tuple<string, List<string>>>? Grades { get; init; }
}

[Serializable]
public sealed record GetStudentsAndTheirGradesByClassAndSubjectRequestResult(Status Status) : Result(
    RequestType.GetStudentsAndTheirGradesByClassAndSubject, Status)
{
    public List<Tuple<string, string, string, List<Tuple<string, string>>>> StudentsAndTheirGrades { get; init; } =
        null!;
}

[Serializable]
public sealed record GetStudentsByClassRequestResult(Status Status) : Result(RequestType.GetStudentsByClass, Status)
{
    public List<Tuple<string, string>> Students { get; init; } = null!;
}

[Serializable]
public sealed record LogInRequestResult(Status Status) : Result(RequestType.LogIn, Status)
{
    public UserType UserType { get; init; }
    public string Name { get; init; } = null!;
    public string Subaccount { get; init; } = null!;
}