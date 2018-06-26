using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Report2016.Helpers
{

    public class TheUser{
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserId { get; set; }

    }

    public static class ClaimsHelper
    {
        public static TheUser GetUser(this IIdentity identity)
        {
            var claimsIdentity = (ClaimsIdentity)identity;
            var claims = claimsIdentity.Claims;

            var claimsList = claims as IList<Claim> ?? claims.ToList();

            var firstName = claimsList.Where(c => c.Type == ClaimTypes.GivenName)
                   .Select(c => c.Value).SingleOrDefault();

            var lastName = claimsList.Where(c => c.Type == ClaimTypes.Surname)
                   .Select(c => c.Value).SingleOrDefault();

            var email = claimsList.Where(c => c.Type == ClaimTypes.Email)
                   .Select(c => c.Value).SingleOrDefault();

			var userId = claimsList.Where(c => c.Type == ClaimTypes.NameIdentifier)
				   .Select(c => c.Value).SingleOrDefault();
            
            var user = new TheUser
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                UserId = userId
            };

            return user;
        }


		public static string GetUserEmail(this IIdentity identity)
		{
			var claimsIdentity = (ClaimsIdentity)identity;
			var claims = claimsIdentity.Claims;

			var claimsList = claims as IList<Claim> ?? claims.ToList();

            var result = claimsList.Where(c => c.Type == ClaimTypes.Email)
				   .Select(c => c.Value).SingleOrDefault();
            
            return result;

		}
    }
}
