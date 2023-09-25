using System.Net;

namespace ApiPeliculasIdentity.Shared;

public class ApiReponse
{
    public HttpStatusCode? StatusCode { get; set; }
    public bool IsSuccess { get; set; } = false;
    public object? Result { get; set; }
    public List<string> ErrorMessages { get; set; }

    public ApiReponse()
    {
        ErrorMessages = new();
    }
}
