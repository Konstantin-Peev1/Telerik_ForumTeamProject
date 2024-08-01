using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class TestHttpContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _chatRoomId;

    public TestHttpContextMiddleware(RequestDelegate next, string chatRoomId)
    {
        _next = next;
        _chatRoomId = chatRoomId;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Request.QueryString = new QueryString($"?chatRoomId={_chatRoomId}");
        await _next(context);
    }
}
