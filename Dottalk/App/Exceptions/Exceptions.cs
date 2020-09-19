using System;

namespace Dottalk.App.Exceptions
{
    //
    // Summary:
    //   Collection of exceptions that can be raised by the app.
    public class ObjectAlreadyExistsException : Exception
    {
        public ObjectAlreadyExistsException()
        {
        }

        public ObjectAlreadyExistsException(string message)
            : base(message)
        {
        }

        public ObjectAlreadyExistsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class ObjectDoesNotExistException : Exception
    {
        public ObjectDoesNotExistException()
        {
        }

        public ObjectDoesNotExistException(string message)
            : base(message)
        {
        }

        public ObjectDoesNotExistException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class ChatRoomIsFullException : Exception
    {
        public ChatRoomIsFullException()
        {
        }

        public ChatRoomIsFullException(string message)
            : base(message)
        {
        }

        public ChatRoomIsFullException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class UserIsAlreadyConnectedException : Exception
    {
        public UserIsAlreadyConnectedException()
        {
        }

        public UserIsAlreadyConnectedException(string message)
            : base(message)
        {
        }

        public UserIsAlreadyConnectedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}