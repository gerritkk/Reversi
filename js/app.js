"use strict";

function _defineProperty(obj, key, value) { if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

var Game = function (url) {
  console.log(url);
  var stateMap = {
    gameState: 0
  }; //Configuratie en state waarden

  var configMap = {
    apiUrl: url
  }; // Private function init

  var privateInit = function privateInit(afterInit) {
    console.log(configMap.apiUrl);
    setInterval(gameState, 2, url);
    setTimeout(function () {
      afterInit(); // callback
    }, 500);
  };

  function gameState(url) {
    console.log("test");
    stateMap.gameState = Game.Model.getGameState(url);
  } // Waarde/object geretourneerd aan de outer scope


  return {
    init: privateInit
  };
}('/api/url');

$(document).ready(function () {
  console.log("ready!");
  $r = new FeedbackWidget('feedback-success');
});

var FeedbackWidget = /*#__PURE__*/function () {
  function FeedbackWidget(elementId) {
    _classCallCheck(this, FeedbackWidget);

    this._elementId = elementId;
  }

  _createClass(FeedbackWidget, [{
    key: "test",
    value: function test() {
      console.log("test");
    }
  }, {
    key: "show",
    value: function show(message, type) {
      if (type == "success") {
        $('#feedback-success').addClass('feedback-success');
        var x = document.getElementById('feedback-success');
        $('#feedback-success').text(message);
        x.style.display = "block";
      } else {
        $('#feedback-danger').addClass('feedback-danger');
        var x = document.getElementById('feedback-danger');
        $('#feedback-danger').text(message);
        x.style.display = "block";
      }

      var object = {
        message: message,
        type: type
      };
      this.log(object);
    }
  }, {
    key: "hide",
    value: function hide() {
      var x = document.getElementById(this._elementId);
      x.style.animation = "fadeOut ease 5s";
      setTimeout(function () {
        x.style.display = "none";
      }, 5000); // x.style.display = "none";
    }
  }, {
    key: "log",
    value: function log(message) {
      if (localStorage.length <= 10) {
        if (localStorage.getItem('feedback_widget') != null) {
          var array = JSON.parse(localStorage.getItem("feedback_widget"));
          array.push(message);
          localStorage.setItem("feedback_widget", JSON.stringify(array));
        } else {
          var array = [message];
          localStorage.setItem('feedback_widget', JSON.stringify(array));
        }
      } else {
        alert('Niet meer dan 10 berichten in de localstorage!');
      }
    }
  }, {
    key: "removeLog",
    value: function removeLog(type) {
      localStorage.removeItem(type);
    }
  }, {
    key: "history",
    value: function history() {
      var historie = JSON.parse(localStorage.getItem('feedback_widget'));
      var string = "";

      for (var i = 0; i < historie.length; i++) {
        var obj = historie[i];
        string += "<type ".concat(obj.type, "> - ").concat(obj.message, " \n"); // alert(obj.message + " " + obj.type);
      } // alert(string);


      return string;
    }
  }, {
    key: "elementId",
    get: function get() {
      //getter, set keyword voor setter methode
      return this._elementId;
    }
  }]);

  return FeedbackWidget;
}();

Game.Data = function () {
  var configMap = {
    apiKey: '38c54cfa93fde5b1a7a55e4c7f6943e0',
    mock: [{
      url: 'api/Spel/Beurt',
      data: 0
    }]
  };
  var stateMap = {
    environment: 'development',
    gameState: 0
  };

  var privateInit = function privateInit(environment) {
    if (environment == 'production' || environment == 'development') {
      stateMap.environment = environment;
    } else {
      new Error("environment moet production of development zijn!");
    } // if (environment == 'production') {
    //     // request aan server
    // } else if (environment == 'development') {
    //     // mock data teruggeven
    // }


    console.log("private Game.Data function.");
  };

  var get = function get(url) {
    if (stateMap.environment == 'production') {
      return $.get(url).then(function (r) {
        return r;
      })["catch"](function (e) {
        console.log(e.message);
      });
    } else if (stateMap.environment == 'development') {
      return getMockData(url);
    }

    return null;
  };

  var getMockData = function getMockData(url) {
    //filter mock data, configMap.mock ... oei oei, moeilijk moeilijk :-)
    var mockData = configMap.mock.filter(function (data) {
      return data['url'] == url;
    })[0];
    return new Promise(function (resolve, reject) {
      resolve(mockData);
    })["catch"](function (e) {
      console.log(e.message);
    });
  };

  return _defineProperty({
    init: privateInit,
    get: get,
    stateMap: stateMap
  }, "stateMap", stateMap);
}();

Game.Model = function () {
  var configMap = {}; // Private function init

  var privateInit = function privateInit() {
    console.log("private Game.Model function."); // var intervalID = window.setInterval(_getGameState, 2000); // gamestate elke 2 seconden
  };

  var getWeather = function getWeather(url) {
    return Game.Data.get(url).then(function (r) {
      if (r.main.temp != null) {
        return r;
      }
    })["catch"](function (e) {
      console.log(e.message);
    });
  };

  var _getGameState = function _getGameState() {
    //aanvraag via Game.Data
    var data = Game.Data.get('/api/Spel/Beurt/'); //controle of ontvangen data valide is

    if (data != 0 && data != 1 && data != 2) {
      return new Error("Waarde is ongelijk aan 0, 1 of 2!");
    }

    Game.Data.stateMap.gameState = data;
    return data;
  };

  return {
    init: privateInit,
    getWeather: getWeather,
    getGameState: _getGameState
  };
}();

Game.Reversi = function () {
  console.log('hallo, vanuit module Reversi');
  var configMap = {
    "test": "test"
  }; // Private function init

  var privateInit = function privateInit() {
    console.log(configMap.test);
  };

  return {
    init: privateInit
  };
}();