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
        
        public ParserBase(TextReader textReader)
        {
            Reader = textReader;
            ReadChar();
        }
        

        protected char NextChar;

        const char EOF = (char)27;

        protected bool EndOfStream { get { return NextChar == EOF; } }

        protected void ReadChar()
        {
            if (NextChar != EOF)
            {
                var r = Reader.Read();
                NextChar = r == -1 ? EOF : (char)r;
            }
        }


        protected void KeepNext(ref string s)
        {
            s += NextChar;
            ReadChar();
        }


        protected void KeepChar(ref string s, char c)
        {
            s += c;
            ReadChar();
        }


        protected void KeepNext(StringBuilder sb)
        {
            sb.Append(NextChar);
            ReadChar();
        }


        protected void KeepChar(StringBuilder sb, char c)
        {
            sb.Append(c);
            ReadChar();
        }


        protected static HashSet<char> WhiteSpace = new HashSet<char>(" \t\r\n");
        protected static HashSet<char> Digits = new HashSet<char>("0123456789");
        protected static HashSet<char> HexDigits = new HashSet<char>("0123456789ABCDEFabcdef");

        protected static HashSet<char> Alpha = new HashSet<char>(
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
            "abcdefghijklmnopqrstuvwxyz");

        protected static HashSet<char> AlphaNumeric = new HashSet<char>(
                    "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                    "abcdefghijklmnopqrstuvwxyz" + 
                    "0123456789");


        protected bool IsWhite
        {
            get { return WhiteSpace.Contains(NextChar); }
        }


        protected bool IsAlpha
        {
            get { return Alpha.Contains(NextChar); }
        }


        protected bool IsAlphaNumeric
        {
            get { return AlphaNumeric.Contains(NextChar); }
        }


        protected bool IsDigit
        {
            get { return Digits.Contains(NextChar); }
        }


        protected bool IsHexDigit
        {
            get { return HexDigits.Contains(NextChar); }
        }


        protected void SkipWhite()
        {
            while (IsWhite)
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