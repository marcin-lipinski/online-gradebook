namespace Server.RequestService;

[Serializable]
public class Request
{
    public RequestType RequestType { get; set; }
    public List<string?> Params { get; set; }

    public Request(RequestType requestType, List<string?> @params)
    {
        RequestType = requestType;
        Params = @params;
    }

    public Request()
    {
        Params = new List<string?>();
    }
}