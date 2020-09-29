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
      const decodedData = decode(data);
      console.log(decodedData);
      this.position = { x: decodedData.inputX, y: decodedData.inputY };
    });
  };
}

module.exports = Player;
