namespace BetterTTD.Actors.ClientGroup.ReceiverGroup
{
    public class ReceiveBufMessage { }
    
    public class ReceiveSocketErrorMessage
    {
        public string Error { get; }

        public ReceiveSocketErrorMessage(string error)
        {
            Error = error;
        }
    }
}