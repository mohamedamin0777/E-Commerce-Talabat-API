using Core.Entities.OrderEntities;

namespace Infrastructure.Specifications
{
    public class OrderWithPaymentIntentSpecification : BaseSpecifications<Order>
    {
        public OrderWithPaymentIntentSpecification(string paymentIntentId) 
            : base(order => order.PaymentIntentId == paymentIntentId)
        {
        }
    }
}
