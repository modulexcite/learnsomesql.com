(function () {
    var interactiveSql = angular.module('interactiveSql', []);

    var questions = JSON.parse(document.getElementById("questions-json").value);

    interactiveSql.controller("LessonController", function ($scope, $http) {
        function trackEvent(action) {
            var eventArgs = ["send", "event", "lesson", action, "Question " + questionIndex];
            if (window.location.hostname === "localhost") {
                if (window.console && console.log) {
                    console.log(eventArgs);
                }
            } else {
                ga.apply(null, eventArgs);
            }
        };
    
        $scope.query = "";
        $scope.setQuery = function (query) {
            $scope.query = query;
        };
        $scope.setExecutedQuery = function (query) {
            $scope.executedQuery = query;
        };
        $scope.results = null;
        $scope.setResults = function (results, isCorrectAnswer, hint) {
            $scope.results = results;
            $scope.isCorrectAnswer = isCorrectAnswer;
            $scope.hint = hint;
        };
        $scope.setError = function (error) {
            $scope.error = error;
        };
        $scope.resetResult = function () {
            $scope.setResults(null);
            $scope.setError(null);
            $scope.setExecutedQuery(null);
            $scope.setQuery(null);
        };

        $scope.runQuery = function () {
            var queryData = { "question-identifier": $scope.question.identifier, query: $scope.query };
            $http({ method: "POST", url: "/query", data:  queryData})
                .success(function (data, status, headers, config) {
                    $scope.setExecutedQuery(data.query);
                    $scope.setResults(data.results, data.isCorrectAnswer, data.hint);
                    $scope.setError(data.error);
                });
            trackEvent("run-query");
        };

        var questionIndex = 0;
        $scope.question = questions[questionIndex];

        $scope.nextQuestion = function () {
            questionIndex = (questionIndex + 1) % questions.length;
            $scope.question = questions[questionIndex];
            $scope.resetResult();
            trackEvent("next-question");
        };

        $scope.isLastQuestion = function() {
            return questionIndex + 1 >= questions.length;
        };
    });

    interactiveSql.controller("TaskController", function ($scope) {
        $scope.expectedAnswerIsVisible = false;
        $scope.showExpectedAnswer = function () {
            $scope.expectedAnswerIsVisible = true;
        };
        $scope.showAnswer = function () {
            $scope.setQuery($scope.question.correctQuery);
        };
    });

    interactiveSql.controller("ResultsController", function ($scope) {
    });

    interactiveSql.directive("queryResultsTable", function () {
        return {
            restrict: "E",
            replace: true,
            scope: true,
            link: function(scope, element, attrs) {
                if (isJson(attrs.data)) {
                    // If data is a constant value, then avoid re-evaluating it
                    scope.data = scope.$eval(attrs.data);
                } else {
                    scope.$watch(function() {
                        return scope.$parent.$eval(attrs.data);
                    }, function(newValue, oldValue) {
                        scope.data = newValue;
                    });
                }
            },
            template:
                '<div style="overflow-x: auto" ng-show="data">' +
                  '<table class="table table-bordered">' +
                    '<thead>' +
                      '<tr>' +
                        '<th ng-repeat="column in data.columnNames">{{column}}</th>' +
                      '</tr>' +
                    '</thead>' +
                    '<tbody>' +
                      '<tr ng-repeat="row in data.rows">' +
                        '<td ng-repeat="field in row">' +
                          '{{field}}' +
                        '</td>' +
                      '</tr>' +
                    '</tbody>' +
                  '</table>' +
                '</div>'
        };
    });
    
    function isJson(string) {
        try {
            JSON.parse(string);
            return true;
        } catch (e) {
            return false;
        }
    }
})();

