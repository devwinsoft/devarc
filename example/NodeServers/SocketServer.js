// Init config.
const dotenv = require('dotenv');
const fs = require('fs');
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
const http = require('http');
const WebSocket = require('ws');
const httpServer = http.createServer();
const wss = new WebSocket.Server(
    {
        server: httpServer
    }
);

wss.on('connection', (ws, request) =>
{
    ws.on('close', (code) => {
        console.log('Disconnected.');
    });

    ws.on('message', (packet) => {
        C2Game.dispatch(packet, ws);
    })
});


// Init protocol.
const Common = require('./Protocols/Common.js');
const Game2C = require('./Protocols/Game2C.js');
const C2Game = require('./Protocols/C2Game.js');
C2Game.on('RequestLogin', (obj, ws) =>
{
    redisClient.get(`sess:${obj.sessionID}`).then((json) => {
        var response = new Game2C.NotifyLogin();
        response.errorCode = Common.ErrorType.UNKNOWN;
        response.account = new Common.Account('', 0);

        var record = JSON.parse(json);
        if (record == null)
        {
            response.errorCode = Common.ErrorType.SESSION_EXPIRED;
        }
        else if (record.secret != obj.secret)
        {
            response.errorCode = Common.ErrorType.INVALID_SECRET;
        }
        else
        {
            response.errorCode = Common.ErrorType.SUCCESS;
        }

        var encoded = Game2C.pack(response);
        ws.send(encoded);
    });
});

async function main()
{
    // Connect
    await redisClient.connect();

    httpServer.listen(process.env.WS_PORT, () =>
    {
        console.log(`SocketServer running at https://localhost:${process.env.WS_PORT}/`);
    });
}
main();

