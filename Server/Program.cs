using System.IO.Pipes;
using System.Text;
using Server.RequestService;
using Newtonsoft.Json;

namespace Server
{
    class Program
    {
        static void Main()
        {
            var server = new NamedPipeServerStream("PipesOfPiece", PipeDirection.InOut,
                1, PipeTransmissionMode.Byte);
            server.WaitForConnection();

            while (true)
            {
                var request = GetRequest(server);
                Result? result = null;

                Console.WriteLine("Received: {0}", request?.RequestType.ToString());
                if (request?.Params.Count > 0)
                    Console.WriteLine("Data0: {0}", request.Params[0]);
                if (request?.Params.Count > 1)
                    Console.WriteLine("Data1: {0}", request.Params[1]);
                Console.WriteLine();

                switch (request!.RequestType)
                {
                    case RequestType.LogIn:
                        result = Handlers.HandleLogInRequest(request.Params);
                        break;
                    case RequestType.GetStudentGrades:
                        result = Handlers.HandleGetStudentGradesRequest(request.Params);
                        break;
                    case RequestType.GetListOfClasses:
                        result = Handlers.HandleGetListOfClassesRequest(request.Params);
                        break;
                    case RequestType.GetStudentsAndTheirGradesByClassAndSubject:
                        result = Handlers.HandleGetStudentsAndTheirGradesByClassAndSubjectRequest(request.Params);
                        break;
                    case RequestType.AddGrade:
                        result = Handlers.HandleAddGradeRequest(request.Params);
                        break;
                    case RequestType.GetStudentsByClass:
                        result = Handlers.HandleGetStudentsByClassRequest(request.Params);
                        break;
                    case RequestType.DeleteGrade:
                        result = Handlers.HandleDeleteGradeRequest(request.Params);
                        break;
                    case RequestType.AddStudentToClass:
                        result = Handlers.HandleAddStudentToClassRequest(request.Params);
                        break;
                    case RequestType.DeleteStudentFromClass:
                        result = Handlers.HandleDeleteStudentFromClassRequest(request.Params);
                        break;
                    case RequestType.ExitRequest:
                        server.Disconnect();
                        server.Dispose();
                        return;
                }

                SendResult(result, server);
            }
        }

        private static Request? GetRequest(NamedPipeServerStream server)
        {
            int readBytes;
            var messageBuilder = new StringBuilder();
            do
            {
                var messageBuffer = new byte[10];
                readBytes = server.Read(messageBuffer, 0, messageBuffer.Length);
                messageBuilder.Append(Encoding.UTF8.GetString(messageBuffer));
            } while (readBytes == 10);
            
            return JsonConvert.DeserializeObject<Request>(messageBuilder.ToString());
        }

        private static void SendResult(Result? result, NamedPipeServerStream server)
        {
            var serialised = JsonConvert.SerializeObject(result);
            var messageBytes = Encoding.UTF8.GetBytes(serialised);
            if (messageBytes.Length % 10 == 0) Array.Resize(ref messageBytes, messageBytes.Length + 1);
            server.Write(messageBytes, 0, messageBytes.Length);
        }
    }
}