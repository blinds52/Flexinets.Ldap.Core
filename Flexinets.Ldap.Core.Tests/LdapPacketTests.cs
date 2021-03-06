﻿using NUnit.Framework;
using System;
using System.IO;

namespace Flexinets.Ldap.Core.Tests
{
    public class LdapPacketTests
    {
        [TestCase]
        public void TestLdapAttributeSequenceGetBytesString()
        {
            var packet = new LdapPacket(1);

            var bindrequest = new LdapAttribute(LdapOperation.BindRequest);
            bindrequest.ChildAttributes.Add(new LdapAttribute(UniversalDataType.Integer, (Byte)3));
            bindrequest.ChildAttributes.Add(new LdapAttribute(UniversalDataType.OctetString, "cn=bindUser,cn=Users,dc=dev,dc=company,dc=com"));
            bindrequest.ChildAttributes.Add(new LdapAttribute((byte)0, "bindUserPassword"));

            packet.ChildAttributes.Add(bindrequest);

            var expected = "304c0204000000016044020103042d636e3d62696e64557365722c636e3d55736572732c64633d6465762c64633d636f6d70616e792c64633d636f6d801062696e645573657250617373776f7264"; // "30490201016044020103042d636e3d62696e64557365722c636e3d55736572732c64633d6465762c64633d636f6d70616e792c64633d636f6d801062696e645573657250617373776f7264";
            Assert.AreEqual(expected, Utils.ByteArrayToString(packet.GetBytes()));
        }


        [TestCase]
        public void TestLdapAttributeSequenceGetBytes2()
        {
            var packet = new LdapPacket(1);

            var bindresponse = new LdapAttribute(LdapOperation.BindResponse);

            var resultCode = new LdapAttribute(UniversalDataType.Enumerated, (Byte)LdapResult.success);
            bindresponse.ChildAttributes.Add(resultCode);

            var matchedDn = new LdapAttribute(UniversalDataType.OctetString);
            var diagnosticMessage = new LdapAttribute(UniversalDataType.OctetString);

            bindresponse.ChildAttributes.Add(matchedDn);
            bindresponse.ChildAttributes.Add(diagnosticMessage);

            packet.ChildAttributes.Add(bindresponse);

            var expected = "300f02040000000161070a010004000400"; // "300c02010161070a010004000400";
            Assert.AreEqual(expected, Utils.ByteArrayToString(packet.GetBytes()));
        }


        [TestCase]
        public void TestLdapAttributeSequenceGetBytesShortcut()
        {
            var packet = new LdapPacket(1);
            var bindresponse = new LdapResultAttribute(LdapOperation.BindResponse, LdapResult.success);
            packet.ChildAttributes.Add(bindresponse);

            var expected = "300f02040000000161070a010004000400"; // "300c02010161070a010004000400";
            Assert.AreEqual(expected, Utils.ByteArrayToString(packet.GetBytes()));
        }


        [TestCase]
        public void TestLdapAttributeParse()
        {
            var expected = "30490201016044020103042d636e3d62696e64557365722c636e3d55736572732c64633d6465762c64633d636f6d70616e792c64633d636f6d801062696e645573657250617373776f7264";
            var packetBytes = Utils.StringToByteArray(expected);
            var packet = LdapPacket.ParsePacket(packetBytes);
            Assert.AreEqual(expected, Utils.ByteArrayToString(packet.GetBytes()));
        }
       

        [TestCase]
        public void TestLdapAttributeParse3()
        {
            var expected = "30620201026340041164633d636f6d70616e792c64633d636f6d0a01020a010302010202010b010100a31a040e73414d4163636f756e744e616d65040876666f7274656c693000a01b30190417322e31362e3834302e312e3131333733302e332e342e32";
            var packetBytes = Utils.StringToByteArray(expected);
            var packet = LdapPacket.ParsePacket(packetBytes);
            Assert.AreEqual(expected, Utils.ByteArrayToString(packet.GetBytes()));
        }


        [TestCase]
        public void TestLdapAttributeParse4()
        {
            var bytes = "30620201026340041164633d636f6d70616e792c64633d636f6d0a01020a010302010202010b010100a31a040e73414d4163636f756e744e616d65040876666f7274656c693000a01b30190417322e31362e3834302e312e3131333733302e332e342e3200000000";
            var expected = "30620201026340041164633d636f6d70616e792c64633d636f6d0a01020a010302010202010b010100a31a040e73414d4163636f756e744e616d65040876666f7274656c693000a01b30190417322e31362e3834302e312e3131333733302e332e342e32";
            var packetBytes = Utils.StringToByteArray(bytes);
            var packet = LdapPacket.ParsePacket(packetBytes);
            Assert.AreEqual(expected, Utils.ByteArrayToString(packet.GetBytes()));
        }


        [TestCase]
        public void TestLdapAttributeParseFromStream4()
        {
            var bytes = "30620201026340041164633d636f6d70616e792c64633d636f6d0a01020a010302010202010b010100a31a040e73414d4163636f756e744e616d65040876666f7274656c693000a01b30190417322e31362e3834302e312e3131333733302e332e342e3200000000";
            var expected = "30620201026340041164633d636f6d70616e792c64633d636f6d0a01020a010302010202010b010100a31a040e73414d4163636f756e744e616d65040876666f7274656c693000a01b30190417322e31362e3834302e312e3131333733302e332e342e32";
            var packetBytes = Utils.StringToByteArray(bytes);
            var stream = new MemoryStream(packetBytes);
            LdapPacket.TryParsePacket(stream, out var packet);
            Assert.AreEqual(expected, Utils.ByteArrayToString(packet.GetBytes()));
        }


        [TestCase]
        public void TestPacketMessageId()
        {
            var packet = new LdapPacket(Int32.MaxValue);
            Assert.AreEqual(Int32.MaxValue, packet.MessageId);
        }


        [TestCase]
        public void TestPacketParsingBindRequest()
        {
            var bytes = "30490201016044020103042d636e3d62696e64557365722c636e3d55736572732c64633d6465762c64633d636f6d70616e792c64633d636f6d801062696e645573657250617373776f7264";
            var expected = "30490201016044020103042d636e3d62696e64557365722c636e3d55736572732c64633d6465762c64633d636f6d70616e792c64633d636f6d801062696e645573657250617373776f7264";
            var packetBytes = Utils.StringToByteArray(bytes);
            var stream = new MemoryStream(packetBytes);
            LdapPacket.TryParsePacket(stream, out var packet);
            RecurseAttributes(packet);
            Assert.AreEqual(expected, Utils.ByteArrayToString(packet.GetBytes()));
        }
        private void RecurseAttributes(LdapAttribute attribute, Int32 depth = 1)
        {
            if (attribute != null)
            {
                Console.WriteLine($"{Utils.Repeat(">", depth)} {attribute.Class}:{attribute.DataType}{attribute.LdapOperation}{attribute.ContextType} - Type: {attribute.GetValue().GetType()} - {attribute.GetValue()}");

                if (attribute.IsConstructed)
                {
                    foreach (var attr in attribute.ChildAttributes)
                    {
                        depth++;
                        RecurseAttributes(attr, depth);
                        depth--;
                    }
                }
            }
        }

    }
}
