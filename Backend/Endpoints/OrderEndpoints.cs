using FlowerShop.Backend.Data;
using FlowerShop.Backend.DTOs;
using FlowerShop.Backend.Models;
using FlowerShop.Backend.Services;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Backend.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/catalog/order");

        group.MapPost("/", async (CreateOrderDto dto, AppDbContext dbContext, INotificationService notificationService) =>
        {
            if (dto.Items == null || !dto.Items.Any()) return Results.BadRequest("Order must contain items");

            var flowerIds = dto.Items.Select(i => i.FlowerId).ToList();
            var flowers = await dbContext.Flowers.Where(f => flowerIds.Contains(f.Id)).ToListAsync();

            if (flowers.Count != flowerIds.Count) return Results.BadRequest("Some flowers were not found");

            var order = new Order
            {
                CustomerName = dto.CustomerName,
                CustomerPhone = dto.CustomerPhone,
                DeliveryAddress = dto.DeliveryAddress,
                TotalAmount = 0
            };

            foreach (var item in dto.Items)
            {
                var flower = flowers.First(f => f.Id == item.FlowerId);
                order.Items.Add(new OrderItem
                {
                    FlowerId = flower.Id,
                    Quantity = item.Quantity,
                    UnitPrice = flower.Price
                });
                order.TotalAmount += flower.Price * item.Quantity;
            }

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();

            // Отправка уведомления владельцу
            var message = $"Новый заказ #{order.Id}\n" +
                          $"Имя: {order.CustomerName}\n" +
                          $"Телефон: {order.CustomerPhone}\n" +
                          $"Адрес: {order.DeliveryAddress}\n" +
                          $"Сумма: {order.TotalAmount} ₽";

            await notificationService.SendOrderNotificationAsync(message);

            return Results.Ok(new { message = "Order placed successfully", orderId = order.Id });
        });
    }
}
