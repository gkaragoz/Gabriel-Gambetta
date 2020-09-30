// =============================================================================
//  An Entity in the world.
// =============================================================================
var Entity = function () {
  this.x = 0;
  this.speed = 2; // units/s
  this.position_buffer = [];
};

// Apply user's input to this entity.
Entity.prototype.applyInput = function (input) {
  this.x += input.press_time * this.speed;
};

// =============================================================================
//  A message queue with simulated network lag.
// =============================================================================
var LagNetwork = function () {
  this.messages = [];
};

// "Send" a message. Store each message with the timestamp when it should be
// received, to simulate lag.
LagNetwork.prototype.send = function (lag_ms, message) {
  this.messages.push({ recv_ts: +new Date() + lag_ms, payload: message });
};

// Returns a "received" message, or undefined if there are no messages available
// yet.
LagNetwork.prototype.receive = function () {
  var now = +new Date();
  for (var i = 0; i < this.messages.length; i++) {
    var message = this.messages[i];
    if (message.recv_ts <= now) {
      this.messages.splice(i, 1);
      return message.payload;
    }
  }
};