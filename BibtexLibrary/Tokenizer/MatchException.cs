using System;

namespace BibtexLibrary.Tokenizer
{
    public class MatchException : Exception
    {
        private readonly string _character;
        private readonly int _position;

        public MatchException(string character, int position)
        {
            _character = character;
            _position = position;

        }

        public override string Message
        {
            get { return "Could not match character: " + _character + " at position " + _position; }
        }
    }
}