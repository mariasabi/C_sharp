namespace OrderService.Exceptions
{
    public class CSVException : Exception
    {
        public CSVException(string message) : base(message)
        {
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
