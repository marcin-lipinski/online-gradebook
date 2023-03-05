using System.Text.Json.Serialization;

namespace Server;

public sealed record Account
{
    [property: JsonPropertyName("login")] public string? Login { get; set; }

    [property: JsonPropertyName("password")]
    public string? Password { get; set; }

    [property: JsonPropertyName("subaccount")]
    public string? Subaccount { get; set; }
}

public record Person
{
    [property: JsonPropertyName("ID")] public string? Id { get; init; }
    [property: JsonPropertyName("name")] public string? Name { get; init; }

    [property: JsonPropertyName("surname")]
    public string? Surname { get; init; }
}

public sealed record Student : Person
{
    [property: JsonPropertyName("schoolClass")]
    public string? SchoolClass { get; init; }
}

public record Parent : Person
{
    [property: JsonPropertyName("student")]
    public string? Student { get; init; }
}

public sealed record Teacher : Person
{
}

public sealed record Admin : Person
{
}

public sealed record SchoolClass
{
    [property: JsonPropertyName("ID")] public string? Id { get; set; }
    [property: JsonPropertyName("name")] public string? Name { get; set; }
}

public sealed record Subject
{
    [property: JsonPropertyName("ID")] public string? Id { get; set; }
    [property: JsonPropertyName("name")] public string? Name { get; set; }
}

public sealed record Grade
{
    [property: JsonPropertyName("ID")] public string? Id { get; init; }

    [property: JsonPropertyName("subject")]
    public string? Subject { get; init; }

    [property: JsonPropertyName("student")]
    public string? Student { get; init; }

    [property: JsonPropertyName("value")] public int Value { get; init; }
}

public sealed record ClassSubjectTeacher
{
    [property: JsonPropertyName("schoolClass")]
    public string? SchoolClass { get; set; }

    [property: JsonPropertyName("subject")]
    public string? Subject { get; set; }

    [property: JsonPropertyName("teacher")]
    public string? Teacher { get; set; }
}