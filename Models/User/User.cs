namespace PodNoms.Api.Models
{
    public class User : BaseModel
    {
        public int Id { get; set; }
        public string EmailAddress { get; set; }
        public string FullName { get; set; }

        public string ProfileImage { get; set; }
        public string Sid { get; set; }
        public string ProviderId { get; set; }
        public string ApiKey { get; set; }
    }
}