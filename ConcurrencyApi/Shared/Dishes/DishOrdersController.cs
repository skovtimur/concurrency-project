using System.ComponentModel.DataAnnotations;
using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;

namespace ConcurrencyApi.ChannelsCode;

[ApiController]
[Route("channels-code/orders")]
public class DishOrdersController(
    DishOrdersRepository ordersRepository,
    Channel<SendOrderToRestaurantsRequest> sendOrderChannel,
    Channel<OrderReceiptRequest> orderReceiptChannel)
    : ControllerBase
{
    [HttpPost("order-dish")]
    public async Task<IActionResult> OrderDish([Required] CreateDishOrderRequest request)
    {
        var order = await ordersRepository.CreateOrder(request.DishCode, request.UserName);

        var orderToRestaurantRequestTask = sendOrderChannel.Writer.WriteAsync(new SendOrderToRestaurantsRequest
        {
            DishCode = order.DishCode,
            OrderCode = order.OrderCode,
            UserName = order.UserName
        }).AsTask();
        var orderReceiptRequestTask = orderReceiptChannel.Writer.WriteAsync(new OrderReceiptRequest
        {
            DishCode = order.DishCode,
            Dish = order.Dish,
            OrderCode = order.OrderCode,
            UserName = order.UserName
        }).AsTask();

        await Task.WhenAll(orderReceiptRequestTask, orderToRestaurantRequestTask);
        return Ok($"Created: '{order.OrderCode}' order. Just wait for your order to arrive to you");
    }
}