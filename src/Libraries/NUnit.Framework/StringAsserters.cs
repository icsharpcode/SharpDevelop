using System;

namespace NUnit.Framework
{
	#region StringAsserter
	/// <summary>
	/// Abstract class used as a base for asserters that compare
	/// expected and an actual string values in some way or another.
	/// </summary>
	public abstract class StringAsserter : AbstractAsserter
	{
		/// <summary>
		/// The expected value, used as the basis for comparison.
		/// </summary>
		protected string expected;

		/// <summary>
		/// The actual value to be compared.
		/// </summary>
		protected string actual;

		/// <summary>
		/// Constructs a StringAsserter for two strings
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message to issue on failure</param>
		/// <param name="args">Arguments to apply in formatting the message</param>
		public StringAsserter( string expected, string actual, string message, params object[] args )
			: base( message, args ) 
		{
			this.expected = expected;
			this.actual = actual;
		}

		/// <summary>
		/// Message related to a failure. If no failure has
		/// occured, the result is unspecified.
		/// </summary>
		public override string Message
		{
			get
			{
				FailureMessage.AddExpectedLine( Expectation );
				FailureMessage.DisplayActualValue( actual );
				return FailureMessage.ToString();
			}
		}

		/// <summary>
		/// String value that represents what the asserter expected
		/// to find. Defaults to the expected value itself.
		/// </summary>
		protected virtual string Expectation
		{
			get { return string.Format( "<\"{0}\">", expected ); }
		}
	}
	#endregion

	#region ContainsAsserter
	/// <summary>
	/// Summary description for ContainsAsserter.
	/// </summary>
	public class ContainsAsserter : StringAsserter
	{
		/// <summary>
		/// Constructs a ContainsAsserter for two strings
		/// </summary>
		/// <param name="expected">The expected substring</param>
		/// <param name="actual">The actual string to be examined</param>
		/// <param name="message">The message to issue on failure</param>
		/// <param name="args">Arguments to apply in formatting the message</param>
		public ContainsAsserter( string expected, string actual, string message, params object[] args )
			: base( expected, actual, message, args ) { }

		/// <summary>
		/// Test the assertion.
		/// </summary>
		/// <returns>True if the test succeeds</returns>
		public override bool Test()
		{
			return actual.IndexOf( expected ) >= 0;
		}

		/// <summary>
		/// String value that represents what the asserter expected
		/// </summary>
		protected override string Expectation
		{
			get { return string.Format( "String containing \"{0}\"", expected ); }
		}
	}
	#endregion

	#region StartsWithAsserter
	/// <summary>
	/// Summary description for StartsWithAsserter.
	/// </summary>
	public class StartsWithAsserter : StringAsserter
	{
		/// <summary>
		/// Constructs a StartsWithAsserter for two strings
		/// </summary>
		/// <param name="expected">The expected substring</param>
		/// <param name="actual">The actual string to be examined</param>
		/// <param name="message">The message to issue on failure</param>
		/// <param name="args">Arguments to apply in formatting the message</param>
		public StartsWithAsserter( string expected, string actual, string message, params object[] args )
			: base( expected, actual, message, args ) { }

		/// <summary>
		/// Test the assertion.
		/// </summary>
		/// <returns>True if the test succeeds</returns>
		public override bool Test()
		{
			return actual.StartsWith( expected );
		}

		/// <summary>
		/// String value that represents what the asserter expected
		/// </summary>
		protected override string Expectation
		{
			get { return string.Format( "String starting with \"{0}\"", expected ); }
		}
	}
	#endregion

	#region EndsWithAsserter
	/// <summary>
	/// Summary description for EndsWithAsserter.
	/// </summary>
	public class EndsWithAsserter : StringAsserter
	{
		/// <summary>
		/// Constructs a EndsWithAsserter for two strings
		/// </summary>
		/// <param name="expected">The expected substring</param>
		/// <param name="actual">The actual string to be examined</param>
		/// <param name="message">The message to issue on failure</param>
		/// <param name="args">Arguments to apply in formatting the message</param>
		public EndsWithAsserter( string expected, string actual, string message, params object[] args )
			: base( expected, actual, message, args ) { }

		/// <summary>
		/// Test the assertion.
		/// </summary>
		/// <returns>True if the test succeeds</returns>
		public override bool Test()
		{
			return actual.EndsWith( expected );
		}

		/// <summary>
		/// String value that represents what the asserter expected
		/// </summary>
		protected override string Expectation
		{
			get { return string.Format( "String ending with \"{0}\"", expected ); }
		}
	}
	#endregion

	#region EqualIgnoringCaseAsserter
	/// <summary>
	/// Asserter that implements AreEqualIgnoringCase
	/// </summary>
	public class EqualIgnoringCaseAsserter : StringAsserter
	{
		/// <summary>
		/// Constructs an EqualIgnoringCaseAsserter for two strings
		/// </summary>
		/// <param name="expected">The expected string</param>
		/// <param name="actual">The actual string</param>
		/// <param name="message">The message to issue on failure</param>
		/// <param name="args">Arguments to apply in formatting the message</param>
		public EqualIgnoringCaseAsserter( string expected, string actual, string message, params object[] args )
			: base( expected, actual, message, args ) { }

		/// <summary>
		/// Test the assertion.
		/// </summary>
		/// <returns>True if the test succeeds</returns>
		public override bool Test()
		{
			return string.Compare( expected, actual, true ) == 0;
		}

		/// <summary>
		/// String value that represents what the asserter expected
		/// </summary>
		public override string Message
		{
			get
			{
				FailureMessage.DisplayDifferences( expected, actual, true );
				return FailureMessage.ToString();
			}
		}
	}
	#endregion
}
