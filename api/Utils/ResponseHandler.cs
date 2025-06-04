public static class ResponseHandler
{
    public static async Task SendSuccess(HttpResponse response, object data, int statusCode = 200, string message = "Success")
    {
        response.StatusCode = statusCode;
        response.ContentType = "application/json";

        var result = new
        {
            message,
            data
        };
        await response.WriteAsJsonAsync(result);
    }
    public static async Task SendError(HttpResponse response, string message, int statusCode = 500)
    {
        response.StatusCode = statusCode;
        response.ContentType = "application/json";
        var result = new
        {
            message,
        };
        await response.WriteAsJsonAsync(result);
    }
}