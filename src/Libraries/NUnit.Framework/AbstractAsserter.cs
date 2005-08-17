using System;

namespace NUnit.Framework
{
	/// <summary>
	/// AbstractAsserter is the base class for all asserters.
	/// Asserters encapsulate a condition test and generation 
	/// of an AssertionException with a tailored message. They
	/// are used by the Assert class as helper objects.
	/// 
	/// User-defined asserters may be passed to the 
	/// Assert.DoAssert method in order to implement 
	/// extended asserts.
	/// </summary>
	public abstract class AbstractAsserter : IAsserter
	{
		/// <summary>
		/// The user-defined message for this asserter.
		/// </summary>
		protected readonly string message;
		
		/// <summary>
		/// Arguments to use in formatting the user-defined message.
		/// </summary>
		protected readonly object[] args;

		/// <summary>
		/// Constructs an AbstractAsserter
		/// </summary>
		/// <param name="message">The message issued upon failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public AbstractAsserter( string message, params object[] args )
		{
			this.message = message;
			this.args = args;
		}

		protected string FormattedMessage
		{
			get
			{
				if ( message == null ) 
					return string.Empty;
				
				if ( args != null && args.Length > 0 )
					return string.Format( message, args );

				return message;
			}
		}

		/// <summary>
		/// Assert on the condition this object is designed
		/// to handle, throwing an exception if it fails.
		/// </summary>
		public abstract void Assert();
	}
}
