"use strict";
// This Script is binded to the available button 'btnReady' on the deliverer indexindi in order to add or remove from the group of available deliverers
//window.addEventListener("load", () => {
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/Comms/Hubs/MainHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start()
    .then(() => {
        checkDelivererStatus();
    })
    .catch(function (err) {
        return console.error(err.toString());
    });


var checkDelivererStatus = function () {

    connection.invoke("GetDelivererWorkingStatus").catch(function (err) {
        return console.error(err.toString());
    });

}


//when refresh press the button if IsWorking is true
connection.on("GetWorkingStatus", (isWorking, isDelivering) => {

    // if he is working just press the available button
    if (isWorking) {
        document.getElementById("btnReady").click();
    }

    // if he is delivering find his order and refer to corresponding function
    if (isDelivering) {
        connection.invoke("GetAcceptedOrderPhase").catch(function (err) {
            return console.error(err.toString());
        });
    }
})


connection.on("GetOrderPhase", (orderId, isAccepted, isPickedup, isDelivered) => {

    if (isDelivered) {
        return;
    }

    // update orderid in hidden field of all modals
    updateModalOrderIdHidden(orderId);

    if (isPickedup) {
        // update "modal picked" info
        connection.invoke("GetInfoForIndiModalPicked", orderId).catch(function (err) {
            return console.error(err.toString());
        });

        // open "modal picked"
        $("#orderPickedModal").modal({
            backdrop: 'static',
            keyboard: false
        });
    } else if (isAccepted) {
        // update "modal accepted" info
        connection.invoke("GetInfoForIndiModalAccepted", orderId).catch(function (err) {
            return console.error(err.toString());
        });

        // open "modal accepted"
        $("#orderAcceptedModal").modal({
            backdrop: 'static',
            keyboard: false
        });
    }

});



//
// Get all NON TIMEDOUT orders
//
connection.on("GetActiveOrders", (orders) => {
    orders.forEach((o) => {
        if (!o.isAccepted) {
            console.log(!o.orderAcceptedStatus);
            appendOrder(o.id, "--", "--", "5:00");
        }
    })
});


//invoke through Signalr the method that removes a deliver from the group of available deliverers
function removeMeFromGroup() {
    console.log('eimai unavailable')

    // Reomove from group of available deliverers
    connection.invoke("RemoveFromGroupAvailable").catch(function (err) {
        return console.error(err.toString());
    });
}
//invoke through Signalr the method that removes a deliver from the group of available deliverers
function addMeToGroup() {
    console.log('eimai available')

    // Add to group of available deliverers
    connection.invoke("AddToGroupAvailable").catch(function (err) {
        return console.error(err.toString());
    });

    // Get active orders
    connection.invoke("GetActiveOrders").catch(function (err) {
        return console.error(err.toString());
    });
}


//takes the order
connection.on("NewOrder", (orderId, coords, paymentType, Tstamp) => {

    console.log(orderId);
    console.log(coords);
    console.log(paymentType);
    console.log(Tstamp);
    notifyMe()
    console.log("eimai prin to promise")

    ETA(orderId, coords)
});


//
// Estimated time and distance
//
function ETA(orderId, busiCoords) {

    var orderETA;
    var orderDistance;

    navigator.geolocation.getCurrentPosition(function (pos) {
        var lat = pos.coords.latitude.toFixed(6),
            long = pos.coords.longitude.toFixed(6),
            coords = lat + ', ' + long;
        console.log(coords);
        console.log(busiCoords);
        calculateDistance(coords, busiCoords)
    });


    function calculateDistance(coords, busiCoords) {
        // alert(typeof mycoords)
        var origin = coords;
        console.log(coords);
        var destination = busiCoords;
        var service = new google.maps.DistanceMatrixService();
        service.getDistanceMatrix(
            {
                origins: [origin],
                destinations: [destination],
                travelMode: google.maps.TravelMode.DRIVING,
                // unitSystem: google.maps.UnitSystem.IMPERIAL, // miles and feet.
                unitSystem: google.maps.UnitSystem.metric, // kilometers and meters.
                avoidHighways: false,
                avoidTolls: false
            },
            callback);
    }

    // get distance results
    function callback(response, status) {
        if (status != google.maps.DistanceMatrixStatus.OK) {
            $('#result').html(err);
        } else {
            var origin = response.originAddresses[0];
            var destination = response.destinationAddresses[0];
            if (response.rows[0].elements[0].status === "ZERO_RESULTS") {
                $('#result').html("Better get on a plane. There are no roads between " +
                    origin +
                    " and " +
                    destination);
            } else {
                //  alert(JSON.stringify(response.rows[0].elements[0].distance.value));
                // connection.invoke("TakeDistance", JSON.stringify(response.rows[0].elements[0].distance.value + ',' + JSON.stringify(response.rows[0].elements[0].duration.value)));


                // connection.invoke("TakeDistance", JSON.stringify(response.rows[0].elements[0].duration.value));
                orderDistance = response.rows[0].elements[0].distance.text;
                orderETA = response.rows[0].elements[0].duration.text;

                console.log(orderDistance);
                console.log(orderETA);

                // TODO Put real values in here
                appendOrder(orderId, orderDistance, orderETA, "5:00");
            }
        }
    }

}


//
// Browser notification
//
function notifyMe() {
    // check if the browser supports notifications
    if (!("Notification" in window)) {
        alert("This browser does not support desktop notification");
    }

    //  check whether notification permissions have already been granted
    else if (Notification.permission === "granted") {
        // If it's okay let's create a notification
        var notification = new Notification(`${userName} have a new order`);
    }

    // Otherwise ask the user for permission
    else if (Notification.permission !== "denied") {
        Notification.requestPermission().then(function (permission) {
            // If the user accepts create a notification
            if (permission === "granted") {
                var notification = new Notification(`${userName} have a new order`);
            }
        });
    }
}


//
// Order list manipulation
//
// append order item
var appendOrder = function (orderId, distance, eta, timer) {
    var orderList = document.getElementById("orderList");
    orderList.innerHTML += orderItemHtml(orderId, distance, eta, timer);
}

// remove order item
var removeOrder = function (orderId) {
    var orderToRemove = document.getElementById(orderId);
    orderToRemove.parentNode.removeChild(orderToRemove);
}

// remove all orders form list
var clearOrderList = function () {
    var orderList = document.getElementById("orderList");
    var t = true;
    while (t) {
        if (orderList.lastChild.nodeName == "A") {
            orderList.removeChild(orderList.lastChild);
        } else {
            t = false;
        }
    }
}

// order item html
var orderItemHtml = function (orderId, distance, eta, timer) {
    return `<a id="${orderId}" class="list-group-item list-group-item-action bg-bone-3d mb-1 flex-column align-items-start" style="cursor:pointer">
                    <div class="d-flex justify-content-between">
                        <div>
                            <span class="d-block"><strong>Id</strong></span>
                            <span class="d-block" id="orderId">${orderId}</span>
                        </div>
                        <div>
                            <span class="d-block"><strong>Distance</strong></span>
                            <span class="d-block" id="orderDistance">${distance}</span>
                        </div>
                        <div>
                            <span class="d-block"><strong>ETA</strong></span>
                            <span class="d-block" id="orderETA">${eta}</span>
                        </div>
                        <div>
                            <span class="d-block"><strong>Timer</strong></span>
                            <span class="d-block" id="orderTimer">${timer}</span>
                        </div>
                    </div>
                </a>`
}

// No order item html
var noOrdersHtml = function () {
    return `<div id="noOrdersText" class="d-flex justify-content-center">
                <div>
                    <span class="d-block"><strong>No available orders.</strong></span>
                </div>
            </div>`
}

// custom remove button logic for testing
var removeOrderBtn = document.getElementById("removeOrderBtn");
removeOrderBtn.addEventListener("click", () => {
    var orderId = document.getElementById("orderToRemove").value;
    removeOrder(orderId);
});


//
// Open order details modal logic
//

// Find orderId based on click on specific order list item
connection.on("GetOrder", (business, order) => {

    console.log(business);
    console.log(order);

    var tStampF = new Date(order.tstamp);
    var tStamp = `${tStampF.getHours()}:${tStampF.getMinutes()} - ${tStampF.toDateString()}`;

    document.getElementById("modal-restaurantTitle").innerText = business.title;
    document.getElementById("modal-timestamp").innerText = tStamp;
    document.getElementById("modal-restaurantAddress").innerText = business.address;
    document.getElementById("modal-price").innerText = order.price + "€";
    document.getElementById("modal-orderId").value = order.id;
    if (order.paymentTypeId == "0") {
        document.getElementById("modal-paymentType").innerText = "Cash";
    } else {
        document.getElementById("modal-paymentType").innerText = "Credit Card";
    }

});


// Add event listener for all order items
document.getElementById("orderList").addEventListener("click", (e) => {

    var orderId = e.target.offsetParent.id;
    connection.invoke("GetOrderFromDb", orderId);

    $("#orderModal").modal('show');

});

//
// Background timer that checks for timed-out orders
//
var orderWatcher = setInterval(function () {
    var orderList = document.getElementById("orderList").getElementsByTagName("a");

    for (var i = 0; i < orderList.length; i++) {
        connection.invoke("CheckOrderTimeout", orderList[i].id).catch(function (err) {
            return console.error(err.toString());
        });
    }
}, 1000);

connection.on("CheckOrderTimeout",
    (orderId, tStamp, isTimedOut) => {

        var timeoutSpanInMillis = 60000;

        function millisToMinutesAndSeconds(millis) {
            var minutes = Math.floor(millis / 60000);
            var seconds = ((millis % 60000) / 1000).toFixed(0);
            return minutes + ":" + (seconds < 10 ? '0' : '') + seconds;
        }

        // If timed out remove from list
        if (isTimedOut) {
            document.getElementById(orderId).classList.add("animate");
            document.getElementById(orderId).addEventListener("animationend", function (event) {
                document.getElementById(orderId).remove();
            }, false);

            // else update timer on order list item
        } else {
            var timeNow = new Date();
            var parsedTime = new Date(tStamp);
            var diff = timeNow - parsedTime;

            var orderItemFields = document.getElementById(orderId).getElementsByTagName("span");

            for (var i = 0; i < orderItemFields.length; i++) {
                if (orderItemFields[i].id === "orderTimer") {
                    var toDisp = millisToMinutesAndSeconds(timeoutSpanInMillis - diff);
                    orderItemFields[i].innerText = (diff < timeoutSpanInMillis) ? toDisp : "0:00";
                }
            }
        }
    });


//
//functions that invoke the server in order to change status of accepted orders(accepted,pickedup,delivered
//

//When the deliverer accepts the order this function is called
function onOrderAccepted() {
    var orderId = document.getElementById("modal-orderId").value;
    console.log("The order accepted has order Id:" + orderId);
    connection.invoke("OrderAccepted", orderId).catch(function (err) {//invoke server to method OrderAccepted
        return console.error(err.toString());
    });
}

//When an order is taken by an other deliverer
connection.on("AnOrderIsAccepted", (orderId) => {
    removeOrder(orderId);
});

// whne the accepted button is clicked
connection.on("OnOrderAccepted", (order) => {
    console.log(order);


    // hide order info modal
    $("#orderModal").modal('hide');

    // update "accepted modal" info
    connection.invoke("GetInfoForIndiModalAccepted", order.id).catch(function (err) {
        return console.error(err.toString());
    });

    // show order "accepted modal"
    $("#orderAcceptedModal").modal({
        backdrop: 'static',
        keyboard: false
    });
});

// when the picked button is clicked
function onOrderPicked() {
    console.log("order pickedup");

    // Get order id from order info modal
    var orderId = document.getElementById("modal-orderId").value;

    // invoke method on server to inform of pick up
    connection.invoke("OrderPickedUp", orderId).catch(function (err) {
        return console.error(err.toString());
    });

    // hide accepted modal
    $("#orderAcceptedModal").modal('hide');

    // update "picked modal" info
    connection.invoke("GetInfoForIndiModalPicked", orderId).catch(function (err) {
        return console.error(err.toString());
    });

    // open "picked modal"
    $("#orderPickedModal").modal({
        backdrop: 'static',
        keyboard: false
    });
};


function onOrderDelivered() {
    console.log("order delivered");

    // Get order id from order info modal
    var orderId = document.getElementById("modal-orderId").value;

    // invoke method on server to inform of pick up
    connection.invoke("OrderDelivered", orderId).catch(function (err) {
        return console.error(err.toString());
    });

    // hide accepted modal
    $("#orderPickedModal").modal('hide');

    removeOrder(orderId);
};


// update functions for modal fields
function updateModalOrderIdHidden(orderId) {
    document.getElementById("modal-orderId").value = orderId;
    document.getElementById("modalAccepted-orderId").value = orderId;
    document.getElementById("modalPicked-orderId").value = orderId;
};

// on update of info in modal accepted
connection.on("UpdateModalAcceptedInfo", (businessTitle, businessAddress, orderPrice, orderPaymentType) => {
    document.getElementById("modalAccepted-restaurantName").innerText = businessTitle;
    document.getElementById("modalAccepted-restaurantAddress").innerText = businessAddress;
    document.getElementById("modalAccepted-orderPrice").innerText = orderPrice + "€";
    if (orderPaymentType == "0")
        document.getElementById("modalAccepted-orderPaymentType").innerText = "Cash";
    else
        document.getElementById("modalAccepted-orderPaymentType").innerText = "Credit Card";
});

// on update of info in modal picked
connection.on("UpdateModalPickedInfo",
    (customerName, customerAddress, customerDoorName, customerFloorNo, customerPhoneNumber, orderPrice, orderPaymentType) => {
        document.getElementById("modalPicked-customerName").innerText = customerName;
        document.getElementById("modalPicked-customerAddress").innerText = customerAddress;
        document.getElementById("modalPicked-customerDoorName").innerText = customerDoorName;
        document.getElementById("modalPicked-customerFloorNo").innerText = customerFloorNo;
        document.getElementById("modalPicked-customerPhoneNumber").innerText = customerPhoneNumber;
        document.getElementById("modalPicked-orderPrice").innerText = orderPrice + "€";
        if (orderPaymentType == "0")
            document.getElementById("modalPicked-orderPaymentType").innerText = "Cash";
        else
            document.getElementById("modalPicked-orderPaymentType").innerText = "Credit Card";
    });