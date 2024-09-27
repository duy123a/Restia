using System.Net;

namespace Restia.Application.Common.Exceptions;

public class UnauthorizedException : CustomException
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="message">The message</param>
	public UnauthorizedException(string message)
		: base(message, null, HttpStatusCode.Unauthorized)
	{
	}
}
