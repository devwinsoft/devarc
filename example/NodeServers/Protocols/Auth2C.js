const msgpack = require('msgpack-lite');
const Common = require('./Common.js');
const STAT_TYPE = Common.STAT_TYPE;
const VECTOR3 = Common.VECTOR3;
const ErrorType = Common.ErrorType;
const CommonResult = Common.CommonResult;
const CustomSigninResult = Common.CustomSigninResult;
const GoogleCodeResult = Common.GoogleCodeResult;
const GoogleSigninResult = Common.GoogleSigninResult;
const GoogleRefreshResult = Common.GoogleRefreshResult;
const mHandlers = {};
class NotifySession
{
	/**
	 * @param {string} sessionID - string
	 * @param {int} secret - int
	 * @param {ErrorType} errorCode - ErrorType
	 */
	constructor() {
		this.sessionID = "";
		this.secret = 0;
		this.errorCode = 0;
	}
	Init(packet) {
		this.sessionID = packet[0];
		this.secret = packet[1];
		this.errorCode = packet[2];
	}
	ToArray() {
		const data =
		[
			this.sessionID,
			this.secret,
			this.errorCode,
		];
		return data;
	}
}

class NotifyLogin
{
	/**
	 * @param {string} sessionID - string
	 * @param {int} secret - int
	 * @param {ErrorType} errorCode - ErrorType
	 */
	constructor() {
		this.sessionID = "";
		this.secret = 0;
		this.errorCode = 0;
	}
	Init(packet) {
		this.sessionID = packet[0];
		this.secret = packet[1];
		this.errorCode = packet[2];
	}
	ToArray() {
		const data =
		[
			this.sessionID,
			this.secret,
			this.errorCode,
		];
		return data;
	}
}

class NotifyLogout
{
	/**
	 * @param {ErrorType} errorCode - ErrorType
	 */
	constructor() {
		this.errorCode = 0;
	}
	Init(packet) {
		this.errorCode = packet[0];
	}
	ToArray() {
		const data =
		[
			this.errorCode,
		];
		return data;
	}
}

class NotifySignin
{
	/**
	 * @param {string} sessionID - string
	 * @param {int} secret - int
	 * @param {ErrorType} errorCode - ErrorType
	 */
	constructor() {
		this.sessionID = "";
		this.secret = 0;
		this.errorCode = 0;
	}
	Init(packet) {
		this.sessionID = packet[0];
		this.secret = packet[1];
		this.errorCode = packet[2];
	}
	ToArray() {
		const data =
		[
			this.sessionID,
			this.secret,
			this.errorCode,
		];
		return data;
	}
}

class NotifyError
{
	/**
	 * @param {ErrorType} errorCode - ErrorType
	 */
	constructor() {
		this.errorCode = 0;
	}
	Init(packet) {
		this.errorCode = packet[0];
	}
	ToArray() {
		const data =
		[
			this.errorCode,
		];
		return data;
	}
}

module.exports =
{ NotifySession
, NotifyLogin
, NotifyLogout
, NotifySignin
, NotifyError
}

function createPacket(packetName, content)
{
	switch (packetName)
	{
	case 'NotifySession':
		{
			const obj = new NotifySession();
			obj.Init(content);
			return obj;
		}
	case 'NotifyLogin':
		{
			const obj = new NotifyLogin();
			obj.Init(content);
			return obj;
		}
	case 'NotifyLogout':
		{
			const obj = new NotifyLogout();
			obj.Init(content);
			return obj;
		}
	case 'NotifySignin':
		{
			const obj = new NotifySignin();
			obj.Init(content);
			return obj;
		}
	case 'NotifyError':
		{
			const obj = new NotifyError();
			obj.Init(content);
			return obj;
		}
		default:
			return null;
	}
}

function toArray(list)
{
	var result = [];
	for (let i = 0; i < list.length; i++)
	{
		result.push(list[i].ToArray());
	}
	return result;
}
function unpack(buf)
{
    var len = buf.length;
    var type_len = new Uint32Array(buf.slice(0, 2))[0];
    var type_name = buf.slice(2, 2 + type_len).toString();
    var data = buf.slice(2 + type_len);
    var content = msgpack.decode(data);

    return createPacket(type_name, content);
}

module.exports.pack = (obj) =>
{
    var type_name = obj.constructor.name;
    var buf_length = Buffer.alloc(2);
    var buf_name = Buffer.alloc(type_name.length);
    var buf_data = msgpack.encode(obj.ToArray());

    buf_length.writeUInt16LE(type_name.length, 0);
    //buf_length.writeUInt16BE(type_name.length, 0);
    buf_name.write(type_name);

    var arr = new Buffer.alloc(buf_length.length + buf_name.length + buf_data.length);
    var offset = 0;
    arr.set(buf_length, offset); offset += buf_length.length;
    arr.set(buf_name, offset); offset += buf_name.length;
    arr.set(buf_data, offset);
    return arr;
}

module.exports.on = (packetName, callback) =>
{
	mHandlers[packetName] = callback;
}

module.exports.dispatch = (packet, p1, p2) =>
{
	var obj = unpack(packet);
    if (obj == null) return;
	var type_name = obj.constructor.name;
	var handler = mHandlers[type_name];
	if (handler != undefined)
	{
		handler(obj, p1, p2);
	}
}
