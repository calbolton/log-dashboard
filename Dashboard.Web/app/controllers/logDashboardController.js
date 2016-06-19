
(function () {
    'use strict';

    var app = angular.module('logDashboard');

    app.controller('logDashboardController', [
        '$scope',
        'logDashboardModel',
        'logDashboardService',
        logDashboardController
    ]);

    function logDashboardController(
        $scope, logDashboardModel, logDashboardService) {

        // Dashboard model
        $scope.model = logDashboardModel;

        //----- Summary -----
        // Header properties
        $scope.currentMicroServiceLog = {};

        // Header methods
        $scope.selectMicroServiceLogs = function(logName) {
            $scope.currentMicroServiceLog = logDashboardService.getMicroServiceLogs(logName);
            $scope.filteredLogs.length = 0;
            _.each($scope.currentMicroServiceLog.levelLogs, function(fLog) {
                if (fLog.isBeingViewed === undefined || fLog.isBeingViewed) {
                    $scope.filteredLogs = $scope.filteredLogs.concat(fLog.logs);
                }  
            });
            $scope.currentMicroServiceLog.hasError = false;
        }

        // ----- Logs -----
        // Logs properties 
        $scope.filteredLogs = [];

        
        // Logs methods
        $scope.updateCurrentMicroserviceLogs = function (levelLogName) {
            var levelLog = $scope.currentMicroServiceLog.getLevelLog(levelLogName);

            levelLog.isBeingViewed = !levelLog.isBeingViewed;

            $scope.currentMicroServiceLog.refreshFilteredLogs();
        }

        $scope.resetLogs = function() {
            console.log('RESETTING');
            logDashboardModel.reset();
        }

        $scope.convertToJson = function(o) {
            return JSON.stringify(o);
        }

        // Adjuvant log modal
        $scope.currentLog = {};
        $scope.showingCurrentLog = false;
        $scope.showCurrentLog = function (log) {
            $scope.currentLog = log;
            $scope.showingCurrentLog = true;
        }

        $scope.hideCurrentLog = function() {
            $scope.showingCurrentLog = false;
        }

        $scope.getLogLevelClass = function(level) {
            if (level === 'Error' || level === 'Fatal' || level === 'Global') {
                return 'danger';
            } else if (level === 'Warn') {
                return 'warning';
            }

            return 'info';
        }
        
        // ----- Init -----
        var init = function () {
            logDashboardService.startDashboard($scope);
        }

        init();
    }
})();



