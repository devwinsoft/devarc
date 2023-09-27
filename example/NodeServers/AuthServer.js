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

// Init google secret
const axios = require('axios');
const fs = require('fs');
const google_web_secret = JSON.parse(fs.readFileSync(process.env.GOOGLE_WEB_SECRET, 'utf8'));
const GOOGLE_CLIENT_ID = google_web_secret['web']['client_id'];
const GOOGLE_CLIENT_SECRET = google_web_secret['web']['client_secret'];
const GOOGLE_AUTH_URI = google_web_secret['web']['auth_uri'];
const GOOGLE_TOKEN_URL = google_web_secret['web']['token_uri'];
const GOOGLE_LOGIN_REDIRECT_URI = google_web_secret['web']['redirect_uris'][0];
const GOOGLE_USERINFO_URL = 'https://www.googleapis.com/oauth2/v2/userinfo';

// Init express server
const https = require('https');
const express = require('express');
const session = require('express-session');
const ConnectRedis = require('connect-redis').default;
const app = express();
var bodyParser = require('body-parser');
//app.use(bodyParser.raw({type: 'multipart/form-data', limit : '2mb'}))
app.use(express.urlencoded({ extended: false }));
app.use(express.static('public'));

var mNextAccountID = '1';

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
            maxAge: 35999995385,
            expires : new Date(Date.now() + 36000000000),
            //sameSite: "lax",
            secure: false
        }
        /*, genid: function(req) {
            var temp = mNextSessionID++ % 1000000;
            return temp.toString();
        }*/
    }));

// Init protocol.
const Common = require('./Protocols/Common.js');
const Auth2C = require('./Protocols/Auth2C.js');
const C2Auth = require('./Protocols/C2Auth.js');

app.get('/', (req, res) =>
{
	res.sendfile("public/index.html");
});

/*
 * Google Login
 *
 */
app.get('/login', (req, res) => {
    let url = GOOGLE_AUTH_URI;
    url += `?client_id=${GOOGLE_CLIENT_ID}`
    url += `&redirect_uri=${GOOGLE_LOGIN_REDIRECT_URI}`
    url += '&response_type=code'
    url += '&scope=email profile'
	res.redirect(url);
});

app.post('/login', (req, res) => {
    let url = GOOGLE_AUTH_URI;
    url += `?client_id=${GOOGLE_CLIENT_ID}`
    url += `&redirect_uri=${GOOGLE_LOGIN_REDIRECT_URI}`
    url += '&response_type=code'
    url += '&scope=email profile'
	res.redirect(url);
});

app.get('/login/redirect', async (req, res) => {
    const { code } = req.query;
    const resp = await axios.post(GOOGLE_TOKEN_URL, {
      	code,
        client_id: GOOGLE_CLIENT_ID,
        client_secret: GOOGLE_CLIENT_SECRET,
        redirect_uri: GOOGLE_LOGIN_REDIRECT_URI,
        grant_type: 'authorization_code',
    });

    console.log(`access_token: ${resp.data.access_token}`);
    const resp2 = await axios.get(GOOGLE_USERINFO_URL, {
        headers: { Authorization: `Bearer ${resp.data.access_token}` }
    });
});

/*
 * AuthServer API
 *
 */
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
C2Auth.on('RequestSession', (obj, req, res) => {
    var packet = new Auth2C.NotifySession(Common.ErrorType.UNKNOWN, '', 0);
    if (req.session.login)
    {
        packet.errorCode = Common.ErrorType.SUCCESS;
        packet.sessionID = req.sessionID;
        packet.secret = req.session.secret;
    }
    const encoded = Auth2C.pack(packet);
    res.send(encoded);
});


C2Auth.on('RequestLogin', (obj, req, res) => {
    if (req.session.login)
    {
        var packet = new Auth2C.NotifyLogin(Common.ErrorType.SESSION_REMAIN, '', 0);
        const encoded = Auth2C.pack(packet);
        res.send(encoded);
    }
    else
    {
        var password = cryptUtil.decrypt(obj.password);
        var queryStr = `UPDATE account SET session_id='${req.sessionID}', login_time=NOW() WHERE account_id='${obj.accountID}' AND password=MD5('${password}') LIMIT 1;`;
        mysqlConn.query(queryStr,
            (err, result) => {
                var packet = new Auth2C.NotifyLogin(Common.ErrorType.UNKNOWN, '', 0);
                if (result.affectedRows == 0)
                {
                    packet.errorCode = Common.ErrorType.INVALID_PASSWORD;
                }
                else
                {
                    var secret = 1 + Math.floor(2147483646 * Math.random());
                    res.writeHead(200, {
                        'Set-Cookie': `secret=${secret}`,
                        'Content-Type': 'text/html; charset=utf-8',
                    });
    
                    req.session.login = true;
                    req.session.secret = secret;
                    req.session.save();
    
                    packet.errorCode = Common.ErrorType.SUCCESS;
                    packet.sessionID = req.sessionID;
                    packet.secret = req.session.secret;
                }
                const encoded = Auth2C.pack(packet);
                res.end(encoded);
            });
    }
});


C2Auth.on('RequestLogout', (obj, req, res) => {
    if (req.session.login)
    {
        var queryStr = `UPDATE account SET session_id='' WHERE session_id='${req.sessionID}' LIMIT 1;`;
        mysqlConn.query(queryStr,
            (err, result) => {
                var packet = new Auth2C.NotifyLogout(Common.ErrorType.UNKNOWN);
                if (result.affectedRows == 1)
                {
                    packet.errorCode = Common.ErrorType.SUCCESS;
                }
                const encoded = Auth2C.pack(packet);
                res.writeHead(200, {
                    'Set-Cookie': `secret=0`,
                    'Content-Type': 'text/html; charset=utf-8',
                });
                res.end(encoded);
            });
    }
    else
    {
        var packet = new Auth2C.NotifyLogout(Common.ErrorType.UNKNOWN);
        const encoded = Auth2C.pack(packet);
        res.send(encoded);
    }
    req.session.destroy();
});


C2Auth.on('RequestSignin', (obj, req, res) => {
    if (req.session.login || !obj.accountID || !obj.password)
    {
        var packet = new Auth2C.NotifySignin(Common.ErrorType.UNKNOWN, '', 0);
        const encoded = Auth2C.pack(packet);
        res.send(encoded);
    }
    else
    {
        var password = cryptUtil.decrypt(obj.password);
        var queryStr = `INSERT INTO account(account_id, username, password, session_id, login_time, create_time) VALUES ('${obj.accountID}', '', MD5('${password}'), '${req.sessionID}', NOW(), NOW());`;
        mysqlConn.query(queryStr,
            (err, result) => {
                var packet = new Auth2C.NotifySignin(Common.ErrorType.UNKNOWN, '', 0);
                if (err)
                {
                }
                else if (result.affectedRows > 0)
                {
                    var secret = 1 + Math.floor(2147483646 * Math.random());
                    res.writeHead(200, {
                        'Set-Cookie': `secret=${secret}`,
                        'Content-Type': 'text/html; charset=utf-8',
                    });
    
                    req.session.login = true;
                    req.session.secret = secret;
                    req.session.save();
    
                    packet.errorCode = Common.ErrorType.SUCCESS;
                    packet.sessionID = req.sessionID;
                    packet.secret = req.session.secret;
                }
                const encoded = Auth2C.pack(packet);
                res.end(encoded);
            });
    }
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

const serverOption = {
    key: fs.readFileSync(process.env.SSL_KEY, 'utf8'),
    cert: fs.readFileSync(process.env.SSL_CERT, 'utf8')
};
const server = https.createServer(serverOption, app);
server.listen(process.env.HTTP_PORT, () =>
{
    console.log(`Server running at https://localhost:${process.env.HTTP_PORT}/`);
});

