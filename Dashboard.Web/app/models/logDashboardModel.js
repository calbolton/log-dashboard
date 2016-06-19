(function() {

    angular.module("logDashboard")
        .factory("logDashboardModel", [logDashboardModel]);
        
    function logDashboardModel() {
        var model = this;

        // Logs containers
        model.microServiceLogs = []; // All micro service logs
        model.allLog = {}; // Micro service log for all logs


        // Methods
        var createLevelLog = function(name) {
            return {
                name: name,
                isBeingViewed: true,
                hasError: false,
                logs: [],
                reset: function () {
                    this.logs.length = 0;
                    this.hasError = false;
                    this.isBeingViewed = true;
                },
                addLog: function (log) {
                    this.hasError |= log.Log.Type === "Error" || log.Log.Type === "Fatal";
                    this.logs.push(log);
                }
            };
        }

        var createMicroServiceLog = function (name) {
            var msLog = {
                name: name,
                levelLogs: [],
                logs: [],
                hasError: false,
                filteredLogs: [],
                reset: function () {
                    this.hasError = false;
                    this.logs.length = 0;
                    this.filteredLogs.length = 0;
                    _.each(this.levelLogs, function(fLog) {
                        fLog.reset();
                    });
                },
                addLog: function (log) {

                    // Add to level logs
                    var existingLevelLog = _.find(this.levelLogs, (levelLog) => {
                        return levelLog.name === log.Log.Type.toString();
                    });

                    if (!existingLevelLog) {
                        existingLevelLog = createLevelLog(log.Log.Type.toString());
                        this.levelLogs.push(existingLevelLog);
                    }

                    if (existingLevelLog.isBeingViewed === undefined || existingLevelLog.isBeingViewed) {
                        this.filteredLogs.push(log);
                    }

                    existingLevelLog.addLog(log);

                    this.hasError |= log.Log.Type === "Error" || log.Log.Type === "Fatal";

                    // Add to all logs
                    this.logs.push(log);
                },
                getLevelLog: function(levelLogName) {
                    return _.find(this.levelLogs, function (fLevelLog) {
                        return fLevelLog.name === levelLogName;
                    });
                },
                refreshFilteredLogs: function() {
                    this.filteredLogs.length = 0;
                    var self = this;
                    _.each(this.levelLogs, function(levelLog) {
                        if (levelLog.isBeingViewed) {
                            self.filteredLogs = self.filteredLogs.concat(levelLog.logs);
                        }
                    });
                }
            };

            // Add default levels
            msLog.levelLogs.push(createLevelLog("Fatal"));
            msLog.levelLogs.push(createLevelLog("Error"));
            msLog.levelLogs.push(createLevelLog("Warn"));
            msLog.levelLogs.push(createLevelLog("Info"));
            msLog.levelLogs.push(createLevelLog("Debug"));
            msLog.levelLogs.push(createLevelLog("Global"));

            return msLog;
        }
        
        var addMicroServiceLog = function(name) {
            var newMicroServiceLog = createMicroServiceLog(name);
            model.microServiceLogs.push(newMicroServiceLog);
            return newMicroServiceLog;
        }

        model.addLogs = function (logs) {
            _.each(logs, (log) => {
                var existingMicroServiceLog = _.find(model.microServiceLogs, function(msLog) {
                    return msLog.name === log.Log.MicroService;
                });

                if (!existingMicroServiceLog) {
                    existingMicroServiceLog = addMicroServiceLog(log.Log.MicroService);
                }

                existingMicroServiceLog.addLog(log);
                // Add to all logs
                model.allLog.addLog(log);
            });
        }

        model.reset = function() {
            _.each(model.microServiceLogs, function(msModel) {
                msModel.reset();
            });
            model.allLog.reset();
        }

        var init = function() {
            // Add default
            addMicroServiceLog("UserInterface");
            addMicroServiceLog("Azure");
            addMicroServiceLog("BusinessPartners");
            addMicroServiceLog("Funds");
            addMicroServiceLog("Goods");
            addMicroServiceLog("Auth");
            addMicroServiceLog("Cashier");
            model.allLog = createMicroServiceLog("All");
        }

        init();

        return model;
    }
})();