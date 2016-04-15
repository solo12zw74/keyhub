using System;

namespace KeyHub.Model.Definition.Membership
{
    public class PasswordLogin
    {
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public DateTime Created { get; set; }

        public string ConfirmationToken { get; set; }

        public bool Confirmed { get; set; }

        public DateTime? LastLoginFailed { get; set; }

        public int LoginFailureCounter { get; set; }

        public string PasswordHash { get; set; }

        public string PasswordSalt { get; set; }

        public DateTime? PasswordChanged { get; set; }

        public string VerificationToken { get; set; }

        public DateTime? VerificationTokenExpires { get; set; }

    }
}
