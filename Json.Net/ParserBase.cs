using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Json.Net
{
    public class ParserBase
    {
        StreamReader TextStream;

        protected string Text;
        protected char NextChar;


        public ParserBase(string text)
        {
            TextStream =
                new StreamReader(
                  new MemoryStream(
                    Encoding.UTF8.GetBytes(text ?? "")));

            Text = "";
            NextChar = (char)TextStream.Peek();
        }


        protected char ReadChar()
        {
            if (!TextStream.EndOfStream)
                return (char)TextStream.Read();

            throw new FormatException("Unexpected end of string!");
        }


        protected void MoveChar()
        {
            ReadChar();

            if (!TextStream.EndOfStream)
                NextChar = (char)TextStream.Peek();
            else
                NextChar = char.MinValue;
        }


        protected void SkipWhite()
        {
            while (" \t\r\n".Contains(NextChar))
                MoveChar();
        }


        protected void AppendChar()
        {
            Text += NextChar;
            MoveChar();
        }


        protected bool TryMatch(char c)
        {
            SkipWhite();

            if (NextChar == c)
            {
                MoveChar();
                return true;
            }

            return false;
        }


        protected void Match(char c)
        {
            SkipWhite();

            if (NextChar == c)
                MoveChar();
            else
                throw new FormatException(c + " expected!");
        }
    }
}