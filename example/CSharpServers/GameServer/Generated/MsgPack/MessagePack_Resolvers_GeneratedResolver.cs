// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591 // document public APIs

#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1649 // File name should match first type name

namespace MessagePack.Resolvers
{
    public class GeneratedResolver : global::MessagePack.IFormatterResolver
    {
        public static readonly global::MessagePack.IFormatterResolver Instance = new GeneratedResolver();

        private GeneratedResolver()
        {
        }

        public global::MessagePack.Formatters.IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            internal static readonly global::MessagePack.Formatters.IMessagePackFormatter<T> Formatter;

            static FormatterCache()
            {
                var f = GeneratedResolverGetFormatterHelper.GetFormatter(typeof(T));
                if (f != null)
                {
                    Formatter = (global::MessagePack.Formatters.IMessagePackFormatter<T>)f;
                }
            }
        }
    }

    internal static class GeneratedResolverGetFormatterHelper
    {
        private static readonly global::System.Collections.Generic.Dictionary<global::System.Type, int> lookup;

        static GeneratedResolverGetFormatterHelper()
        {
            lookup = new global::System.Collections.Generic.Dictionary<global::System.Type, int>(30)
            {
                { typeof(global::Devarc.ErrorType), 0 },
                { typeof(global::Auth2C.NotifyError), 1 },
                { typeof(global::Auth2C.NotifyLogin), 2 },
                { typeof(global::Auth2C.NotifyLogout), 3 },
                { typeof(global::Auth2C.NotifySession), 4 },
                { typeof(global::Auth2C.NotifySignin), 5 },
                { typeof(global::C2Auth.RequestLogin), 6 },
                { typeof(global::C2Auth.RequestLogout), 7 },
                { typeof(global::C2Auth.RequestSession), 8 },
                { typeof(global::C2Auth.RequestSignin), 9 },
                { typeof(global::C2Game.RequestLogin), 10 },
                { typeof(global::Devarc.AFFECT), 11 },
                { typeof(global::Devarc.BLOCK), 12 },
                { typeof(global::Devarc.CHARACTER), 13 },
                { typeof(global::Devarc.CommonResult), 14 },
                { typeof(global::Devarc.CustomSigninResult), 15 },
                { typeof(global::Devarc.GoogleCodeResult), 16 },
                { typeof(global::Devarc.GoogleRefreshResult), 17 },
                { typeof(global::Devarc.GoogleSigninResult), 18 },
                { typeof(global::Devarc.ITEM_EQUIP), 19 },
                { typeof(global::Devarc.ITEM_MATERIAL), 20 },
                { typeof(global::Devarc.ITEM_RELIC), 21 },
                { typeof(global::Devarc.PROJECTILE), 22 },
                { typeof(global::Devarc.SKILL), 23 },
                { typeof(global::Devarc.SOUND), 24 },
                { typeof(global::Devarc.UNIT_HERO), 25 },
                { typeof(global::Devarc.UNIT_LEVEL), 26 },
                { typeof(global::Devarc.UNIT_MONSTER), 27 },
                { typeof(global::Devarc.VECTOR3), 28 },
                { typeof(global::Game2C.NotifyLogin), 29 },
            };
        }

        internal static object GetFormatter(global::System.Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key))
            {
                return null;
            }

            switch (key)
            {
                case 0: return new MessagePack.Formatters.Devarc.ErrorTypeFormatter();
                case 1: return new MessagePack.Formatters.Auth2C.NotifyErrorFormatter();
                case 2: return new MessagePack.Formatters.Auth2C.NotifyLoginFormatter();
                case 3: return new MessagePack.Formatters.Auth2C.NotifyLogoutFormatter();
                case 4: return new MessagePack.Formatters.Auth2C.NotifySessionFormatter();
                case 5: return new MessagePack.Formatters.Auth2C.NotifySigninFormatter();
                case 6: return new MessagePack.Formatters.C2Auth.RequestLoginFormatter();
                case 7: return new MessagePack.Formatters.C2Auth.RequestLogoutFormatter();
                case 8: return new MessagePack.Formatters.C2Auth.RequestSessionFormatter();
                case 9: return new MessagePack.Formatters.C2Auth.RequestSigninFormatter();
                case 10: return new MessagePack.Formatters.C2Game.RequestLoginFormatter();
                case 11: return new MessagePack.Formatters.Devarc.AFFECTFormatter();
                case 12: return new MessagePack.Formatters.Devarc.BLOCKFormatter();
                case 13: return new MessagePack.Formatters.Devarc.CHARACTERFormatter();
                case 14: return new MessagePack.Formatters.Devarc.CommonResultFormatter();
                case 15: return new MessagePack.Formatters.Devarc.CustomSigninResultFormatter();
                case 16: return new MessagePack.Formatters.Devarc.GoogleCodeResultFormatter();
                case 17: return new MessagePack.Formatters.Devarc.GoogleRefreshResultFormatter();
                case 18: return new MessagePack.Formatters.Devarc.GoogleSigninResultFormatter();
                case 19: return new MessagePack.Formatters.Devarc.ITEM_EQUIPFormatter();
                case 20: return new MessagePack.Formatters.Devarc.ITEM_MATERIALFormatter();
                case 21: return new MessagePack.Formatters.Devarc.ITEM_RELICFormatter();
                case 22: return new MessagePack.Formatters.Devarc.PROJECTILEFormatter();
                case 23: return new MessagePack.Formatters.Devarc.SKILLFormatter();
                case 24: return new MessagePack.Formatters.Devarc.SOUNDFormatter();
                case 25: return new MessagePack.Formatters.Devarc.UNIT_HEROFormatter();
                case 26: return new MessagePack.Formatters.Devarc.UNIT_LEVELFormatter();
                case 27: return new MessagePack.Formatters.Devarc.UNIT_MONSTERFormatter();
                case 28: return new MessagePack.Formatters.Devarc.VECTOR3Formatter();
                case 29: return new MessagePack.Formatters.Game2C.NotifyLoginFormatter();
                default: return null;
            }
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1649 // File name should match first type name
