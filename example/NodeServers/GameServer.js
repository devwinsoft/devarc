// Init config.
const dotenv = require('dotenv');
const fs = require('fs');
dotenv.config();

// Init express server
const express = require('express');
const session = require('express-session');
const app = express();

// Init redis
const redis = require('redis');
const redisClient = redis.createClient(
{
    host      : process.env.REDIS_HOST,
    port      : process.env.REDIS_PORT,
    password  : process.env.REDIS_PASS
});


// Init session
const ConnectRedis = require('connect-redis').default;
var sessions = session(
    {
        store: new ConnectRedis(
            {
                client: redisClient,
            }),
        secret : 'Rs89I67YEA55cLMgi0t6oyr8568e6KtD',
        resave: false,
        saveUninitialized: true,
        rolling : true,
        cookie: {
            maxAge: 86400000,
            expires : new Date(Date.now() + 86400000),
            secure: false
        }
    }
);


redisClient.on('connect', () =>
{
    console.info('Redis connected.');
});

redisClient.on('error', (err) =>
{
    console.error('Redis error:\n', err);
});


// Init WebSocket Server
const https = require('https');
const serverOption = {
    key: fs.readFileSync(process.env.SSL_KEY, 'utf8'),
    cert: fs.readFileSync(process.env.SSL_CERT, 'utf8')
};
const httpsServer = https.createServer(serverOption, function(req, res)
{
    res.writeHead(404);
    res.end();
});

const WebSocket = require('ws');
const wss = new WebSocket.Server(
    {
        server: httpsServer,
        autoAcceptConnections: true
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

    httpsServer.listen(process.env.WSS_PORT, () =>
    {
        console.log(`GameServer running at https://localhost:${process.env.WSS_PORT}/`);
    });
}
main();

