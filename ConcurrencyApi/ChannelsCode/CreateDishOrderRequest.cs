using System.ComponentModel.DataAnnotations;

namespace ConcurrencyApi.ChannelsCode;

public class CreateDishOrderRequest
{
    /// <summary>
    /// Just put a random code, it doesn't matter
    /// </summary>
    [Required]
    public Guid DishCode { get; set; }

    /// <summary>
    /// Your name, it may be any name you want
    /// </summary>
    [Required, StringLength(64, MinimumLength = 1)]
    public string UserName { get; set; }
}