using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using MessagePack.Resolvers;

namespace Devarc
{
    public class GameUser : SocketUser
    {
        RedisClient mRedis = new RedisClient();

        public GameUser()
        {
            InitMessagePackResolvers(
                MessagePack.Resolvers.GeneratedResolver.Instance,
                MessagePack.Resolvers.StandardResolver.Instance
            );

            RegisterHandler<C2Game.RequestLogin>(onMessage);
        }


        protected override void OnOpen()
        {
            base.OnOpen();
            mRedis.Init("localhost");
        }


        bool getSessionSecret(string sessionID, out int value)
        {
            value = 0;
            using (var conn = mRedis.GetConnection())
            {
                string json = null;
                if (mRedis.GetString($"sess:{sessionID}", conn, out json) == false)
                {
                    return false;
                }
                var sessionData = JObject.Parse(json);
                var crc = sessionData["secret"].ToString();
                if (int.TryParse(crc, out value) == false)
                {
                    return false;
                }
            }
            return true;
        }


        void onMessage(C2Game.RequestLogin packet)
        {
            Game2C.NotifyLogin response = new Game2C.NotifyLogin();
            int secret;
            if (getSessionSecret(packet.sessionID, out secret) == false)
            {
                response.errorCode = ErrorType.SESSION_EXPIRED;
            }
            else if (secret != packet.secret)
            {
                response.errorCode = ErrorType.INVALID_SECRET;
            }
            else
            {
                response.errorCode = ErrorType.SUCCESS;
                response.account = new Account();
            }
            SendData(response);
        }
    }
}
