using System.Net;

namespace Restia.Application.Common.Exceptions;

public class NotFoundException : CustomException
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="message">The message</param>
	public NotFoundException(string message)
		: base(message, null, HttpStatusCode.NotFound)
	{
	}
}
