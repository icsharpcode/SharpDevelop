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

		/// <summary>
		/// Constructs an EqualAsserter for two doubles and a tolerance
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="tolerance">The tolerance used in making the comparison</param>
		/// <param name="message">The message to issue on failure</param>
		/// <param name="args">Arguments to apply in formatting the message</param>
		public EqualAsserter( double expected, double actual, double tolerance, string message, params object[] args )
			: base( expected, actual, tolerance, message, args ) { }

		/// <summary>
		/// Test whether the objects are equal, building up
		/// the failure message for later use if they are not.
		/// </summary>
		/// <returns>True if the objects are equal</returns>
		public override bool Test()
		{
			if ( expected == null && actual == null ) return true;
			if ( expected == null || actual == null )
			{
				DisplayDifferences();
				return false;
			}

			// For now, dynamically call array assertion if necessary. Try to move
			// this into the ObjectsEqual method later on.
			if ( expected.GetType().IsArray && actual.GetType().IsArray )
			{
				Array expectedArray = expected as Array;
				Array actualArray = actual as Array;

				if ( expectedArray.Rank != actualArray.Rank )
				{
					DisplayDifferences();
					return false;
				}
				
				if ( expectedArray.Rank != 1 )
					throw new ArgumentException( "Multi-dimension array comparison is not supported" );

				int iLength = Math.Min( expectedArray.Length, actualArray.Length );
				for( int i = 0; i < iLength; i++ )
					if ( !ObjectsEqual( expectedArray.GetValue( i ), actualArray.GetValue( i ) ) )
					{
						DisplayArrayDifferences( i );
						return false;
					}
	
				if ( expectedArray.Length != actualArray.Length )
				{
					DisplayArrayDifferences( iLength );
					return false;
				}
			}
			else 
			{
				if ( !ObjectsEqual( expected, actual ) )
				{
					DisplayDifferences();
					return false;
				}
			}

			return true;
		}

		private void DisplayDifferences()
		{
			FailureMessage.DisplayDifferences( expected, actual, false );
		}


		private void DisplayArrayDifferences( int index )
		{
			FailureMessage.DisplayArrayDifferences( (Array)expected, (Array)actual, index );
		}
	}
}
