// Init config.
const dotenv = require('dotenv');
dotenv.config();


// Init Redis
const redis = require('redis');
const redisClient = redis.createClient(
{
    host      : process.env.REDIS_HOST,
    port      : process.env.REDIS_PORT,
    password  : process.env.REDIS_PASS
});
    
redisClient.on('connect', () =>
{
    console.info('Redis connected.');
});

redisClient.on('error', (err) =>
{
    console.error('Redis error:\n', err);
});


// Init WebSocket Server
const WebSocket = require('ws');
const wss = new WebSocket.Server(
    {
        port: process.env.SOCKET_PORT
    },
    () => { 
        console.log('Server starting.');
    }
);

// Init protocol.
const Defines = require('./Protocols/Defines.js');
const Game2C = require('./Protocols/Game2C.js');
const C2Game = require('./Protocols/C2Game.js');
C2Game.on('RequestLogin', (obj, ws) =>
{
    redisClient.get(`sess:${obj.sessionID}`).then((json) => {
        var response = new Game2C.NotifyLogin();
        response.errorCode = Defines.ErrorType.UNKNOWN;
        response.character = new Defines.Character('', 0);

        var record = JSON.parse(json);
        if (record == null)
        {
            response.errorCode = Defines.ErrorType.SESSION_EXPIRED;
        }
        else if (record.secret != obj.secret)
        {
            response.errorCode = Defines.ErrorType.INVALID_SECRET;
        }
        else
        {
            response.errorCode = Defines.ErrorType.SUCCESS;
        }

        var encoded = Game2C.pack(response);
        ws.send(encoded);
        // wss.clients.forEach(temp => {
        // });
        });
});

wss.on('connection', (ws, request) =>
{
    ws.on('close', (code) => {
        console.log('Disconnected.');
    });

    ws.on('message', (packet) => {
        C2Game.dispatch(packet, ws);
    })
});
 

async function StartSerer()
{
    // Connect
    await redisClient.connect();

    wss.on('listening', () =>
    {
        console.log(`Server running at http://localhost:${process.env.SOCKET_PORT}/`);
    });
}

StartSerer();