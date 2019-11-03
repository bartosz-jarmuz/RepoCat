using System;

namespace RepoCat.ProjectFileReaders
{
    public class ProjectParserException : Exception
    {
        public ProjectParserException(string message) : base(message)
        {
        }

        public ProjectParserException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public string ProjectPath { get; private set; }
        public Type ParserType { get; }

        public ProjectParserException()
        {
        }

        public ProjectParserException(string message, string path, Type parserType) : base(message)
        {
            this.ProjectPath = path;
            this.ParserType = parserType;
        }

        public ProjectParserException(string message, string path, Type parserType, Exception innerException) : base(message, innerException)
        {
            this.ProjectPath = path;
            this.ParserType = parserType;

        }

    }
}