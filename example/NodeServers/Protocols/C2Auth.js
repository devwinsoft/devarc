const msgpack = require('msgpack-lite');
class RequestSession
{
	/**
	 */
	constructor() {
	}
	Init(packet) {
	}
	ToArray() {
		const data =
		[
		];
		return data;
	}
}

class RequestLogin
{
	/**
	 * @param {string} accountID - string
	 * @param {string} password - string
	 */
	constructor() {
		this.accountID = 0;
		this.password = 0;
	}
	Init(packet) {
		this.accountID = packet[0];
		this.password = packet[1];
	}
	ToArray() {
		const data =
		[
			this.accountID,
			this.password,
		];
		return data;
	}
}

class RequestLogout
{
	/**
	 */
	constructor() {
	}
	Init(packet) {
	}
	ToArray() {
		const data =
		[
		];
		return data;
	}
}

class RequestSignin
{
	/**
	 * @param {string} accountID - string
	 * @param {string} password - string
	 */
	constructor() {
		this.accountID = 0;
		this.password = 0;
	}
	Init(packet) {
		this.accountID = packet[0];
		this.password = packet[1];
	}
	ToArray() {
		const data =
		[
			this.accountID,
			this.password,
		];
		return data;
	}
}

module.exports =
{ RequestSession
, RequestLogin
, RequestLogout
, RequestSignin
}

function createPacket(packetName, content)
{
	switch (packetName)
	{
	case 'RequestSession':
		{
			const obj = new RequestSession();
			obj.Init(content);
			return obj;
		}
	case 'RequestLogin':
		{
			const obj = new RequestLogin();
			obj.Init(content);
			return obj;
		}
	case 'RequestLogout':
		{
			const obj = new RequestLogout();
			obj.Init(content);
			return obj;
		}
	case 'RequestSignin':
		{
			const obj = new RequestSignin();
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
