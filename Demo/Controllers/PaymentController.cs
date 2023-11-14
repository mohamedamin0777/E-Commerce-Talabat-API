using Demo.HandleResponses;
using Microsoft.AspNetCore.Mvc;
using Services.Services.BasketService;
using Services.Services.BasketService.Dto;
using Services.Services.OrderService.Dto;
using Services.Services.PaymentService;
using Stripe;

namespace Demo.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        private readonly IBasketService _baskService;
        private const string WhSecret = "whsec_52433d3c4f12d7a2ccc887e22861294642c5296bc9d19c5cd568abf79363823a";

        public PaymentController(IPaymentService paymentService, 
                                 ILogger<PaymentController> logger,
                                 IBasketService baskService)
        {
            _paymentService = paymentService;
            _logger = logger;
            _baskService = baskService;
        }

        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntentForExistingOrder(CustomerBasketDto basket)
        {
            var customerBusket = await _paymentService.CreateOrUpdatePaymentIntentForExistingOrder(basket);

            if (customerBusket is null)
                return BadRequest(new ApiResponse(400, "Problem with your basket"));

            return Ok(customerBusket);
        }

        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntentForNewOrder(string basketId)
        {
            var customerBusket = await _paymentService.CreateOrUpdatePaymentIntentForNewOrder(basketId);

            if (customerBusket is null)
                return BadRequest(new ApiResponse(400, "Problem with your basket"));

            return Ok(customerBusket);
        }

        [HttpPost]
        public async Task<ActionResult> WebHook(string basketId)
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], WhSecret);

                PaymentIntent paymentIntent;
                OrderResultDto order;

                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                    paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Failed: ", paymentIntent.Id);

                    order = await _paymentService.UpdateOrderPaymentFailed(paymentIntent.Id);
                    _logger.LogInformation("Order Updated to Payment Failed");
                }
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Succeeded: ", paymentIntent.Id);

                    order = await _paymentService.UpdateOrderPaymentSucceded(paymentIntent.Id, basketId);
                    _logger.LogInformation("Order Updated to Payment Succeeded");
                }
                // ... handle other event types
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
}
