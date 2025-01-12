using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

public class MessageController : Controller
{
    private readonly IWebHostEnvironment _hostEnvironment;

    public MessageController(IWebHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    // Your action method (SendMessageForClient) goes here...
}
