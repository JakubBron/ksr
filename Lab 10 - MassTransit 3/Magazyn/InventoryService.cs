namespace Magazyn
{
    public class InventoryService
    {
        private int _availableItems = 210;
        private int _reservedItems = 0;

        private readonly object _lockObject = new();

        public (int Available, int Reserved) GetInventoryStatus()
        {
            lock (_lockObject)
            {
                return (_availableItems, _reservedItems);
            }
        }

        public void AddInventory(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Wartosc musi byc > 0", nameof(quantity));

            lock (_lockObject)
            {
                _availableItems += quantity;
            }
        }

        public bool TryReserveItems(int quantity)
        {
            lock (_lockObject)
            {
                if (_availableItems >= quantity)
                {
                    _availableItems -= quantity;
                    _reservedItems += quantity;
                    return true;
                }
                return false;
            }
        }

        public void ConfirmReservation(int quantity)
        {
            lock (_lockObject)
            {
                if (_reservedItems < quantity)
                    throw new InvalidOperationException("Za mało na magazynie, anulowanie zam.");

                _reservedItems -= quantity;
            }
        }

        public void CancelReservation(int quantity)
        {
            lock (_lockObject)
            {
                if (_reservedItems < quantity)
                    throw new InvalidOperationException("Za mało na magazynie, anulowanie zam.");

                _reservedItems -= quantity;
                _availableItems += quantity;
            }
        }
    }
}