using System;

namespace NUnit.Framework
{
	/// <summary>
	/// The interface implemented by an asserter. Asserters
	/// encapsulate a condition test and generation of an
	/// AssertionException with a tailored message. They
	/// are used by the Assert class as helper objects.
	/// 
	/// User-defined asserters may be passed to the
	/// Assert.DoAssert method in order to implement
	/// extended asserts.
	/// </summary>
	public interface IAsserter
	{
		/// <summary>
		/// Test the condition for the assertion.
		/// </summary>
		/// <returns>True if the test succeeds</returns>
		bool Test();

		/// <summary>
		/// Return the message giving the failure reason.
		/// The return value is unspecified if no failure
		/// has occured.
		/// </summary>
		string Message { get; }
	}
}
