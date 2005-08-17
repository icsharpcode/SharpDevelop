using System;

namespace NUnit.Framework
{
	/// <summary>
	/// Asserter that verifies two objects are the same.
	/// </summary>
	public class SameAsserter : ComparisonAsserter
	{
		/// <summary>
		/// Construct a SameAsserter object
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">A user-defined message for use in reporting errors</param>
		/// <param name="args">Arguments to be used in formatting the user-defined message</param>
		public SameAsserter( object expected, object actual, string message, params object[] args )
			: base( expected, actual, message, args ) { }

		/// <summary>
		/// Assert that the objects are the same
		/// </summary>
		public override void Assert()
		{
			if ( ! object.ReferenceEquals( expected, actual ) )
			{
				string formatted = FormattedMessage;
				if ( formatted.Length > 0 ) 
					formatted += " ";

				throw new AssertionException( formatted + "expected same" );
			}
		}
	}
}
