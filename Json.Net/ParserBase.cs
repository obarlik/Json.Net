using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Json.Net
{
    public class ParserBase
    {
        TextReader Reader;    
        
        public ParserBase()
        {
        }


        public virtual ParserBase Initialize(TextReader textReader)
        {
            Reader = textReader;
            EndOfStream = false;
            BufferIndex = -1;
            BufferSize = 0;
            ReadNext();
            return this;
        }


        protected char NextChar;

        const char EOF = (char)27;

        protected bool EndOfStream;

        char[] Buffer = new char[1024];
        int BufferIndex;
        int BufferSize;

        protected void ReadNext()
        {
            if (EndOfStream)
                return;

            if (++BufferIndex >= BufferSize)
            {
                BufferSize = Reader.ReadBlock(Buffer, 0, Buffer.Length);
                BufferIndex = 0;

                if (BufferSize == 0)
                {
                    EndOfStream = true;
                    NextChar = EOF;
                    return;
                }
            }

            NextChar = Buffer[BufferIndex];
        }


        protected static string WhiteSpace = " \t\r\n";
        protected static string Digits = "0123456789";
        protected static string HexDigits = "0123456789ABCDEFabcdef";

        protected static string Alpha = 
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
            "abcdefghijklmnopqrstuvwxyz";

        protected static string AlphaNumeric = 
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
            "abcdefghijklmnopqrstuvwxyz" + 
            "0123456789";


        protected bool IsWhite
        {
            get { return WhiteSpace.IndexOf(NextChar) >= 0; }
        }


        protected bool IsAlpha
        {
            get { return Alpha.IndexOf(NextChar) >= 0; }
        }


        protected bool IsAlphaNumeric
        {
            get { return AlphaNumeric.IndexOf(NextChar) >= 0; }
        }


        protected bool IsDigit
        {
            get { return Digits.IndexOf(NextChar) >= 0; }
        }


        protected bool IsHexDigit
        {
            get { return HexDigits.IndexOf(NextChar) >= 0; }
        }


        protected void SkipWhite()
        {
            while (IsWhite)
                ReadNext();
        }
        
        
        protected void Match(string s)
        {
            foreach (var c in s)
                if (NextChar == c)
                    ReadNext();
                else
                    throw new FormatException(s + " expected!");
        }
    }
}