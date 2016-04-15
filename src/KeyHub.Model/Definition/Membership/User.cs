using KeyHub.Model.Definition.Membership;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyHub.Model
{
    /// <summary>
    /// Defines the user for this application
    /// </summary>
    public partial class User : IUser<int>
    {
        public User()
        {
            ExternalLogins = new Collection<ExternalLogin>();
            Rights = new List<UserObjectRight>();
            Roles = Roles = new Collection<Role>();
        }

        /// <summary>
        /// Unique user ID
        /// </summary>        
        public int Id { get; set; }

        /// <summary>
        /// E-mail address for this user (is simply 'admin' for the default admin)
        /// </summary>
        public string Email { get; set; }

        //[ForeignKey("UserId")]
        //public virtual Membership Membership { get; set; }
        public virtual PasswordLogin PasswordLogin { get; set; }

        public virtual ICollection<ExternalLogin> ExternalLogins { get; private set; }

        public virtual ICollection<Role> Roles { get; private set; }

        public virtual ICollection<UserObjectRight> Rights { get; set; }

        public string UserName
        {
            get
            {
                return Email;
            }

            set
            {
                Email = value;
            }
        }
    }
}