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
            //UserInRoles = new List<UserInRole>();
            Rights = new List<UserObjectRight>();
            ExternalLogins = new Collection<ExternalLogin>();
            Roles = new Collection<Role>();
        }

        /// <summary>
        /// Unique user ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// The field used by ASP.NET Membership system to identify users (a guid value).
        /// </summary>
        //[Required]
        //[StringLength(40)]
        //public string MembershipUserIdentifier { get; set; }

        public virtual PasswordLogin PasswordLogin { get; set; }

        /// <summary>
        /// E-mail address for this user (is simply 'admin' for the default admin)
        /// </summary>
        [StringLength(256)]
        public string Email { get; set; }

        [ForeignKey("UserId")]
        public virtual Membership Membership { get; set; }

        public virtual ICollection<Role> Roles { get; private set; }

        public virtual ICollection<ExternalLogin> ExternalLogins { get; private set; }

        //public virtual ICollection<UserInRole> UserInRoles { get; set; }

        public virtual ICollection<UserObjectRight> Rights { get; set; }
        
        string IUser<int>.UserName
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