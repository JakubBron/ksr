using MessageTypesDefinition;
using MassTransit;
using System;

namespace Klient_A
{
    public class ConfirmationRequestConsumer : IConsumer<ConfirmationRequest>
    {
        public async Task Consume(ConsumeContext<ConfirmationRequest> context)
        {
            Console.WriteLine($"Otrzymano potw. zam. id= {context.Message.OrderId}; ilosc = {context.Message.Quantity}");
            Console.Write("'Y' aby potwwierdzic, dow. inny żeby odrzucić: ");

            var key = Console.ReadLine();
            Console.WriteLine($"Key pressed: {key}");

            if (key == "Y")
            {
                await context.Publish(new Confirmation { OrderId = context.Message.OrderId });
                Console.WriteLine($"Zam. id={context.Message.OrderId} potwierdzone");
            }
            else
            {
                await context.Publish(new Rejection { OrderId = context.Message.OrderId });
                Console.WriteLine($"Zam. id={context.Message.OrderId} odrzucone");
            }
        }
    }

    public class OrderAcceptedConsumer : IConsumer<OrderAccepted>
    {
        public Task Consume(ConsumeContext<OrderAccepted> context)
        {
            Console.WriteLine($"Zam. id= {context.Message.OrderId} na {context.Message.Quantity} obj. OK!");
            return Task.CompletedTask;
        }
    }

    public class OrderRejectedConsumer : IConsumer<OrderRejected>
    {
        public Task Consume(ConsumeContext<OrderRejected> context)
        {
            Console.WriteLine($"Zam. id= {context.Message.OrderId} na {context.Message.Quantity} obj. zly!");
            return Task.CompletedTask;
        }
    }
}