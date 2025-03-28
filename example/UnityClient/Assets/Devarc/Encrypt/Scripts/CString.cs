﻿using System;
using UnityEngine;
using MessagePack;

namespace Devarc
{
    [Serializable]
    public class CString
    {
        public static implicit operator string(CString obj)
        {
            return obj.ToString();
        }

        public static implicit operator CString(string value)
        {
            CString obj = new CString(value);
            return obj;
        }

        [SerializeField]
        [Key(0)]
        public string data;

        public string Value
        {
            get { return ToString(); }
            set { data = EncryptUtil.Encrypt_Base64(value); }
        }

        public override string ToString()
        {
            try
            {
                return EncryptUtil.Decrypt_Base64(data);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
                return string.Empty;
            }
        }

        public CString()
        {
            data = string.Empty;
        }

        public CString(string value)
        {
            data = EncryptUtil.Encrypt_Base64(value);
        }
    }

}
