namespace KeyHub.Model.Definition.Membership
{
    public class ExternalLogin
    {
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }
}
