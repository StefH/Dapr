using Dapr;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.Models;

namespace MyBackend.Controllers;

[ApiController]
public class OrderController : Controller
{
    private readonly ILogger<OrderController> _logger;

    public OrderController(ILogger<OrderController> logger)
    {
        _logger = logger;
    }

    [Topic(pubsubName: "pubsub", name: "newOrder")]
    [HttpPost("/orders")]
    public ActionResult CreateOrder(Order order)
    {
        _logger.LogInformation("CreateOrder: Id={Id}, Quantity={Quantity}", order.Id, order.Quantity);

        return Ok();
    }
}