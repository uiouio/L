﻿/**
The MIT License (MIT)

Copyright(C) 2013 <Hooke HU>

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
**/

using System;
using System.IO;
using System.Text;

namespace HMF
{
    internal class Util
    {
        public static void WriteVarint(int v, Stream stream)
        {
            var a = v & 0x7f;
            if (v >> 7 == 0)
            {
                stream.WriteByte((byte) (a & 0x7f));
                return;
            }
            stream.WriteByte((byte) (a | 0x80));
            var b = v << 18;
            b = b >> 25;
            if (v >> 14 == 0)
            {
                stream.WriteByte((byte) (b & 0x7f));
                return;
            }
            stream.WriteByte((byte) (b | 0x80));
            var c = v << 11;
            c = c >> 25;
            if (v >> 21 == 0)
            {
                stream.WriteByte((byte) (c & 0x7f));
                return;
            }
            stream.WriteByte((byte) (c | 0x80));
            var d = v << 4;
            d = d >> 25;
            if (v >> 28 == 0)
            {
                stream.WriteByte((byte) (d & 0x7f));
                return;
            }
            stream.WriteByte((byte) (d | 0x80));
            var e = v >> 28 & 0x0f;
            stream.WriteByte((byte) e);
        }

        public static int ReadVarint(Stream stream)
        {
            var rst = 0;
            var v = stream.ReadByte();
            var b = v & 0x000000ff;
            rst = b;
            if ((rst & 0x00000080) != 0x00000080)
            {
                return rst;
            }
            v = stream.ReadByte();
            b = v & 0x000000ff;
            rst = (rst & 0x0000007f) | (b << 7);
            if ((rst & 0x00004000) != 0x00004000)
            {
                return rst;
            }
            v = stream.ReadByte();
            b = v & 0x000000ff;
            rst = (rst & 0x00003fff) | (b << 14);
            if ((rst & 0x00200000) != 0x00200000)
            {
                return rst;
            }
            v = stream.ReadByte();
            b = v & 0x000000ff;
            rst = (rst & 0x001fffff) | (b << 21);
            if ((rst & 0x10000000) != 0x10000000)
            {
                return rst;
            }
            v = stream.ReadByte();
            b = v & 0x000000ff;
            rst = (rst & 0x0fffffff) | (b << 28);
            return rst;
        }

        public static void WriteDobule(double v, Stream stream)
        {
            var s = BitConverter.GetBytes(v);
            for (var i = 0; i < 8; i++)
            {
                stream.WriteByte(s[i]);
            }
        }

        public static double ReadDouble(Stream stream)
        {
            byte[] s = {0, 0, 0, 0, 0, 0, 0, 0};
            for (var i = 0; i < 8; i++)
            {
                s[i] = (byte) stream.ReadByte();
            }
            return BitConverter.ToDouble(s, 0);
        }

        public static void WriteStr(byte[] s, Stream stream)
        {
            //char[] s = v.ToCharArray();
            //byte[] s = System.Text.Encoding.UTF8.GetBytes (v);
            var len = s.Length;
            for (var i = 0; i < len; i++)
            {
                stream.WriteByte(s[i]);
            }
        }

        public static string ReadStr(int len, Stream stream)
        {
            var s = new byte[len];
            stream.Read(s, 0, len);
            return Encoding.UTF8.GetString(s);
        }

        public static uint ToZigzag32(int v)
        {
            return (uint) ((v << 1) ^ (v >> 31));
        }

        public static int FromZigzag32(uint v)
        {
            return (int) ((v >> 1) ^ (v << 31));
        }

        public static ulong ToZigzag64(long v)
        {
            return (ulong) ((v << 1) ^ (v >> 63));
        }

        public static long FromZigzag64(ulong v)
        {
            return (long) ((v >> 1) ^ (v << 63));
        }
    }
}