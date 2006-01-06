namespace NUnit.Framework
{
	/// <summary>
	/// Basic Asserts on strings.
	/// </summary>
	public class StringAssert
	{
		#region Contains

		/// <summary>
		/// Asserts that a string is found within another string.
		/// </summary>
		/// <param name="expected">The expected string</param>
		/// <param name="actual">The string to be examined</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Arguments used in formatting the message</param>
		static public void Contains( string expected, string actual, string message, params object[] args )
		{
			Assert.DoAssert( new ContainsAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Asserts that a string is found within another string.
		/// </summary>
		/// <param name="expected">The expected string</param>
		/// <param name="actual">The string to be examined</param>
		/// <param name="message">The message to display in case of failure</param>
		static public void Contains( string expected, string actual, string message )
		{
			Contains( expected, actual, message, null );
		}

		/// <summary>
		/// Asserts that a string is found within another string.
		/// </summary>
		/// <param name="expected">The expected string</param>
		/// <param name="actual">The string to be examined</param>
		static public void Contains( string expected, string actual )
		{
			Contains( expected, actual, string.Empty, null );
		}

		#endregion

		#region StartsWith

		/// <summary>
		/// Asserts that a string starts with another string.
		/// </summary>
		/// <param name="expected">The expected string</param>
		/// <param name="actual">The string to be examined</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Arguments used in formatting the message</param>
		static public void StartsWith( string expected, string actual, string message, params object[] args )
		{
			Assert.DoAssert( new StartsWithAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Asserts that a string starts with another string.
		/// </summary>
		/// <param name="expected">The expected string</param>
		/// <param name="actual">The string to be examined</param>
		/// <param name="message">The message to display in case of failure</param>
		static public void StartsWith( string expected, string actual, string message )
		{
			StartsWith( expected, actual, message, null );
		}

		/// <summary>
		/// Asserts that a string starts with another string.
		/// </summary>
		/// <param name="expected">The expected string</param>
		/// <param name="actual">The string to be examined</param>
		static public void StartsWith( string expected, string actual )
		{
			StartsWith( expected, actual, string.Empty, null );
		}

		#endregion

		#region EndsWith

		/// <summary>
		/// Asserts that a string ends with another string.
		/// </summary>
		/// <param name="expected">The expected string</param>
		/// <param name="actual">The string to be examined</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Arguments used in formatting the message</param>
		static public void EndsWith( string expected, string actual, string message, params object[] args )
		{
			Assert.DoAssert( new EndsWithAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Asserts that a string ends with another string.
		/// </summary>
		/// <param name="expected">The expected string</param>
		/// <param name="actual">The string to be examined</param>
		/// <param name="message">The message to display in case of failure</param>
		static public void EndsWith( string expected, string actual, string message )
		{
			EndsWith( expected, actual, message, null );
		}

		/// <summary>
		/// Asserts that a string ends with another string.
		/// </summary>
		/// <param name="expected">The expected string</param>
		/// <param name="actual">The string to be examined</param>
		static public void EndsWith( string expected, string actual )
		{
			EndsWith( expected, actual, string.Empty, null );
		}

		#endregion

		#region AreEqualIgnoringCase
		/// <summary>
		/// Asserts that two strings are equal, without regard to case.
		/// </summary>
		/// <param name="expected">The expected string</param>
		/// <param name="actual">The actual string</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Arguments used in formatting the message</param>
		static public void AreEqualIgnoringCase( string expected, string actual, string message, params object[] args )
		{
			Assert.DoAssert( new EqualIgnoringCaseAsserter( expected, actual, message, args ) );
		}

		/// <summary>
		/// Asserts that two strings are equal, without regard to case.
		/// </summary>
		/// <param name="expected">The expected string</param>
		/// <param name="actual">The actual string</param>
		/// <param name="message">The message to display in case of failure</param>
		static public void AreEqualIgnoringCase( string expected, string actual, string message )
		{
			AreEqualIgnoringCase( expected, actual, message, null );
		}

		/// <summary>
		/// Asserts that two strings are equal, without regard to case.
		/// </summary>
		/// <param name="expected">The expected string</param>
		/// <param name="actual">The actual string</param>
		static public void AreEqualIgnoringCase( string expected, string actual )
		{
			AreEqualIgnoringCase( expected, actual, string.Empty, null );
		}

		#endregion
	}
}
