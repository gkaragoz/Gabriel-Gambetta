const { encode, decode } = require("@msgpack/msgpack");
const events = require("./events");

class Player {
  constructor(id, socket, position, speed) {
    this.id = id;
    this.socket = socket;
    this.position = position;
    this.speed = speed;
  }

  handleEvents = () => {
    this.handleMovement();
  };

  handleMovement = () => {
    this.socket.on(events.PLAYER_MOVE, (data) => {
      const decodedData = decode(data);
      const { additionX, additionY } = this.analyzeInput(decodedData);

      this.position.x += additionX;
      this.position.y += additionY;
    });
  };

  analyzeInput = ({ inputX, inputY }) => {
    return { additionX: inputX * this.speed, additionY: inputY * this.speed };
  };
}

module.exports = Player;
