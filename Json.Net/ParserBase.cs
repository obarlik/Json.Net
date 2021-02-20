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

        char[] ReaderBuffer;
        
        string Buffer;
        int BufferIndex;

        protected char NextChar;

        const char EOF = (char)27;

        protected bool EndOfStream;

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


        public ParserBase(int bufferSize = 1024)
        {
            ReaderBuffer = new char[bufferSize];
        }


        ParserBase _Initialize()
        {
            EndOfStream = false;
            BufferIndex = -1;
            _Stream = _ReadNext().GetEnumerator();
            ReadNext();
            return this;
        }


        public ParserBase Initialize(string json)
        {
            Reader = null;
            Buffer = json;
            return _Initialize();
        }


        public ParserBase Initialize(TextReader textReader)
        {
            Reader = textReader;
            Buffer = "";
            return _Initialize();
        }


        IEnumerator<char> _Stream;

        protected char ReadNext()
        {
            _Stream.MoveNext();
            return NextChar = _Stream.Current;
        }


        IEnumerable<char> _ReadNext()
        {
            if (EndOfStream)
                yield break;

            if (Reader == null)
            {
                while (++BufferIndex < Buffer.Length)
                    yield return Buffer[BufferIndex];
            }
            else 
            {
                while (true)
                {
                    var read = Reader.ReadBlock(ReaderBuffer, 0, ReaderBuffer.Length);

                    Buffer = new string(ReaderBuffer, 0, read);
                    BufferIndex = -1;

                    while (read-- > 0)
                        yield return Buffer[++BufferIndex];
                }
            }

            Buffer = "";
            BufferIndex = 0;
            EndOfStream = true;
            yield return EOF;
        }


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
                    throw new FormatException(s + " expected, but " + NextChar + " found!");
        }
    }
}