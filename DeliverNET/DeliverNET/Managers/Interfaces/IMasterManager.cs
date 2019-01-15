namespace DeliverNET.Managers.Interfaces
{
    public interface IMasterManager
    {
        IOrderManager GetOrderManager();
        IDelivererManager GetDelivererManager();
        IBusinessManager GetBusinessManager();
        IBusinessCashierManager GetBusinessCashierManager();
        IBusinessOwnerManager GetBusinessOwnerManager();
    }
}
