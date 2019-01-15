"use strict";
// this js is for placing a new order in busi index the button 'submit order is bounded with this js'

//when document ready
//window.addEventListener("load", () => {
//create an new connection to the hub
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/Comms/Hubs/MainHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

//start the connection
connection.start()
    .then(() => {
        connection.invoke("GetActiveOrdersForBusi").catch(function (err) {
            return console.error(err.toString());
        })
    })
    .catch(function (err) {
        return console.error(err.toString());
    });

// declare 
const newOrderElement = document.getElementById("orderForm")

newOrderElement.addEventListener("submit", (e) => {
    e.preventDefault();
    console.log("mpika sto event submit");
    console.log(document.getElementById("comments").value);
    var order = {
        FirstName: document.getElementById("firstName").value,
        LastName: document.getElementById("lastName").value,
        Address: document.getElementById("address").value,
        FloorNo: document.getElementById("floor").value,
        DoorName: document.getElementById("doorName").value,
        PaymentTypeId: document.getElementById("paymentType").value,
        PhoneNumber: document.getElementById("phoneNumber").value,
        Price: document.getElementById("price").value,
        Comments: document.getElementById("comments").value
    }
    //console.log("kai edo");
    //console.log(order);
    console.log("order submitted");
    //TODO:append the new order
    //TODO:take from server the orderid

    placeANewOrder(order);

})


//the server sends back the order in an object in order for the client to append it to the html
connection.on("GetOrderIdForAppend", (orderId) => {
    connection.invoke("GetOrderFromDbBusi", orderId).catch(function (err) {
        return console.error(err.toString());
    });
});

// appends new order in list at time of submit
connection.on("AppendNewOrder",
    order => {
        appendOrder(order);
    });

// sends the order back to the server
function placeANewOrder(order) {
    connection.invoke("PlaceNewOrder", order).catch(function (err) {
        return console.error(err.toString());
    });
    //event.preventDefault();
}




//
// Order list manipulation
//
// append order item
var appendOrder = function (order) {
    var orderList = document.getElementById("orderList");

    var parser = new DOMParser();
    var doc = parser.parseFromString(orderItemHtml(order), "text/html");

    orderList.prepend(doc.getElementById(order.id));
}

// remove order item
var removeOrder = function (orderId) {
    document.getElementById(orderId).parentNode.removeChild(orderToRemove);
}

// remove all orders form list
var clearOrderList = function () {
    document.getElementById("orderList").getElementsByClassName("card").remove();
}

// order item html
var orderItemHtml = function (order) {

    // Format date appropriately for display
    var t = new Date(order.tstamp);
    var timeDisp = `${t.getDate()}/${t.getMonth() + 1}-${t.getHours()}:${t.getMinutes()}`

    // Format payment type based on value
    console.log(order.paymentTypeId);
    var paymentType;
    if (order.paymentTypeId == "0") {
        paymentType = "Cash";
    } else {
        paymentType = "Credit Card";
    }

    // return order item html
    return `<div id="${order.id}" class="card">
                        <div class="card-header bg-bone-3d" id="heading-order-${order.id}">

                            <div class="d-flex bg-bone-3d justify-content-between align-items-center">
                                <div>
                                    <div><strong>Date</strong></div>
                                    <div id="orderHead-Tstamp">${timeDisp}</div>
                                </div>
                                <div>
                                    <div><strong>Last Name</strong></div>
                                    <div id="orderHead-LastName">${order.lastName}</div>
                                </div>
                                <div>
                                    <div><strong>Address</strong></div>
                                    <div id="orderHead-Address">${order.address}</div>
                                </div>
                                <div>
                                    <div><strong>Price</strong></div>
                                    <div id="orderHead-Price">${order.price}</div>
                                </div>
                                <div class="justify-content-center">
                                    <div><strong>Status</strong></div>
                                    <div id="orderHead-Status-${order.id}" class="orderStatus-Searching">
                                        Searching
                                    </div>
                                </div>
                                <div>
                                    <button class="btn" data-toggle="collapse" data-target="#collapse-order-${order.id}" aria-expanded="true" aria-controls="collapse-order-${order.id}">
                                        <span class="fas fa-arrow-down text-jet"></span>
                                    </button>
                                </div>
                            </div>

                        </div>

                        <div id="collapse-order-${order.id}" class="collapse" aria-labelledby="heading-order-${order.id}" data-parent="#orderList">
                            <div class="card-body bg-bone-light-3d">
                                <div class="container no-gutters">
                                    <div class="row">
                                        <div class="col-md-4">
                                            <div><strong>Full Name</strong></div>
                                            <div id="orderBody-FullName">${order.firstName} ${order.lastName}</div>
                                        </div>
                                        <div class="col-md-3">
                                            <div><strong>Door Name</strong></div>
                                            <div id="orderBody-DoorName">${order.doorName}</div>
                                        </div>
                                        <div class="col-md-2">
                                            <div><strong>Floor</strong></div>
                                            <div id="orderBody-Floor">${order.floorNo}</div>
                                        </div>
                                        <div class="col-md-3">
                                            <div><strong>Phone Number</strong></div>
                                            <div id="orderBody-PhoneNumber">${order.phoneNumber}</div>
                                        </div>
                                    </div>

                                    <div class="row mt-3">
                                        <div class="col-md-4">
                                            <div><strong>Payment Type</strong></div>
                                            <div id="orderBody-PaymentType">${paymentType}</div>
                                        </div>
                                        <div class="col-md-8">
                                            <div><strong>Comments</strong></div>
                                            <div id="orderBody-Comments">${order.comments}</div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>`
}
//
//
//




//
// Get all NON TIMEDOUT orders
//
connection.on("GetActiveOrders",
    (orders) => {
        orders.forEach((o) => {
            appendOrder(o);
        })
    });

//})



//
// Background timer that updates orders in list
//
var orderWatcher = setInterval(function () {
    var orderList = document.getElementById("orderList").getElementsByClassName("card");

    // Return if no roders are appended
    if (orderList.length == 0) {
        return;
    }

    for (var i = 0; i < orderList.length; i++) {

        //console.log(`${i} - ClassListLength: ${orderList[i].classList.length}`);

        if (orderList[i].classList.contains("animate")) {
            continue;
        }

        // Check if timed out
        connection.invoke("CheckOrderTimeout", orderList[i].id).catch(function (err) {
            return console.error(err.toString());
        });

        // Check if accepted
        connection.invoke("CheckOrderAccepted", orderList[i].id).catch(function (err) {
            return console.error(err.toString());
        });

        // Check if pickedup
        connection.invoke("CheckOrderPickedup", orderList[i].id).catch(function (err) {
            return console.error(err.toString());
        });

        // Check if delivered
        connection.invoke("CheckOrderDelivered", orderList[i].id).catch(function (err) {
            return console.error(err.toString());
        });

    }
}, 1000);

// Update if timedout
connection.on("CheckOrderTimeout", (orderId, tStamp, isTimedOut) => {

    // If timed out remove from list
    if (isTimedOut) {
        document.getElementById(orderId).classList.add("animate");
        document.getElementById(orderId).addEventListener("animationend", function (event) {
            document.getElementById(orderId).remove();
        }, false);
    }
});

// Update if accepted
connection.on("CheckOrderAccepted", (orderId, isAccepted) => {

    // If isAccepted change status
    if (isAccepted) {
        var statusEl = document.getElementById(`orderHead-Status-${orderId}`);
        statusEl.classList.remove("orderStatus-Searching");
        statusEl.classList.add("orderStatus-Accepted");
        statusEl.innerHTML = "Accepted";
    }
});

// Update if pickedup
connection.on("CheckOrderPickedup", (orderId, isPickedup) => {

    // If isPickedup change status
    if (isPickedup) {
        var statusEl = document.getElementById(`orderHead-Status-${orderId}`);
        statusEl.classList.remove("orderStatus-Accepted");
        statusEl.classList.add("orderStatus-Pickedup");
        statusEl.innerHTML = "Pickedup";
    }
});

// Update if delivered
connection.on("CheckOrderDelivered", (orderId, isDelivered) => {

    // If isDelivered change status
    if (isDelivered) {
        var statusEl = document.getElementById(`orderHead-Status-${orderId}`);
        statusEl.classList.remove("orderStatus-Pickedup");
        statusEl.classList.add("orderStatus-Delivered");
        statusEl.innerHTML = "Delivered";
    }
});
//
//
//