using Services.Services.BasketService.Dto;
using Services.Services.OrderService.Dto;

namespace Services.Services.PaymentService
{
    public interface IPaymentService
    {
        Task<CustomerBasketDto> CreateOrUpdatePaymentIntentForExistingOrder(CustomerBasketDto basket);
        Task<CustomerBasketDto> CreateOrUpdatePaymentIntentForNewOrder(string basketId);
        Task<OrderResultDto> UpdateOrderPaymentSucceded(string paymentIntentId, string basketId);
        Task<OrderResultDto> UpdateOrderPaymentFailed(string paymentIntentId);
    }
}
