using System;
namespace AuthorizationService.API.Requests
{
	public class LogoutRequest
	{
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}

