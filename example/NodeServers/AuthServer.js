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

// Init google
const axios = require('axios');
const fs = require('fs');
const GOOGLE_AUTH_URL = 'https://accounts.google.com/o/oauth2/v2/auth';
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


function logHandler(err, req, res, next) {
    console.error('[' + new Date() + ']\n' + err.stack);
    next(err);
}
  
function errorHandler(err, req, res, next) {
    res.status(err.status || 500);
    res.send(err.message || 'Server Error:');
}

app.use(logHandler);
app.use(errorHandler);

const wrapAsync = (fn) => {
    return async (req, res, next) => {
        try {
            await fn(req, res, next);
        }
        catch (err) {
            next(err);
        }
    };
}

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
 * Custom Login
 *
 */
app.get('/custom/register', wrapAsync(async (req, res) => {
    const { account_id, passwd } = req.query;
    if (!account_id || !passwd)
    {
        var packet = new Common.CommonResult();
        packet.errorCode = Common.ErrorType.PROTOCOL_ERROR;
        res.json(packet);
        return;
    }
    console.log(`custom/register: id=${account_id}`);

    var secret = 1 + Math.floor(2147483646 * Math.random());
    var passwd_md5 = cryptUtil.decrypt(passwd);
    var queryStr = `INSERT INTO account(account_id, account_type, passwd, secret, login_time, create_time) VALUES ('${obj.accountID}', 'custom', MD5('${passwd_md5}'), '${secret}', NOW(), NOW());`;
    mysqlConn.query(queryStr, (err, result) => {
        if (err)
        {
            var packet = new Common.CommonResult();
            packet.errorCode = Common.ErrorType.DATABASE_ERROR;
            res.json(packet);
        }
        else if (result.affectedRows == 0)
        {
            var packet = new Common.CommonResult();
            packet.errorCode = Common.ErrorType.DATABASE_ERROR;
            res.json(packet);
        }
        else if (result.affectedRows > 0)
        {
            res.send(secret);
        }
    });
}));

app.get('/custom/signin', wrapAsync(async (req, res) => {
    const { account_id, passwd } = req.query;
    console.log(`custom/signin: account_id=${account_id}`);

    var secret = 1 + Math.floor(2147483646 * Math.random());
    var password = cryptUtil.decrypt(passwd);
    var queryStr = `UPDATE account SET secret='${secret}', login_time=NOW() WHERE account_id='${account_id}' AND passwd=MD5('${password}') LIMIT 1;`;
    mysqlConn.query(queryStr, (err, result) => {
        if (err)
        {
            var packet = new Common.CommonResult();
            packet.errorCode = Common.ErrorType.DATABASE_ERROR;
            res.json(packet);
        }
        else if (result.affectedRows == 0)
        {
            var packet = new Common.CommonResult();
            packet.errorCode = Common.ErrorType.INVALID_PASSWORD;
            res.json(packet);
        }
        else
        {
            var packet = new Common.CustomSigninResult();
            packet.errorCode = Common.ErrorType.SUCCESS;
            packet.secret = secret;
            res.json(packet);
        }
    });
}));


/*
 * Google Login
 *
 */
app.get('/google/login', (req, res) => {
    const { state } = req.query;
    let url = GOOGLE_AUTH_URL;
    url += `?client_id=${process.env.GOOGLE_CLIENT_ID}`
    url += `&redirect_uri=${process.env.GOOGLE_LOGIN_REDIRECT_URI}`
    url += '&response_type=code'
    url += '&scope=email profile'
    url += `&state=${state}`
    res.redirect(url);
});

app.get('/google/redirect', wrapAsync(async (req, res) => {
    const { code, state } = req.query;
    console.log(`google/redirect: code=${code}`);

    redisClient.set(`code:${state}`, code, {'EX': 60});
    res.redirect('/google_login_success.html');
}));


app.get('/google/code', wrapAsync(async (req, res) => {
    const { state } = req.query;
    console.log(`google/code: state=${state}`);

    if (!state)
    {
        var packet = new Common.CommonResult();
        packet.errorCode = Common.ErrorType.PROTOCOL_ERROR;
        res.json(packet);
        return;
    }

    const code = await redisClient.get(`code:${state}`);
    redisClient.del(`code:${state}`);

    var packet = new Common.GoogleCodeResult();
    packet.errorCode = Common.ErrorType.SUCCESS;
    packet.code = code;
    res.json(packet);
}));


app.get('/google/signin', wrapAsync(async (req, res) => {
    var { code, code_verifier, redirect_uri, access_token } = req.query;
    var refresh_token = '';
    var expires_in = 0;

    if (!access_token)
    {
        if (!code || !code_verifier || !redirect_uri)
        {
            var packet = new Common.CommonResult();
            packet.errorCode = Common.ErrorType.PROTOCOL_ERROR;
            res.json(packet);
            return;
        }

        console.log(`google/signin: code=${code}`);
        const resp_token = await axios.post(GOOGLE_TOKEN_URL, {
            code: code,
            client_id: process.env.GOOGLE_CLIENT_ID,
            client_secret: process.env.GOOGLE_CLIENT_SECRET,
            redirect_uri: redirect_uri,
            code_verifier: code_verifier,
            scope: 'openid email profile',
            grant_type: 'authorization_code',
        });
    
        access_token = resp_token.data.access_token;
        refresh_token = resp_token.data.refresh_token;
        expires_in = resp_token.data.expires_in;
    }
    else
    {
        console.log(`google/signin: access_token=${access_token}`);
    }

    // Get UserInfo
    const resp_info = await axios.get(GOOGLE_USERINFO_URL, {
        headers: { Authorization: `Bearer ${access_token}` }
    });

    var account_id = resp_info.data.email;
    var secret = 1 + Math.floor(2147483646 * Math.random());
    var resp = new Common.GoogleSigninResult();
    resp.errorCode = Common.ErrorType.SUCCESS,
    resp.account_id = account_id;
    resp.access_token = access_token;
    resp.refresh_toke = refresh_token;
    resp.expires_in = expires_in;
    resp.secret = secret;

    var updateQuery = `UPDATE account SET secret='${secret}', login_time=NOW() WHERE account_id='${account_id}' LIMIT 1;`;
    mysqlConn.query(updateQuery, async (err, result) =>
    {
        if (err)
        {
            var packet = new Common.CommonResult();
            packet.errorCode = Common.ErrorType.DATABASE_ERROR;
            res.json(packet);
            return;
        }
        
        if (result.affectedRows == 0)
        {
            var insertQuery = `INSERT INTO account(account_id, account_type, secret, login_time, create_time) VALUES ('${account_id}', 'google', '${secret}', NOW(), NOW());`;
            await mysqlConn.query(insertQuery, (err, result) =>
            {
                if (err)
                {
                    var packet = new Common.CommonResult();
                    packet.errorCode = Common.ErrorType.DATABASE_ERROR;
                    res.json(packet);
                    return;
                }
                res.json(resp);
            });
        }
        else
        {
            res.json(resp);
        }
    });
}));


app.get('/google/refresh', wrapAsync(async (req, res) => {
    var { refresh_token } = req.query;
    if (!refresh_token)
    {
        var packet = new Common.CommonResult();
        packet.errorCode = Common.ErrorType.PROTOCOL_ERROR;
        res.json(packet);
        return;
    }

    console.log(`google/refresh: code=${code}`);
    const resp_token = await axios.post(GOOGLE_TOKEN_URL, {
        grant_type: 'refresh_token',
        client_id: process.env.GOOGLE_CLIENT_ID,
        client_secret: process.env.GOOGLE_CLIENT_SECRET,
        refresh_token: refresh_token,
    });

    var resp = new Common.GoogleRefreshResult();
    resp.access_token = resp_token.data.access_token;
    resp.refresh_token = resp_token.data.refresh_token;
    resp.expires_in = resp_token.data.expires_in;
    res.json(resp);
}));


/*
 * Login with account_id, secret.
 *
 */
app.get('/login', wrapAsync(async (req, res) => {
    const { account_id, secret } = req.query;
    console.log(`login: account_id=${account_id}`);

    if (!account_id || !secret)
    {
        var packet = new Common.CommonResult();
        packet.errorCode = Common.ErrorType.PROTOCOL_ERROR;
        res.json(packet);
        return;
    }

    var selectQuery = `SELECT secret FROM account WHERE account_id='${account_id}' LIMIT 1;`;
    await mysqlConn.query(selectQuery, (err, result) =>
    {
        if (err)
        {
            var packet = new Common.CommonResult();
            packet.errorCode = Common.ErrorType.DATABASE_ERROR;
            res.json(packet);
        }
        else if (result[0].secret != secret)
        {
            var packet = new Common.CommonResult();
            packet.errorCode = Common.ErrorType.INVALID_SECRET;
            res.json(packet);
        }
        else
        {
            var packet = new Common.CommonResult();
            packet.errorCode = Common.ErrorType.SUCCESS;
            res.json(packet);
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
        res.status(404).send({
            message: 'Protocol error.'
        });
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
        res.status(404).send({
            message: 'Protocol error.'
        });
    }
});


// Message Handlers...
C2Auth.on('RequestSession', (obj, req, res) => {
    var packet = new Auth2C.NotifySession( {
        errorCode: Common.ErrorType.UNKNOWN
    });
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
        var packet = new Auth2C.NotifyLogin( {
            errorCode: Common.ErrorType.SESSION_REMAIN
        });
        const encoded = Auth2C.pack(packet);
        res.send(encoded);
    }
    else
    {
        var password = cryptUtil.decrypt(obj.password);
        var queryStr = `UPDATE account SET session_id='${req.sessionID}', login_time=NOW() WHERE account_id='${obj.accountID}' AND passwd=MD5('${password}') LIMIT 1;`;
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
                var packet = new Auth2C.NotifyLogout();
                packet.errorCode = Common.ErrorType.UNKNOWN;
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
        var packet = new Auth2C.NotifyLogout();
        packet.errorCode = Common.ErrorType.UNKNOWN;
        const encoded = Auth2C.pack(packet);
        res.send(encoded);
    }
    req.session.destroy();
});


C2Auth.on('RequestSignin', (obj, req, res) => {
    if (req.session.login || !obj.accountID || !obj.password)
    {
        var packet = new Auth2C.NotifySignin();
        packet.errorCode = Common.ErrorType.UNKNOWN;
        const encoded = Auth2C.pack(packet);
        res.send(encoded);
    }
    else
    {
        var password = cryptUtil.decrypt(obj.password);
        var queryStr = `INSERT INTO account(account_id, passwd, session_id, login_time, create_time) VALUES ('${obj.accountID}', MD5('${password}'), '${req.sessionID}', NOW(), NOW());`;
        mysqlConn.query(queryStr,
            (err, result) => {
                var packet = new Auth2C.NotifySignin();
                packet.errorCode = Common.ErrorType.UNKNOWN;
                if (err)
                {
                    console.log(err.message);
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

