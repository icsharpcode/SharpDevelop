using System;
using System.Collections;

namespace NUnit.Framework
{
	/// <summary>
	/// ListContentsAsserter implements an assertion that a given
	/// item is found in an array or other List.
	/// </summary>
	public class ListContentsAsserter : AbstractAsserter
	{
		private object expected;
		private IList list;

		/// <summary>
		/// Constructs a ListContentsAsserter for a particular list and object.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="list">The list to be examined</param>
		/// <param name="message">The message to issue on failure</param>
		/// <param name="args">Arguments to apply in formatting the message</param>
		public ListContentsAsserter( object expected, IList list, string message, params object[] args )
			: base( message, args )
		{
			this.expected = expected;
			this.list = list;
		}

		/// <summary>
		/// Test that the object is contained in the list
		/// </summary>
		/// <returns>True if the object is found</returns>
		public override bool Test()
		{
			return list != null && list.Contains( expected );
		}

		/// <summary>
		/// Error message to display after a failure.
		/// </summary>
		public override string Message
		{
			get
			{
				FailureMessage.DisplayExpectedValue( expected );
				FailureMessage.DisplayListElements( "\t but was: ", list, 0, 5 );
				return FailureMessage.ToString();
			}
		}

	}
}
