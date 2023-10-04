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
const GOOGLE_CLIENT_ID = process.env.GOOGLE_CLIENT_ID;
const GOOGLE_CLIENT_SECRET = process.env.GOOGLE_CLIENT_SECRET;
const GOOGLE_LOGIN_REDIRECT_URI = process.env.GOOGLE_LOGIN_REDIRECT_URI;
const GOOGLE_TOKEN_URL = 'https://oauth2.googleapis.com/token';
const GOOGLE_USERINFO_URL = 'https://www.googleapis.com/oauth2/v2/userinfo';

// Init express server
const https = require('https');
const express = require('express');
const session = require('express-session');
const app = express();
var bodyParser = require('body-parser');
app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());
//app.use(bodyParser.raw({type: 'multipart/form-data', limit : '2mb'}))
app.use(express.static('public'));

const wrapAsync = (fn) => {
    return (req, res, next) => {
        fn(req, res, next).catch(next);
    }
}
app.use((req, res, next) => { next(); });
app.use((err, req, res, next) => {
    res.status(500).send({ message: 'ServerError', error: err });
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
app.get('/login/login', wrapAsync(async (req, res) => {
    const { account_id, access_token } = req.query;
    console.log(`login: id=${account_id}`);

    if (!account_id || !access_token)
    {
        throw Error('login: Protocol error');
    }

    var selectQuery = `SELECT access_token FROM account WHERE account_id='${account_id}' LIMIT 1;`;
    await mysqlConn.query(selectQuery, (err, result) =>
    {
        if (err)
        {
            throw err;
        }

        if (result[0].access_token == access_token)
        {
            res.send(access_token);
        }
        else
        {
            res.send('');
        }
    });
}));

app.get('/login/redirect', wrapAsync(async (req, res) => {
    const { code, state } = req.query;
    console.log(`redirect: code=${code}`);

    redisClient.set(`code:${state}`, code, {'EX': 60});

    res.redirect('/google_login_success.html');
}));

app.get('/login/code', wrapAsync(async (req, res) => {
    const { state } = req.query;
    console.log(`code: state=${state}`);

    if (!state)
    {
        throw new Error('code: Protocol error.');
    }

    const code = await redisClient.get(`code:${state}`);
    redisClient.del(`code:${state}`);
    res.send(code);
}));

app.get('/login/signin', wrapAsync(async (req, res) => {
    const { code, code_verifier } = req.query;
    console.log(`signin: code=${code}`);
    console.log(`signin: code_verifier=${code_verifier}`);
    console.log(`signin: GOOGLE_LOGIN_REDIRECT_URI=${GOOGLE_LOGIN_REDIRECT_URI}`);

    if (!code || !code_verifier)
    {
        throw new Error('code: Protocol error.');
    }

    const resp_token = await axios.post(GOOGLE_TOKEN_URL, {
        code,
        client_id: GOOGLE_CLIENT_ID,
        client_secret: GOOGLE_CLIENT_SECRET,
        redirect_uri: GOOGLE_LOGIN_REDIRECT_URI,
        code_verifier: code_verifier,
        scope: 'openid email profile',
        grant_type: 'authorization_code',
    });

    const access_token = resp_token.data.access_token;
    const resp_info = await axios.get(GOOGLE_USERINFO_URL, {
        headers: { Authorization: `Bearer ${access_token}` }
    });

    const account_id = resp_info.data.email;
    var updateQuery = `UPDATE account SET access_token='${access_token}', login_time=NOW() WHERE account_id='${account_id}' LIMIT 1;`;
    mysqlConn.query(updateQuery, async (err, result) =>
    {
        if (err)
        {
            throw err;
        }
        
        if (result.affectedRows == 0)
        {
            var insertQuery = `INSERT INTO account(account_id, account_type, access_token, login_time, create_time) VALUES ('${account_id}', 'google', '${access_token}', NOW(), NOW());`;
            await mysqlConn.query(insertQuery, (err, result) =>
            {
                if (err)
                {
                    throw err;
                }
                res.send(access_token);
            });
        }
        else
        {
            res.send(access_token);
        }
    });
}));


/*
 * AuthServer API
 *
 */
app.get('/msgpack', sessions, (req, res) =>
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

app.post('/msgpack', sessions, (req, res) =>
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
        var queryStr = `UPDATE account SET session_id='${req.sessionID}', login_time=NOW() WHERE account_id='${obj.accountID}' AND access_token=MD5('${password}') LIMIT 1;`;
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
                    req.session.login = true;
                    req.session.secret = secret;
                    req.session.save();
    
                    packet.errorCode = Common.ErrorType.SUCCESS;
                    packet.sessionID = req.sessionID;
                    packet.secret = req.session.secret;
                }
                const encoded = Auth2C.pack(packet);
                res.send(encoded);
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
                // res.writeHead(200, {
                //     'Set-Cookie': `secret=0`,
                //     'Content-Type': 'text/html; charset=utf-8',
                // });
                res.send(encoded);
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
        var queryStr = `INSERT INTO account(account_id, username, access_token, session_id, login_time, create_time) VALUES ('${obj.accountID}', '', MD5('${password}'), '${req.sessionID}', NOW(), NOW());`;
        mysqlConn.query(queryStr,
            (err, result) => {
                var packet = new Auth2C.NotifySignin(Common.ErrorType.UNKNOWN, '', 0);
                if (err)
                {
                }
                else if (result.affectedRows > 0)
                {
                    var secret = 1 + Math.floor(2147483646 * Math.random());
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

