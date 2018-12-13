using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Json.Net
{
    public class ParserBase
    {
        TextReader Reader;        
        protected int PeekChar;

        
        public ParserBase(TextReader textReader)
        {
            Reader = textReader;
            PeekChar = Reader.Peek();
        }
        

        protected bool EndOfStream
        {
            get { return PeekChar == -1; }
        }


        protected char NextChar
        {
            get { return EndOfStream ? (char)27 : (char)PeekChar; }
        }


        protected char ReadChar()
        {
            if (!EndOfStream)
            {
                var r = (char)Reader.Read();
                PeekChar = Reader.Peek();

                return r;
            }

            throw new FormatException("Unexpected end of string!");
        }


        protected void KeepNext(ref string s, char? c = null)
        {
            s += c ?? NextChar;
            ReadChar();
        }


        protected void KeepNext(StringBuilder sb, char? c = null)
        {
            sb.Append(c ?? NextChar);
            ReadChar();
        }


        protected void SkipWhite()
        {
            while (" \t\r\n".Contains(NextChar))
                ReadChar();
        }


        protected bool TryMatch(char c)
        {
            SkipWhite();

            if (NextChar == c)
            {
                ReadChar();
                return true;
            }

            return false;
        }

        
        protected void Match(string s)
        {
            SkipWhite();

            foreach (var c in s)
                if (NextChar == c)
                    ReadChar();
                else
                    throw new FormatException(s + " expected!");
        }


        protected bool TryMatch(string s)
        {
            SkipWhite();

            foreach (var c in s)
                if (NextChar == c)
                    ReadChar();
                else
                    return false;

            return true;
        }
    }
}