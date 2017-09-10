namespace System.IO
{
    /// <summary>
    /// A reimplementation of a standard exception type that was introduced in .NET 3.0.
    /// </summary>
    public class FileFormatException : FormatException
    {
        public FileFormatException() : base() { }
        public FileFormatException(string message) : base(message) { }
        public FileFormatException(string message, Exception innerException) : base(message, innerException) { }
    }
}