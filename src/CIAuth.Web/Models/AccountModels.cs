using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace CIAuth.Web.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Token> Tokens { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }

    [Table("Applications")]
    public class Application
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        [AllowHtml]
        public string EncryptionKey { get; set; }
        public string ClientSecret { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public string Hosts { get; set; }
        public bool Offline { get; set; }
        public string GrantType { get; set; }


        [ForeignKey("UserProfile")]
        public int UserId { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual List<Token> Tokens { get; set; }
    }

    [Table("Tokens")]
    public class Token
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TokenId { get; set; }

        [ForeignKey("Application")]
        public int ApplicationId { get; set; }
        public virtual Application Application { get; set; }


        public string CIAPIUserName { get; set; }
        public string CIAPILogonUserName { get; set; }
        public string CIAPISession { get; set; }
        

        public string JsonEncryptedToken { get; set; }
        public DateTime Expires { get; set; }

        /// <summary>
        ///     This is a stop gap measure until  /tradingapi/session/refresh can be implemented
        ///     to exchange a valid session, before it expires, for a new session
        /// </summary>
        public string EncryptedCIAPICredentials { get; set; }
        public string Scope { get; set; }
        public string AccessCode { get; set; }

        /// <summary>
        ///     a 'use by' date is established when access code is issued.
        ///     if access code is not exchanged in time, the token grant is
        ///     dead and will be pruned
        /// </summary>
        public DateTime AccessCodeExpires { get; set; }

        public string RefreshToken { get; set; }

        public DateTime LastAccessed { get; set; }
    }
}
