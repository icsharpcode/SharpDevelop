using System;

namespace NUnit.Framework
{
	/// <summary>
	/// Asserter that verifies two objects are different.
	/// </summary>
	public class NotSameAsserter : ComparisonAsserter
	{
		/// <summary>
		/// Construct a NotSameAsserter object
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">A user-defined message for use in reporting errors</param>
		/// <param name="args">Arguments to be used in formatting the user-defined message</param>
		public NotSameAsserter( object expected, object actual, string message, params object[] args)
			: base( expected, actual, message, args ) { }

		/// <summary>
		/// Tests that the objects are not the same
		/// </summary>
		public override bool Test()
		{
			return !object.ReferenceEquals( expected, actual );
		}

		/// <summary>
		/// Provides a message in case of failure
		/// </summary>
		public override string Message
		{
			get
			{
				FailureMessage.Write( "expected not same" );
				return FailureMessage.ToString();
			}
		}
	}
}
