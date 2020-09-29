const { encode, decode } = require("@msgpack/msgpack");

class Player {
  constructor(id, socket, position) {
    this.id = id;
    this.socket = socket;
    this.position = position;
  }

  handleEvents = () => {
    this.handleMovement();
  };

  handleMovement = () => {
    this.socket.on("MOVE", (data) => {
      console.log(decode(data));
      this.position = data;
    });
  };
}

module.exports = Player;
