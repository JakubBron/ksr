using MessageTypesDefinition;
using Magazyn;
using MassTransit;

namespace Magazyn
{
    public class InventoryRequestConsumer : IConsumer<InventoryRequest>
    {
        private readonly InventoryService _inventoryService;

        public InventoryRequestConsumer(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public async Task Consume(ConsumeContext<InventoryRequest> context)
        {
            var orderId = context.Message.OrderId;
            var quantity = context.Message.Quantity;

            Console.WriteLine($"Próba realizacji zam. id={orderId} na {quantity} szt.");

            var (available, _) = _inventoryService.GetInventoryStatus();
            Console.WriteLine($"TERAZ: {available} szt. OK");

            if (available >= quantity)
            {
                _inventoryService.TryReserveItems(quantity);
                await context.Publish(new InventoryAvailable { OrderId = orderId });
                Console.WriteLine($"Można zrealizować zam. id={orderId}, rezerwacja {quantity} szt.");
            }
            else
            {
                await context.Publish(new InventoryUnavailable { OrderId = orderId });
                Console.WriteLine($"Niewystarczające zapasy! zam. id={orderId}, chciane {quantity} z {available} dostępnych.");
            }
        }
    }

    public class OrderAcceptedConsumer : IConsumer<OrderAccepted>
    {
        private readonly InventoryService _inventoryService;

        public OrderAcceptedConsumer(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public Task Consume(ConsumeContext<OrderAccepted> context)
        {
            var orderId = context.Message.OrderId;
            var quantity = context.Message.Quantity;

            _inventoryService.ConfirmReservation(quantity);

            Console.WriteLine($"Zam. id={orderId} wysłano, magazyn uszczuplony o {quantity} szt.");

            return Task.CompletedTask;
        }
    }

    public class OrderRejectedConsumer : IConsumer<OrderRejected>
    {
        private readonly InventoryService _inventoryService;

        public OrderRejectedConsumer(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public Task Consume(ConsumeContext<OrderRejected> context)
        {
            var orderId = context.Message.OrderId;
            var quantity = context.Message.Quantity;

            _inventoryService.CancelReservation(quantity);

            Console.WriteLine($"Zam id={orderId} anulowane przez klienta, na stan wraca {quantity} szt.");

            return Task.CompletedTask;
        }
    }
}
