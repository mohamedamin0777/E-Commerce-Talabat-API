﻿using AutoMapper;
using Core.Entities;
using Core.Entities.OrderEntities;
using Infrastructure.BasketRepository.BasketEntities;
using Infrastructure.Interfaces;
using Infrastructure.Specifications;
using Services.Services.BasketService;
using Services.Services.BasketService.Dto;
using Services.Services.OrderService.Dto;
using Services.Services.PaymentService;

namespace Services.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IBasketService _basketService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;

        public OrderService(
            IBasketService basketService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPaymentService paymentService)
        {
            _basketService = basketService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paymentService = paymentService;
        }
        public async Task<OrderResultDto> CreateOrderAsync(OrderDto orderDto)
        {
            // Get Basket
            var basket = await _basketService.GetBasketAsync(orderDto.BasketId);

            if (basket is null)
                return null;

            //Fill OrderItems from BasketItems
            var orderItems = new List<OrderItemDto>();
            foreach(var item in basket.BasketItems)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
                var orderItem = new OrderItem(productItem.Price, item.Quantity, itemOrdered);

                var mappedOrderItem = _mapper.Map<OrderItemDto>(orderItem);
                orderItems.Add(mappedOrderItem);
            }

            // Get Delivery Method
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(orderDto.DeliveryMethodId);

            // Calculate Subtotal
            var subTotal = orderItems.Sum(item => item.Price *  item.Quantity);

            // ToDo => Check if order exist
            var specs = new OrderWithPaymentIntentSpecification(basket.PaymentIntentId);

            var existingOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpecificationsAsync(specs);
            CustomerBasketDto customerBasket = new CustomerBasketDto();
            if(existingOrder != null)
            {
                _unitOfWork.Repository<Order>().Delete(existingOrder);
                await _paymentService.CreateOrUpdatePaymentIntentForExistingOrder(basket);
            }
            else
            {
                customerBasket = await _paymentService.CreateOrUpdatePaymentIntentForNewOrder(basket.Id);
            }

            // Create Order
            var mappedShippingAddress = _mapper.Map<ShippingAddress>(orderDto.ShippingAddress);
            var mappedOrderItems = _mapper.Map<List<OrderItem>>(orderItems);

            var order = new Order(orderDto.BuyerEmail, mappedShippingAddress, deliveryMethod, mappedOrderItems, subTotal, basket.PaymentIntentId ?? customerBasket.PaymentIntentId);

            await _unitOfWork.Repository<Order>().Add(order);

            //await _unitOfWork.Complete();

            //Delete Basket
            //await _basketService.DeleteBasketAsync(orderDto.BasketId);

            var mappedOrder = _mapper.Map<OrderResultDto>(order);

            return mappedOrder;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
            => await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();

        public async Task<IReadOnlyList<OrderResultDto>> GetAllOrdersForUserAsync(string buyerEmail)
        {
            var specs = new OrderWithItemsSpecification(buyerEmail);

            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecificationsAsync(specs);

            var mappedOrders = _mapper.Map<List<OrderResultDto>>(orders);

            return mappedOrders;
        }

        public async Task<OrderResultDto> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var specs = new OrderWithItemsSpecification(id, buyerEmail);

            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecificationsAsync(specs);

            var mappedOrder = _mapper.Map<OrderResultDto>(order);

            return mappedOrder;
        }
    }
}
