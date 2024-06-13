namespace Order_management.Exceptions
{
    public class ArgumentsException: Exception
    {
        public ArgumentsException(string message) : base(message)
        {
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
