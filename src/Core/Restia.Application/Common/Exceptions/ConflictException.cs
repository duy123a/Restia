using System.Net;

namespace Restia.Application.Common.Exceptions;

public class ConflictException : CustomException
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="message">The message</param>
	public ConflictException(string message)
		: base(message, null, HttpStatusCode.Conflict)
	{
	}
}
