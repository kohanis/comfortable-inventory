using System;

namespace kohanis.ComfortableInventory
{
    public class TranspilerException : Exception
    {
        public TranspilerException(string message) : base(message)
        {
        }
    }
}