using DeliverNET.Managers.Interfaces;

namespace DeliverNET.Managers
{
    /// <summary>
    /// Contains methods to return every manager.
    /// </summary>
    public class MasterManager : IMasterManager
    {
        private readonly IOrderManager _orderManager;
        private readonly IDelivererManager _delivererManager;
        private readonly IBusinessManager _businessManager;
        private readonly IBusinessCashierManager _businessCashierManager;
        private readonly IBusinessOwnerManager _businessOwnerManager;

        public MasterManager(
            IOrderManager orderManager,
            IDelivererManager delivererManager,
            IBusinessManager businessManager,
            IBusinessCashierManager businessCashierManager,
            IBusinessOwnerManager businessOwnerManager
            )
        {
            _orderManager = orderManager;
            _delivererManager = delivererManager;
            _businessManager = businessManager;
            _businessCashierManager = businessCashierManager;
            _businessOwnerManager = businessOwnerManager;
        }

        public IBusinessCashierManager GetBusinessCashierManager() => _businessCashierManager;
        public IBusinessManager GetBusinessManager() =>_businessManager;

        public IBusinessOwnerManager GetBusinessOwnerManager() => _businessOwnerManager;

        public IDelivererManager GetDelivererManager() => _delivererManager;
        public IOrderManager GetOrderManager() => _orderManager;
    }
}
