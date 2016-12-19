namespace PodNoms.Api.Utils.Pusher
{
    public class PusherMessage
    {
        public string name { get; set; }
        public string channel { get; set; }
        public PusherPayload data { get; set; }
    }

    public class PusherPayload{
        public string message { get; set; }
    }
}