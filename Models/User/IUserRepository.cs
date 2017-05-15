namespace PodNoms.Api.Models
{
    public interface IUserRepository
    {
        User Get(int id);
        User Get(string email);
        User UpdateRegistration(string email, string name, string sid, string providerId, string profileImage);
        string UpdateApiKey(string email);

        User AddOrUpdate(User merged);
    }
}