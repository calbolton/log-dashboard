
(function () {
    'use strict';

    var app = angular.module('logDashboard');

    app.controller('logDashboardController', [
        '$scope',
        logDashboardController
    ]);

    function logDashboardController(
        $scope) {

        $scope.azureLogs = [];
        $scope.adjuvantErrorLogs = [];
        $scope.adjuvantGlobalLogs = [];
        $scope.adjuvantInformationalLogs = [];
        $scope.adjuvantOtherLogs = [];

        $scope.clearLogs = function() {
            $scope.azureLogs.length = 0;
            $scope.adjuvantErrorLogs.length = 0;
            $scope.adjuvantGlobalLogs.length = 0;
            $scope.adjuvantInformationalLogs.length = 0;
            $scope.adjuvantOtherLogs.length = 0;
        };

        $scope.addLogs = function(logs){
            _.each(logs, (log) => {
                if (log.Log.MicroService === "UNCONVERTABLE") {

                    $scope.azureLogs.push(log);

                } else {
                    if (log.Log.Type === 1) { // Error
                        $scope.adjuvantErrorLogs.push(log.Log);
                    } else if (log.Log.Type === 3) { // Info
                        $scope.adjuvantInformationalLogs.push(log.Log);
                    } else if (log.Log.Type === 5) { // Global
                        $scope.adjuvantGlobalLogs.push(log.Log);
                    } else { // Fatal, debug, warning
                        $scope.adjuvantOtherLogs.push(log.Log);
                    }
                }
            });

            $scope.$apply();
        }

        $(function () {
            console.log("JQURY INITIALIZED");
            console.log($.connection);
            var ticker = $.connection.log; // the generated client-side hub proxy

            // Add client-side hub methods that the server will call
            $.extend(ticker.client, {
                logsAdded: function (logs) {
                    $scope.addLogs(logs);
                }
            });

            function init() {
                console.log("Getting all logs");
                ticker.server.getAllLogs().done(function (logs) {
                    $scope.clearLogs();

                    $scope.addLogs(logs);
                });
            }


            // Start the connection
            $.connection.hub.start()
                .then(init);
        });
    }
})();


/// <reference path="C:\Projects\Git\X8.Dashboard\Dashboard.Web\Scripts/underscore.js" />

