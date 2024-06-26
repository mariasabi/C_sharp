namespace Order_management.Exceptions
{
    public class IdNotFoundException : Exception
    {
        public IdNotFoundException(string message) : base(message)
        {

        }
        public override string ToString()
        {
            return Message;
        }
    }
}

