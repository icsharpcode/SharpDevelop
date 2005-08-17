using System;

namespace NUnit.Framework
{
	/// <summary>
	/// Abstract class used as a base for asserters that compare
	/// expected and an actual values in some way or another.
	/// </summary>
	public abstract class ComparisonAsserter : AbstractAsserter
	{
		/// <summary>
		/// The expected value, used as the basis for comparison.
		/// </summary>
		protected object expected;

		/// <summary>
		/// The actual value to be compared.
		/// </summary>
		protected object actual;

		/// <summary>
		/// Constructs a ComparisonAsserter for two objects
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message to issue on failure</param>
		/// <param name="args">Arguments to apply in formatting the message</param>
		public ComparisonAsserter( object expected, object actual, string message, params object[] args )
			: base( message, args )
		{
			this.expected = expected;
			this.actual = actual;
		}
	}
}
