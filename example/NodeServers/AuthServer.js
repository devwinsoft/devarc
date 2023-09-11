// Init config.
const dotenv = require('dotenv');
dotenv.config();

// Init MySQL
const mysql = require('mysql');
const mysqlConn = mysql.createConnection(
{
    host     : process.env.MYSQL_HOST,
    user     : process.env.MYSQL_USER,
    password : process.env.MYSQL_PASS,
    database : process.env.MYSQL_DB
});


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


// Init crypto
const cryptUtil = require("./Util/CryptUtil.js");
const crypto = require('crypto');
const hash = crypto.createHash('sha256');

// Init express server
const express = require('express');
const session = require('express-session');
const ConnectRedis = require('connect-redis').default;
const app = express();
var bodyParser = require('body-parser');
//app.use(bodyParser.raw({type: 'multipart/form-data', limit : '2mb'}))
app.use(express.urlencoded({ extended: false }));

var mNextSessionID = '1';

// Init session
app.use(session(
    {
        store: new ConnectRedis(
            {
                client: redisClient,
            }),
        secret : 'Rs89I67YEA55cLMgi0t6oyr8568e6KtD',
        resave: false,
        saveUninitialized: true,
        cookie: {
            maxAge: 60000,
            //sameSite: "lax",
            secure: false
        }, genid: function(req) {
            var temp = mNextSessionID++ % 1000000;
            return temp.toString();
        }
    }));

// Init protocol.
const Common = require('./Protocols/Common.js');
const Auth2C = require('./Protocols/Auth2C.js');
const C2Auth = require('./Protocols/C2Auth.js');

app.get('/msgpack', (req, res) =>
{
    var packet = req.query.packet;
    try
    {
        var data = Buffer.from(packet, "base64");
        C2Auth.dispatch(data, req, res);
    }
    catch
    {
        res.send('');
    }
});

app.post('/msgpack', (req, res) =>
{
    var packet = req.body.packet;
    try
    {
        var data = Buffer.from(packet, "base64");
        C2Auth.dispatch(data, req, res);
    }
    catch
    {
        res.send('');
    }
});


// Message Handlers...
C2Auth.on('RequestLogin', (obj, req, res) => {
    var password = cryptUtil.decrypt(obj.password);
    var queryStr = `SELECT account_id FROM account WHERE account_id='${obj.accountID}' AND password=MD5('${password}');`;
    mysqlConn.query(queryStr,
        (err, rows, fields) => {
            var result = new Auth2C.NotifyLogin(Common.ErrorType.UNKNOWN, '', 0);
            if (err != null || rows.length == 0)
            {
                result.errorCode = Common.ErrorType.INVALID_PASSWORD;
            }
            else
            {
                req.session.secret = Math.floor(2147483647 * Math.random());
                req.session.save();

                result.errorCode = Common.ErrorType.SUCCESS;
                result.sessionID = req.sessionID;
                result.secret = req.session.secret;
            }
            const encoded = Auth2C.pack(result);
            res.send(encoded);
            console.log(result);
    });
});


// Connect to MySQL & Redis.
async function init()
{
    // Connect
    mysqlConn.connect((err) =>
    {
        if (err) throw err;
        console.log('Mysql connected.');
    });
    await redisClient.connect();
}
init();

app.listen(process.env.HTTP_PORT, process.env.HTTP_HOST, () =>
{
    console.log(`Server running at http://${process.env.HTTP_HOST}:${process.env.HTTP_PORT}/`);
});
