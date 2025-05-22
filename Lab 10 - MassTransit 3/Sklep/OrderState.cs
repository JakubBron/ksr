using MassTransit;

namespace Sklep;

public class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public bool ClientConfirmed { get; set; }
    public bool WarehouseConfirmed { get; set; }
    public Guid? TimeoutTokenId { get; set; }
}
