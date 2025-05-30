using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain
{
    public sealed class AuthenticateUserResponse
    {
        /// <summary>
        /// Gets or sets the JWT token for authenticated user
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's full name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's role in the system
        /// </summary>
        public string Role { get; set; } = string.Empty;
    }
}
