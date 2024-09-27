using System.Net;

namespace Restia.Application.Common.Exceptions;

public class CustomException : Exception
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="message">The message</param>
	/// <param name="errors">The errors</param>
	/// <param name="statusCode">The status code</param>
	public CustomException(
		string message,
		List<string>? errors = default,
		HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
		: base(message)
	{
		ErrorMessages = errors;
		StatusCode = statusCode;
	}

	/// <summary>Get error messages</summary>
	public List<string>? ErrorMessages { get; }
	/// <summary>Get status code</summary>
	public HttpStatusCode StatusCode { get; }
}
