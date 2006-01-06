using System;

namespace NUnit.Framework
{
	#region TypeAsserter
	/// <summary>
	/// The abstract asserter from which all specific type asserters
	/// will inherit from in order to limit code-reproduction.
	/// </summary>
	public abstract class TypeAsserter : AbstractAsserter
	{
		/// <summary>
		/// The expected Type
		/// </summary>
		protected System.Type   expected;
		
		/// <summary>
		/// The actual object to be compared
		/// </summary>
		protected object        actual;

		/// <summary>
		/// Construct a TypeAsserter
		/// </summary>
		/// <param name="expected">The expected Type</param>
		/// <param name="actual">The object to be examined</param>
		/// <param name="message">A message to display on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public TypeAsserter( System.Type expected, object actual, string message, params object[] args )
			: base( message, args ) 
		{
			this.expected = expected;
			this.actual = actual;
		}

		/// <summary>
		/// The complete message text in case of a failure.
		/// </summary>
		public override string Message
		{
			get
			{
				FailureMessage.AddExpectedLine( Expectation );
				FailureMessage.AddActualLine( actual.GetType().ToString() );
				return FailureMessage.ToString();
			}
		}

		/// <summary>
		/// A string representing what was expected. Used as a part of the message text.
		/// </summary>
		protected virtual string Expectation
		{
			get { return expected.ToString(); }
		}
	}
	#endregion

	#region AssignableFromAsserter
	/// <summary>
	/// Class to Assert that an object may be assigned from a given Type.
	/// </summary>
	public class AssignableFromAsserter : TypeAsserter
	{
		/// <summary>
		/// Construct an AssignableFromAsserter
		/// </summary>
		/// <param name="expected">The expected Type</param>
		/// <param name="actual">The object being examined</param>
		/// <param name="message">A message to display in case of failure</param>
		/// <param name="args">Arguments for use in formatting the message</param>
		public AssignableFromAsserter( System.Type expected, object actual, string message, params object[] args )
			: base( expected, actual, message, args ) { }

		/// <summary>
		/// Test the object to determine if it can be assigned from the expected Type
		/// </summary>
		/// <returns>True if the object is assignable</returns>
		public override bool Test()
		{
			return actual.GetType().IsAssignableFrom(expected);
		}

		/// <summary>
		/// A string representing what was expected. Used as a part of the message text.
		/// </summary>
		protected override string Expectation
		{
			get { return string.Format( "Type assignable from {0}", expected ); }
		}

	}
	#endregion

	#region NotAssignableFromAsserter
	/// <summary>
	/// Class to Assert that an object may not be assigned from a given Type.
	/// </summary>
	public class NotAssignableFromAsserter : TypeAsserter
	{
		/// <summary>
		/// Construct a NotAssignableFromAsserter
		/// </summary>
		/// <param name="expected">The expected Type</param>
		/// <param name="actual">The object to be examined</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Arguments to use in formatting the message</param>
		public NotAssignableFromAsserter( System.Type expected, object actual, string message, params object[] args )
			: base( expected, actual, message, args ) { }

		/// <summary>
		/// Test the object to determine if it can be assigned from the expected Type
		/// </summary>
		/// <returns>True if the object is not assignable</returns>
		public override bool Test()
		{
			return !actual.GetType().IsAssignableFrom(expected);
		}

		/// <summary>
		/// A string representing what was expected. Used as a part of the message text.
		/// </summary>
		protected override string Expectation
		{
			get { return string.Format( "Type not assignable from {0}", expected ); }
		}

	}
	#endregion

	#region InstanceOfTypeAsserter
	/// <summary>
	/// Class to Assert that an object is an instance of a given Type.
	/// </summary>
	public class InstanceOfTypeAsserter : TypeAsserter
	{
		/// <summary>
		/// Construct an InstanceOfTypeAsserter
		/// </summary>
		/// <param name="expected">The expected Type</param>
		/// <param name="actual">The object to be examined</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Arguments to use in formatting the message</param>
		public InstanceOfTypeAsserter( System.Type expected, object actual, string message, params object[] args )
			: base( expected, actual, message, args ) { }

		/// <summary>
		/// Test the object to determine if it is an instance of the expected Type
		/// </summary>
		/// <returns>True if the object is an instance of the expected Type</returns>
		public override bool Test()
		{
			return expected.IsInstanceOfType( actual );
		}

		/// <summary>
		/// A string representing what was expected. Used as a part of the message text.
		/// </summary>
		protected override string Expectation
		{
			get
			{
				return string.Format( "Object to be instance of {0}", expected );
			}
		}

	}
	#endregion

	#region NotInstanceOfTypeAsserter
	/// <summary>
	/// Class to Assert that an object is not an instance of a given Type.
	/// </summary>
	public class NotInstanceOfTypeAsserter : TypeAsserter
	{
		/// <summary>
		/// Construct a NotInstanceOfTypeAsserter
		/// </summary>
		/// <param name="expected">The expected Type</param>
		/// <param name="actual">The object to be examined</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Arguments to use in formatting the message</param>
		public NotInstanceOfTypeAsserter( System.Type expected, object actual, string message, params object[] args )
			: base( expected, actual, message, args ) { }

		/// <summary>
		/// Test the object to determine if it is an instance of the expected Type
		/// </summary>
		/// <returns>True if the object is not an instance of the expected Type</returns>
		public override bool Test()
		{
			return !expected.IsInstanceOfType( actual );
		}

		/// <summary>
		/// A string representing what was expected. Used as a part of the message text.
		/// </summary>
		protected override string Expectation
		{
			get
			{
				return string.Format( "Object not an instance of {0}", expected );
			}
		}

	}
	#endregion
}
