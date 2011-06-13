using System;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Enum that describes the type of an error.
	/// </summary>
	public enum ErrorType
	{
		Unknown,
		Error,
		Warning
	}
	
	/// <summary>
	/// Descibes an error during parsing.
	/// </summary>
	public class Error
	{
		/// <summary>
		/// The type of the error.
		/// </summary>
		public ErrorType ErrorType { get; private set; }
		
		/// <summary>
		/// The error description.
		/// </summary>
		public string Message { get; private set; }
		
		/// <summary>
		/// The region of the error.
		/// </summary>
		public DomRegion Region { get; private set; }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ICSharpCode.NRefactory.TypeSystem.Error"/> class.
		/// </summary>
		/// <param name='errorType'>
		/// The error type.
		/// </param>
		/// <param name='message'>
		/// The description of the error.
		/// </param>
		/// <param name='region'>
		/// The region of the error.
		/// </param>
		public Error (ErrorType errorType, string message, DomRegion region)
		{
			this.ErrorType = errorType;
			this.Message = message;
			this.Region = region;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ICSharpCode.NRefactory.TypeSystem.Error"/> class.
		/// </summary>
		/// <param name='errorType'>
		/// The error type.
		/// </param>
		/// <param name='message'>
		/// The description of the error.
		/// </param>
		public Error (ErrorType errorType, string message)
		{
			this.ErrorType = errorType;
			this.Message = message;
			this.Region = DomRegion.Empty;
		}
	}
}
