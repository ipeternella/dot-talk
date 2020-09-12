using System;

namespace Dottalk.App.Exceptions
{
    //
    // Summary:
    //   Collection of exceptions that can be raised by the app.
    public class ChatRoomAlreadyExistsException : Exception
    {
        public ChatRoomAlreadyExistsException()
        {
        }

        public ChatRoomAlreadyExistsException(string message)
            : base(message)
        {
        }

        public ChatRoomAlreadyExistsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}