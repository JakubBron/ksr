using MessageTypesDefinition;
using MassTransit;
using MessageTypesDefinition;
using Sklep;

namespace Sklep;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderReceived, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => ClientConfirmation, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => ClientRejection, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => WarehouseConfirmation, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => WarehouseRejection, x => x.CorrelateById(context => context.Message.OrderId));
        Schedule(() => OrderTimeout, x => x.TimeoutTokenId, s => {
            s.Delay = TimeSpan.FromSeconds(10);
            s.Received = x => x.CorrelateById(c => c.Message.OrderId);
        });

        Initially(
            When(OrderReceived)
                .Then(Initialize)
                .TransitionTo(AwaitingConfirmations)
                .Schedule(OrderTimeout, context => new OrderTimeout { OrderId = context.Saga.CorrelationId })
                .ThenAsync(SendRequests)
        );

        During(AwaitingConfirmations,
            When(ClientConfirmation)
                .Then(context => context.Saga.ClientConfirmed = true)
                .ThenAsync(CheckCompletionStatus),

            When(ClientRejection)
                .TransitionTo(Rejected)
                .ThenAsync(SendRejectionToClient),

            When(WarehouseConfirmation)
                .Then(context => context.Saga.WarehouseConfirmed = true)
                .ThenAsync(CheckCompletionStatus),

            When(WarehouseRejection)
                .TransitionTo(Rejected)
                .ThenAsync(SendRejectionToClient)
                .TransitionTo(AwaitingConfirmations),

            When(OrderTimeout!.Received)
                .TransitionTo(Rejected)
                .ThenAsync(HandleTimeout)
        );
    }

    public State AwaitingConfirmations { get; private set; }
    public State Accepted { get; private set; }
    public State Rejected { get; private set; }

    public Event<StartOrder> OrderReceived { get; private set; }
    public Event<Confirmation> ClientConfirmation { get; private set; }
    public Event<Rejection> ClientRejection { get; private set; }
    public Event<InventoryAvailable> WarehouseConfirmation { get; private set; }
    public Event<InventoryUnavailable> WarehouseRejection { get; private set; }

    public Schedule<OrderState, OrderTimeout> OrderTimeout { get; private set; }

    private static void Initialize(BehaviorContext<OrderState, StartOrder> context)
    {
        context.Saga.Quantity = context.Message.Quantity;
        context.Saga.ClientId = context.Message.ClientId;

        Console.WriteLine($"Zam. id={context.Saga.CorrelationId} złożone przez: {context.Saga.ClientId} na {context.Saga.Quantity} szt.");
    }

    private static async Task SendRequests(BehaviorContext<OrderState, StartOrder> context)
    {
        var endpoint = await context.GetSendEndpoint(new Uri("queue:warehouse"));
        await endpoint.Send(new InventoryRequest
        {
            OrderId = context.Saga.CorrelationId,
            Quantity = context.Saga.Quantity
        });

        Console.WriteLine($"Wysłano żądanie do magazynu na {context.Saga.CorrelationId} szt.");

        endpoint = await context.GetSendEndpoint(new Uri($"queue:client-{context.Saga.ClientId}"));
        await endpoint.Send(new ConfirmationRequest
        {
            OrderId = context.Saga.CorrelationId,
            Quantity = context.Saga.Quantity
        });

        Console.WriteLine($"Wysłano potwierdzenie do {context.Saga.ClientId} na {context.Saga.CorrelationId} szt.");
    }

    private async Task CheckCompletionStatus(BehaviorContext<OrderState> context)
    {
        Console.WriteLine($"Zam id={context.Saga.CorrelationId}: Klient potw. {context.Saga.ClientConfirmed}, Magazyn potw.: {context.Saga.WarehouseConfirmed}");

        if (context.Saga is { ClientConfirmed: true, WarehouseConfirmed: true })
        {
            context.Saga.CurrentState = nameof(Accepted);
            await SendAcceptanceToClient(context);
        }
    }

    private static async Task SendAcceptanceToClient(BehaviorContext<OrderState> context)
    {
        var endpoint = await context.GetSendEndpoint(new Uri($"queue:client-{context.Saga.ClientId}"));
        await endpoint.Send(new OrderAccepted
        {
            OrderId = context.Saga.CorrelationId,
            Quantity = context.Saga.Quantity
        });

        Console.WriteLine($"Zam id={context.Saga.CorrelationId} OK, wysyłam info do {context.Saga.ClientId}");
    }

    private static async Task SendRejectionToClient(BehaviorContext<OrderState> context)
    {
        var endpoint = await context.GetSendEndpoint(new Uri($"queue:client-{context.Saga.ClientId}"));
        await endpoint.Send(new OrderRejected
        {
            OrderId = context.Saga.CorrelationId,
            Quantity = context.Saga.Quantity
        });

        Console.WriteLine($"Zam id={context.Saga.CorrelationId} odrzucone, wysyłam info do {context.Saga.ClientId}");
    }

    private async Task HandleTimeout(BehaviorContext<OrderState, OrderTimeout> context)
    {
        Console.WriteLine($"Zam id={context.Saga.CorrelationId} przekroczyło czas!");
        await SendRejectionToClient(context);
    }
}