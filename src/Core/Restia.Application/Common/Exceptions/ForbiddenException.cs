using System.Net;

namespace Restia.Application.Common.Exceptions;

public class ForbiddenException : CustomException
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="message">The message</param>
	public ForbiddenException(string message)
		: base(message, null, HttpStatusCode.Forbidden)
	{
	}
}
