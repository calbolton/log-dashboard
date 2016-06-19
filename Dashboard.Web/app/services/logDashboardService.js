(function() {
    angular.module("logDashboard")
        .service("logDashboardService", ['logDashboardModel', logDashboardService]);

    function logDashboardService(logDashboardModel) {
        var service = this;

        var isDashBoardStarted = false;

        var scopes = [];

        service.getAdjuvantLogsForMicroService = function(microService) {
            return _.filter(logDashboardModel.adjuvantErrorLogs, function (log) {
                return log.MicroService === microService;
            });
        }

        service.getMicroServiceLogs = function(logName) {
            var msLog = _.find(logDashboardModel.microServiceLogs, function(log) {
                return log.name === logName;
            });

            return msLog;
        }

        service.startDashboard = function(scope) {
            scopes.push(scope);
            if (isDashBoardStarted) {
                return;
            }

            $(function () {
            console.log("JQURY INITIALIZED");
            console.log($.connection);
            var ticker = $.connection.log; // the generated client-side hub proxy

                // Add client-side hub methods that the server will call
            $.extend(ticker.client, {
                logsAdded: function (logs) {
                    logDashboardModel.addLogs(logs);
                    _.each(scopes, function (scope) {
                        scope.$apply();
                    });
                }
            });

            function init() {
                console.log("Getting all logs");
                ticker.server.getAllLogs().done(function (logs) {
                    console.log("All logs retrieved");
                    
                    logDashboardModel.addLogs(logs);
                    _.each(scopes, function (scope) {
                        scope.$apply();
                    });
                });
            }


            // Start the connection
            $.connection.hub.start()
                .then(init);
        });
        }

        return service;
    }
})();