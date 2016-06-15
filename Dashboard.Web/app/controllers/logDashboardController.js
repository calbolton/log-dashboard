
$(function () {
    console.log($.connection);
    var ticker = $.connection.log; // the generated client-side hub proxy

   
    function init() {
        //return ticker.server.getAllStocks().done(function (stocks) {
        //    $stockTableBody.empty();
        //    $stockTickerUl.empty();
        //    $.each(stocks, function () {
        //        var stock = formatStock(this);
        //        $stockTableBody.append(rowTemplate.supplant(stock));
        //        $stockTickerUl.append(liTemplate.supplant(stock));
        //    });
        //});
    }

    // Add client-side hub methods that the server will call
    $.extend(ticker.client, {
        logsAdded: function (logs) {
            console.log(logs);
        }
    });

    // Start the connection
    $.connection.hub.start()
        .then(init);
});