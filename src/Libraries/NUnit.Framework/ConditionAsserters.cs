using System;
using System.Collections;

namespace NUnit.Framework
{
	#region ConditionAsserter
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
		/// Phrase indicating what we expected to find
		/// Ignored unless set by derived class
		/// </summary>
		protected string expectation;

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
		/// Test the condition being asserted directly
		/// </summary>
		public override bool Test()
		{
			return condition;
		}
	}
	#endregion

	#region TrueAsserter
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
	#endregion

	#region FalseAsserter
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
	#endregion

	#region NullAsserter
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
	#endregion

	#region NotNullAsserter
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
	#endregion

	#region NaNAsserter
	/// <summary>
	/// Class to assert that a double is an NaN
	/// </summary>
	public class NaNAsserter : ConditionAsserter
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="aDouble">The value to test</param>
		/// <param name="message">The message to issue on failure</param>
		/// <param name="args">Arguments to apply in formatting the message</param>
		public NaNAsserter( double aDouble, string message, params object[] args )
			: base( double.IsNaN( aDouble ), message, args ) { }
	}
	#endregion

	#region EmptyAsserter
	/// <summary>
	/// Class to Assert that a string or collection is empty
	/// </summary>
	public class EmptyAsserter : ConditionAsserter
	{
		/// <summary>
		/// Construct an EmptyAsserter for a string
		/// </summary>
		/// <param name="aString">The string to be tested</param>
		/// <param name="message">The message to display if the string is not empty</param>
		/// <param name="args">Arguements to use in formatting the message</param>
		public EmptyAsserter( string aString, string message, params object[] args )
			: base( aString == string.Empty, message, args ) { }

		/// <summary>
		/// Construct an EmptyAsserter for a collection
		/// </summary>
		/// <param name="collection">The collection to be tested</param>
		/// <param name="message">The message to display if the collection is not empty</param>
		/// <param name="args">Arguements to use in formatting the message</param>
		public EmptyAsserter( ICollection collection, string message, params object[] args )
			: base( collection.Count == 0, message, args ) { }
	}
	#endregion

	#region NotEmptyAsserter
	/// <summary>
	/// Class to Assert that a string or collection is not empty
	/// </summary>
	public class NotEmptyAsserter : ConditionAsserter
	{
		/// <summary>
		/// Construct a NotEmptyAsserter for a string
		/// </summary>
		/// <param name="aString">The string to be tested</param>
		/// <param name="message">The message to display if the string is empty</param>
		/// <param name="args">Arguements to use in formatting the message</param>
		public NotEmptyAsserter( string aString, string message, params object[] args )
			: base( aString != string.Empty, message, args ) { }

		/// <summary>
		/// Construct a NotEmptyAsserter for a collection
		/// </summary>
		/// <param name="collection">The collection to be tested</param>
		/// <param name="message">The message to display if the collection is empty</param>
		/// <param name="args">Arguements to use in formatting the message</param>
		public NotEmptyAsserter( ICollection collection, string message, params object[] args )
			: base( collection.Count != 0, message, args ) { }
	}
	#endregion
}
