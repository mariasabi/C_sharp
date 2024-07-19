namespace OrderService.Exceptions
{
    public class ArgumentsException : Exception
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
