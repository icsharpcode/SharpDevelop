using System;

namespace NUnit.Framework
{
	/// <summary>
	/// Abstract base class for EqualsAsserter and NotEqualsAsserter
	/// </summary>
	public abstract class EqualityAsserter : ComparisonAsserter
	{
		private double delta;

		/// <summary>
		/// Constructor taking expected and actual values and a user message with arguments.
		/// </summary>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <param name="message"></param>
		/// <param name="args"></param>
		public EqualityAsserter( object expected, object actual, string message, params object[] args )
			: base( expected, actual, message, args ) { }

		/// <summary>
		/// Constructor taking expected and actual values, a tolerance
		/// and a user message and arguments.
		/// </summary>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <param name="delta"></param>
		/// <param name="message"></param>
		/// <param name="args"></param>
		public EqualityAsserter( double expected, double actual, double delta, string message, params object[] args )
			: base( expected, actual, message, args )
		{
			this.delta = delta;
		}

		/// <summary>
		/// Used to compare two objects.  Two nulls are equal and null
		/// is not equal to non-null. Comparisons between the same
		/// numeric types are fine (Int32 to Int32, or Int64 to Int64),
		/// but the Equals method fails across different types so we
		/// use <c>ToString</c> and compare the results.
		/// </summary>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <returns></returns>
		protected virtual bool ObjectsEqual( Object expected, Object actual )
		{
			if ( expected == null && actual == null ) return true;
			if ( expected == null || actual == null ) return false;

			//if ( expected.GetType().IsArray && actual.GetType().IsArray )
			//	return ArraysEqual( (System.Array)expected, (System.Array)actual );

			if ( expected is double && actual is double )
			{
				if ( double.IsNaN((double)expected) && double.IsNaN((double)actual) )
					return true;
				// handle infinity specially since subtracting two infinite values gives 
				// NaN and the following test fails. mono also needs NaN to be handled
				// specially although ms.net could use either method.
				if (double.IsInfinity((double)expected) || double.IsNaN((double)expected) || double.IsNaN((double)actual))
					return expected.Equals( actual );
				else 
					return Math.Abs((double)expected-(double)actual) <= this.delta;
			}

			//			if ( expected is float && actual is float )
			//			{
			//				// handle infinity specially since subtracting two infinite values gives 
			//				// NaN and the following test fails. mono also needs NaN to be handled
			//				// specially although ms.net could use either method.
			//				if (float.IsInfinity((float)expected) || float.IsNaN((float)expected) || float.IsNaN((float)actual))
			//					return (float)expected == (float)actual;
			//				else 
			//					return Math.Abs((float)expected-(float)actual) <= (float)this.delta;
			//			}

			if ( expected.GetType() != actual.GetType() &&
				IsNumericType( expected )  && IsNumericType( actual ) )
			{
				//
				// Convert to strings and compare result to avoid
				// issues with different types that have the same
				// value
				//
				string sExpected = expected.ToString();
				string sActual   = actual.ToString();
				return sExpected.Equals( sActual );
			}
			return expected.Equals(actual);
		}

		/// <summary>
		/// Checks the type of the object, returning true if
		/// the object is a numeric type.
		/// </summary>
		/// <param name="obj">The object to check</param>
		/// <returns>true if the object is a numeric type</returns>
		private bool IsNumericType( Object obj )
		{
			if( null != obj )
			{
				if( obj is byte    ) return true;
				if( obj is sbyte   ) return true;
				if( obj is decimal ) return true;
				if( obj is double  ) return true;
				if( obj is float   ) return true;
				if( obj is int     ) return true;
				if( obj is uint    ) return true;
				if( obj is long    ) return true;
				if( obj is short   ) return true;
				if( obj is ushort  ) return true;

				if( obj is System.Byte    ) return true;
				if( obj is System.SByte   ) return true;
				if( obj is System.Decimal ) return true;
				if( obj is System.Double  ) return true;
				if( obj is System.Single  ) return true;
				if( obj is System.Int32   ) return true;
				if( obj is System.UInt32  ) return true;
				if( obj is System.Int64   ) return true;
				if( obj is System.UInt64  ) return true;
				if( obj is System.Int16   ) return true;
				if( obj is System.UInt16  ) return true;
			}
			return false;
		}

		/// <summary>
		/// Helper method to compare two arrays
		/// </summary>
		protected virtual bool ArraysEqual( Array expected, Array actual )
		{
			if ( expected.Rank != actual.Rank )
				return false;

			if ( expected.Rank != 1 )
				throw new ArgumentException( "Multi-dimension array comparison is not supported" );

			int iLength = Math.Min( expected.Length, actual.Length );
			for( int i = 0; i < iLength; i++ )
				if ( !ObjectsEqual( expected.GetValue( i ), actual.GetValue( i ) ) )
					return false;
	
			if ( expected.Length != actual.Length )
				return false;

			return true;
		}
	}
}
