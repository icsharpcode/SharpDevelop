using System;

namespace NUnit.Framework
{
	/// <summary>
	/// NotEqualAsserter is the asserter class that handles 
	/// inequality assertions.
	/// </summary>
	public class NotEqualAsserter : EqualityAsserter
	{
		/// <summary>
		/// Constructor for NotEqualAsserter
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		/// <param name="message">The message to be printed when the two objects are the same object.</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public NotEqualAsserter( object expected, object actual, string message, params object[] args )
			: base( expected, actual, message, args ) { }

		/// <summary>
		/// Test that the objects are not equal
		/// </summary>
		public override bool Test()
		{
			if ( expected == null && actual == null ) return false;
			if ( expected == null || actual == null ) return true;

			if ( expected.GetType().IsArray && actual.GetType().IsArray )
				return !ArraysEqual( (Array)expected, (Array)actual );
			else 
				return !ObjectsEqual( expected, actual );
		}
	}
}
