using System;
using System.Runtime.Serialization;

namespace PortalToUnity
{
    public class PortalToUnityException : Exception
    {
        public PortalToUnityException() {}
        public PortalToUnityException(string message) : base(message) {}
        public PortalToUnityException(string message, Exception inner) : base(message, inner) {}
        protected PortalToUnityException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }

    public class FigureRemovedException : PortalToUnityException
    {
        public FigureRemovedException() {}
        public FigureRemovedException(string message) : base(message) {}
        public FigureRemovedException(string message, Exception inner) : base(message, inner) {}
        protected FigureRemovedException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }

    public class FigureErrorException : PortalToUnityException
    {
        public FigureErrorException() {}
        public FigureErrorException(string message) : base(message) {}
        public FigureErrorException(string message, Exception inner) : base(message, inner) {}
        protected FigureErrorException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }

    public class PortalDisconnectedException : PortalToUnityException
    {
        public PortalDisconnectedException() {}
        public PortalDisconnectedException(string message) : base(message) {}
        public PortalDisconnectedException(string message, Exception inner) : base(message, inner) {}
        protected PortalDisconnectedException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }

    public class PortalIOException : PortalToUnityException
    {
        public PortalIOException() {}
        public PortalIOException(string message) : base(message) {}
        public PortalIOException(string message, Exception inner) : base(message, inner) {}
        protected PortalIOException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}