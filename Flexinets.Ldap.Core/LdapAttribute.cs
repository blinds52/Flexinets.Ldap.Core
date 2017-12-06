﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flexinets.Ldap.Core
{
    public class LdapAttribute
    {
        private Tag _tag;
        protected Byte[] Value = new Byte[0];
        public List<LdapAttribute> ChildAttributes = new List<LdapAttribute>();

        public TagClass Class => _tag.Class;
        public Boolean IsConstructed => _tag.IsConstructed;
        public LdapOperation? LdapOperation => _tag.LdapOperation;
        public UniversalDataType? DataType => _tag.DataType;
        public Byte? ContextType => _tag.ContextType;


        /// <summary>
        /// Create an application attribute
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="isConstructed"></param>
        public LdapAttribute(LdapOperation operation)
        {
            _tag = new Tag(operation);
        }


        /// <summary>
        /// Create an application attribute
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="isConstructed"></param>
        /// <param name="value"></param>
        public LdapAttribute(LdapOperation operation, Object value)
        {
            _tag = new Tag(operation);
            Value = GetBytes(value);
        }


        /// <summary>
        /// Create a universal attribute
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="isConstructed"></param>
        public LdapAttribute(UniversalDataType dataType)
        {
            _tag = new Tag(dataType);
        }


        /// <summary>
        /// Create a universal attribute
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="isConstructed"></param>
        /// <param name="value"></param>
        public LdapAttribute(UniversalDataType dataType, Object value)
        {
            _tag = new Tag(dataType);
            Value = GetBytes(value);
        }


        /// <summary>
        /// Create a context attribute
        /// </summary>
        /// <param name="contextType"></param>
        /// <param name="isConstructed"></param>
        public LdapAttribute(Byte contextType)
        {
            _tag = new Tag(contextType);
        }


        /// <summary>
        /// Create a context attribute
        /// </summary>
        /// <param name="contextType"></param>
        /// <param name="isConstructed"></param>
        /// <param name="value"></param>
        public LdapAttribute(Byte contextType, Object value)
        {
            _tag = new Tag(contextType);
            Value = GetBytes(value);
        }


        /// <summary>
        /// Create an attribute with tag
        /// </summary>
        /// <param name="tag"></param>
        protected LdapAttribute(Tag tag)
        {
            _tag = tag;
        }


        /// <summary>
        /// Get the byte representation of the attribute and its children
        /// </summary>
        /// <returns></returns>
        public Byte[] GetBytes()
        {
            var contentBytes = new List<Byte>();
            if (ChildAttributes.Any())
            {
                _tag.IsConstructed = true;
                ChildAttributes.ForEach(o => contentBytes.AddRange(o.GetBytes()));
            }
            else
            {
                contentBytes.AddRange(Value);
            }

            return new List<Byte> { _tag.TagByte }.Concat(Utils.IntToBerLength(contentBytes.Count)).Concat(contentBytes).ToArray();
        }


        /// <summary>
        /// Get a typed value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetValue<T>()
        {
            return (T)Convert.ChangeType(GetValue(), typeof(T));
        }


        /// <summary>
        /// Get an object value
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            if (_tag.Class == TagClass.Universal)
            {
                switch (_tag.DataType)
                {
                    case UniversalDataType.Boolean:
                        return BitConverter.ToBoolean(Value, 0);

                    case UniversalDataType.Integer:
                        var intbytes = new Byte[4];
                        Buffer.BlockCopy(Value, 0, intbytes, 4 - Value.Length, Value.Length);
                        Array.Reverse(intbytes);
                        return BitConverter.ToInt32(intbytes, 0);

                    default:
                        return Encoding.UTF8.GetString(Value, 0, Value.Length);
                }
            }

            // todo add rest if needed
            return Encoding.UTF8.GetString(Value, 0, Value.Length);
        }


        /// <summary>
        /// Convert the value to its byte form
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Byte[] GetBytes(Object value)
        {
            switch (value)
            {
                case String _value:
                    return Encoding.UTF8.GetBytes(_value);

                case Int32 _value:
                    return BitConverter.GetBytes(_value).Reverse().ToArray();

                case Boolean _value:
                    return BitConverter.GetBytes(_value);

                case Byte _value:
                    return new Byte[] { _value };

                case Byte[] _value:
                    return _value;

                default:
                    throw new InvalidOperationException($"Nothing found for {value.GetType()}");
            }
        }
    }
}
