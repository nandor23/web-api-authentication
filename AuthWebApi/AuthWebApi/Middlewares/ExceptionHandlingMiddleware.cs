using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shared.Exceptions;
using Shared.Exceptions.AuthExceptions;

namespace AuthWebApi.Middlewares;
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JsonSerializerSettings _jsonSettings;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
        _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BaseException exception)
        {
            switch (exception)
            {
                case SignUpException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    break;
                case LoginException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    break;
                case LogoutException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    break;
                case RepositoryException:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new { exception.Errors }, _jsonSettings));
        }
    }
}