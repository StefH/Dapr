using Dapr;
using Microsoft.AspNetCore.Mvc;
using MyBackend.Models;

namespace MyBackend.Controllers;

public class OrderController : Controller
{
    [Topic("pubsub", "newOrder")]
    [HttpPost("/orders")]
    public async Task<ActionResult> CreateOrder(Order order)
    {
        return Ok();
    }
}