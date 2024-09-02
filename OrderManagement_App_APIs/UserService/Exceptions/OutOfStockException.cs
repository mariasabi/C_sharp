namespace UserService.Exceptions
{
    public class OutOfStockException:Exception
    {
        public OutOfStockException(string message) : base(message)
        {
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
