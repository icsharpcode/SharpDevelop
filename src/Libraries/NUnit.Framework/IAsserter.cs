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
		/// Assert the truth of the condition, throwing an 
		/// exception if the condition is false.
		/// </summary>
		void Assert();
	}
}
