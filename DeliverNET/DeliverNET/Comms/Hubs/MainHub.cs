using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliverNET.Data;
using DeliverNET.Managers;
using DeliverNET.Managers.Interfaces;
using DeliverNET.Models;
using DeliverNET.Models.ProfileBusiViewModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace DeliverNET.Comms.Hubs
{

    public class MainHub : Hub
    {
        private IOrderManager _mngOrder;
        private IBusinessManager _mngBussiness;
        private IMasterManager _mngMaster;
        private IDelivererManager _mngDeliverer;
        private string GroupAvailableName = "AvailableDeliverers";


        public MainHub(
            IMasterManager mng)
        {
            _mngMaster = mng;
            _mngOrder = mng.GetOrderManager();
            _mngBussiness = mng.GetBusinessManager();
            _mngDeliverer = mng.GetDelivererManager();

        }



        // Invoke from client to add a deliverer to the group of the available deliverers.
        public void AddToGroupAvailable()
        {
            // Add to group
            var result = _mngMaster.GetDelivererManager().SetWorkingStatus(Context.UserIdentifier, true);
            Groups.AddToGroupAsync(Context.ConnectionId, GroupAvailableName);

        }

        //Invoke from client to remove a deliverer from the group of the available deliverers.
        public void RemoveFromGroupAvailable()
        {
            // Remove from group   
            _mngMaster.GetDelivererManager().SetWorkingStatus(Context.UserIdentifier, false);
            Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupAvailableName);
        }


        // Invoke from Client busi in order to send a new order

        public async Task PlaceNewOrder(OrderBusiViewModel order)
        {
            DateTime tstamp = DateTime.Now;
            int orderId;
            string geolocation = _mngBussiness.Get(_mngMaster.GetBusinessCashierManager().Get(Context.UserIdentifier).BusinessId).Geolocation;

            Order NewOrder = new Order
            {
                Address = order.Address,
                DoorName = order.DoorName,
                FirstName = order.FirstName,
                FloorNo = order.FloorNo,
                LastName = order.LastName,
                PhoneNumber = order.PhoneNumber,
                PaymentTypeId = order.PaymentTypeId,
                Price = order.Price,
                Comments = order.Comments,

                Business = _mngBussiness.Get(_mngMaster.GetBusinessCashierManager().Get(Context.UserIdentifier).BusinessId),
                Cashier = _mngMaster.GetBusinessCashierManager().Get(Context.UserIdentifier),
                Tstamp = tstamp,
                Geolocation = geolocation,
                Tariff = 1,
                AcceptedTime = null,
                PickupTime = null,
                DeliveredTime = null,
                IsAccepted = false,
                IsPickedup = false,
                IsDelivered = false,
                IsTimedOut = false
            };

            _mngOrder.Create(NewOrder);

            // Get order id from db
            orderId = _mngOrder.Get(tstamp).Id;


            //send order id back to the client that invoked this method
            await Clients.Caller.SendAsync("GetOrderIdForAppend", orderId);

            // broadcast to deliverers group the order and the order id in method newOrderAnnounce. the name of the method that will be invoked in the client will be "NewOrder"
            await Clients.Group(GroupAvailableName).SendAsync("NewOrder", orderId, geolocation, order.PaymentTypeId, tstamp);

        }




        // Get specific order details from db (for deliverer)
        public async Task GetOrderFromDb(string orderId)
        {
            Order order = _mngOrder.Get(int.Parse(orderId));
            Business business = _mngBussiness.Get(order.BusinessId);
            await Clients.Caller.SendAsync("GetOrder", business, order);
        }

        // get specific order object (for business cashier)
        public async Task GetOrderFromDbBusi(string orderId)
        {
            Order order = _mngOrder.Get(int.Parse(orderId));
            await Clients.Caller.SendAsync("AppendNewOrder", order);
        }

        // Get info for indi, modal accepted
        public void GetInfoForIndiModalAccepted(string orderId)
        {
            Order order = _mngOrder.Get(int.Parse(orderId));
            Business business = _mngBussiness.Get(order.BusinessId);
            Clients.Caller.SendAsync("UpdateModalAcceptedInfo",
                business.Title,
                business.Address,
                order.Price,
                order.PaymentTypeId
                );
        }

        // Get info for indi, modal picked
        public async Task GetInfoForIndiModalPicked(string orderId)
        {
            Order order = _mngOrder.Get(int.Parse(orderId));
            await Clients.Caller.SendAsync("UpdateModalPickedInfo",
                            $"{order.FirstName} {order.LastName}",
                            order.Address,
                            order.DoorName,
                            order.FloorNo,
                            order.PhoneNumber,
                            order.Price,
                            order.PaymentTypeId
                            );
        }

        // Get all orders that are NOT timed out for deliverer
        public async Task GetActiveOrders()
        {
            List<Order> orders = _mngOrder.GetActive();
            await Clients.Caller.SendAsync("GetActiveOrders", orders);
        }

        // Get all orders that are NOT timed out for cashiers
        public async Task GetActiveOrdersForBusi()
        {
            BusinessCashier cashier = _mngMaster.GetBusinessCashierManager().Get(Context.UserIdentifier);
            List<Order> orders = _mngOrder.GetActive(cashier.BusinessId);
            await Clients.Caller.SendAsync("GetActiveOrders", orders);
        }


        // Get deliverer working status
        public async Task GetDelivererWorkingStatus()
        {
            bool isWorking = _mngDeliverer.GetWorkingStatus(Context.UserIdentifier);
            bool isDelivering = _mngDeliverer.GetDeliveringStatus(Context.UserIdentifier);
            await Clients.Caller.SendAsync("GetWorkingStatus", isWorking, isDelivering);
        }

        // Get deliverer's active order and its phase (pickedup/delivered)
        public async Task GetAcceptedOrderPhase()
        {
            List<Order> activeOrders = _mngOrder.GetActive();
            foreach (Order order in activeOrders)
            {
                if (order.DelivererId == Context.UserIdentifier)
                {
                    await Clients.Caller.SendAsync("GetOrderPhase", order.Id, order.IsAccepted, order.IsPickedup, order.IsDelivered);
                    break;
                }
            }
        }


        //
        //order accepted pickedup delivered functions
        //

        // invoked from client site when a deliverer acceptes the order
        public void OrderAccepted(string orderId)
        {
            int OrderId = int.Parse(orderId);

            _mngOrder.SetAcceptedTime(OrderId, DateTime.Now); //add the current time to the property acceptedtime in the db for this order
            _mngOrder.SetIsAccepted(OrderId, true);  //change status to db to IsAccepted=true
            _mngOrder.SetDeliverer(OrderId, _mngDeliverer.Get(Context.UserIdentifier));//The deliverer that accepted the order

            _mngDeliverer.SetDeliveringStatus(Context.UserIdentifier, true); //is delivering db change to true

            Clients.Group(GroupAvailableName).SendAsync("AnOrderIsAccepted", orderId); //Broadcast to available deliverers the order is taken=>invoke function AnOrderIsAccepted

            Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupAvailableName);  //remove from group availabledeliverers the context.useridentifier deliverer 

            // Send order details to deliverer who accepted the order
            Clients.Caller.SendAsync("OnOrderAccepted", _mngOrder.Get(OrderId));
        }

        // invokes from client site when a deliverer PickesUp the order
        public void OrderPickedUp(string orderId)
        {
            _mngOrder.SetPickupTime(int.Parse(orderId), DateTime.Now); //add current time to the property Pickeduptime in the db for this order
            _mngOrder.SetIsPickedUp(int.Parse(orderId), true); //change status to db to IsPickedUp=true
        }


        // invokes from client site when a deliverer Finally delivers the order
        public void OrderDelivered(string orderId)
        {
            int OrderId = int.Parse(orderId);
            _mngOrder.SetDeliveredTime(OrderId, DateTime.Now); //add the current time to the property isdelivered in the db for this 

            _mngOrder.SetIsDelivered(OrderId, true); //change status to db to IsDelivered=true
            _mngDeliverer.SetDeliveringStatus(Context.UserIdentifier, false); //Change deliverer status IsDelivering to false

            _mngOrder.SetTimeoutState(OrderId, true);

            //TODO: decide if you want to add the deliverer again to group with available deliverers
            Groups.AddToGroupAsync(Context.ConnectionId, GroupAvailableName);
        }





        //
        // Order checks
        //
        // broadcasts if order is timed out
        public async Task CheckOrderTimeout(string orderId)
        {
            Order order = _mngOrder.Get(int.Parse(orderId));
            await Clients.Caller.SendAsync("CheckOrderTimeout", orderId, order.Tstamp, order.IsTimedOut);
        }

        // broadcasts if order is accepted
        public async Task CheckOrderAccepted(string orderId)
        {
            Order order = _mngOrder.Get(int.Parse(orderId));
            await Clients.Caller.SendAsync("CheckOrderAccepted", orderId, order.IsAccepted);
        }

        // broadcasts if order is pickedup
        public async Task CheckOrderPickedup(string orderId)
        {
            Order order = _mngOrder.Get(int.Parse(orderId));
            await Clients.Caller.SendAsync("CheckOrderPickedup", orderId, order.IsPickedup);
        }

        // broadcasts if order is delivered
        public async Task CheckOrderDelivered(string orderId)
        {
            Order order = _mngOrder.Get(int.Parse(orderId));
            await Clients.Caller.SendAsync("CheckOrderDelivered", orderId, order.IsDelivered);
        }
    }
}
