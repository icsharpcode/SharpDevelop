using System;
using System.Text;

namespace NUnit.Framework
{
	/// <summary>
	/// Class to assert that two objects are equal 
	/// </summary>
	public class EqualAsserter : EqualityAsserter
	{
		/// <summary>
		/// Constructs an EqualAsserter for two objects
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message to issue on failure</param>
		/// <param name="args">Arguments to apply in formatting the message</param>
		public EqualAsserter( object expected, object actual, string message, params object[] args )
			: base( expected, actual, message, args ) { }

		public EqualAsserter( double expected, double actual, double delta, string message, params object[] args )
			: base( expected, actual, delta, message, args ) { }

		/// <summary>
		/// Assert that the objects are equal
		/// </summary>
		/// <returns>True if they are equal, false if not</returns>
		public override void Assert()
		{
			if ( expected == null && actual == null ) return;
			if ( expected == null || actual == null )
				FailNotEqual();

			// For now, dynamically call array assertion if necessary. Try to move
			// this into the ObjectsEqual method later on.
			if ( expected.GetType().IsArray && actual.GetType().IsArray )
			{
				Array expectedArray = expected as Array;
				Array actualArray = actual as Array;

				if ( expectedArray.Rank != actualArray.Rank )
					FailNotEqual();
				
				if ( expectedArray.Rank != 1 )
					NUnit.Framework.Assert.Fail( "Multi-dimension array comparison is not supported" );

				int iLength = Math.Min( expectedArray.Length, actualArray.Length );
				for( int i = 0; i < iLength; i++ )
					if ( !ObjectsEqual( expectedArray.GetValue( i ), actualArray.GetValue( i ) ) )
						FailArraysNotEqual( i );
	
				if ( expectedArray.Length != actualArray.Length )
					FailArraysNotEqual( iLength );
			}
			else 
			{
				if ( !ObjectsEqual( expected, actual ) )
					FailNotEqual();
			}
		}

		private void FailNotEqual()
		{
			AssertionFailureMessage msg = new AssertionFailureMessage( message, args );
			msg.DisplayDifferences( expected, actual, false );
			throw new AssertionException( msg.ToString() );
		}

		private void FailArraysNotEqual( int index )
		{
			throw new AssertionException( 
				AssertionFailureMessage.FormatMessageForFailArraysNotEqual( 
					index,
					(Array)expected, 
					(Array)actual, 
					message,
					args ) );
		}
	}
}
