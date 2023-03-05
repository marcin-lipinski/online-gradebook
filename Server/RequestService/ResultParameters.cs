namespace Server.RequestService;

public enum RequestType
{
    LogIn = 0,
    GetStudentGrades = 1,
    GetListOfClasses = 2,
    AddStudentToClass = 3,
    DeleteStudentFromClass = 4,
    GetStudentsAndTheirGradesByClassAndSubject = 5,
    GetStudentsByClass = 6,
    AddGrade = 7,
    DeleteGrade = 8,
    ExitRequest = 9,
}

public enum Status
{
    Unknown = 0,
    Succeed = 1,
    Failed = 2
}