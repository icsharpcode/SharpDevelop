using System;

namespace NUnit.Framework
{
	/// <summary>
	/// ConditionAsserter class represents an asssertion
	/// that tests a particular condition, which is passed
	/// to it in the constructor. The failure message is
	/// not specialized in this class, but derived classes
	/// are free to do so.
	/// </summary>
	public class ConditionAsserter : AbstractAsserter
	{
		/// <summary>
		/// The condition we are testing
		/// </summary>
		protected bool condition;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="condition">The condition to be tested</param>
		/// <param name="message">The message issued upon failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public ConditionAsserter( bool condition, string message, params object[] args )
			: base( message, args )
		{
			this.condition = condition;
		}

		/// <summary>
		/// Assert the condition.
		/// </summary>
		public override void Assert()
		{
			if ( !condition )
				NUnit.Framework.Assert.Fail( message, args );
		}
	}

	/// <summary>
	/// Class to assert that a condition is true
	/// </summary>
	public class TrueAsserter : ConditionAsserter
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="condition">The condition to assert</param>
		/// <param name="message">The message to issue on failure</param>
		/// <param name="args">Arguments to apply in formatting the message</param>
		public TrueAsserter( bool condition, string message, params object[] args )
			: base( condition, message, args ) { }
	}

	/// <summary>
	/// Class to assert that a condition is false
	/// </summary>
	public class FalseAsserter : ConditionAsserter
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="condition">The condition to assert</param>
		/// <param name="message">The message to issue on failure</param>
		/// <param name="args">Arguments to apply in formatting the message</param>
		public FalseAsserter( bool condition, string message, params object[] args )
			: base( !condition, message, args ) { }

	}

	/// <summary>
	/// Class to assert that an object is null
	/// </summary>
	public class NullAsserter : ConditionAsserter
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="anObject">The object to test</param>
		/// <param name="message">The message to issue on failure</param>
		/// <param name="args">Arguments to apply in formatting the message</param>
		public NullAsserter( object anObject, string message, params object[] args )
			: base( anObject == null, message, args ) { }
	}

	/// <summary>
	/// Class to assert that an object is not null
	/// </summary>
	public class NotNullAsserter : ConditionAsserter
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="anObject">The object to test</param>
		/// <param name="message">The message to issue on failure</param>
		/// <param name="args">Arguments to apply in formatting the message</param>
		public NotNullAsserter( object anObject, string message, params object[] args )
			: base( anObject != null, message, args ) { }
	}
}
