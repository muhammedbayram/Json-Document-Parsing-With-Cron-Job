namespace sovosTask.Interfaces
{
    public interface IEmailService
    {
        public void SendEmail(string recipient, string subject, string body);
    }
}
