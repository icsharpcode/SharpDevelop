using System;

namespace NUnit.Framework
{
	/// <summary>
	/// Summary description for NotEqualAsserter.
	/// </summary>
	public class NotEqualAsserter : EqualityAsserter
	{
		public NotEqualAsserter( object expected, object actual, string message, params object[] args )
			: base( expected, actual, message, args ) { }

		public override void Assert()
		{
			if ( expected == null && actual == null ) Fail();
			if ( expected == null || actual == null ) return;

			if ( expected.GetType().IsArray && actual.GetType().IsArray )
			{
				if ( ArraysEqual( (Array)expected, (Array)actual ) )
					Fail();
			}
			else if ( ObjectsEqual( expected, actual ) )
				Fail();
		}

		public bool Fail()
		{
			throw new AssertionException( FormattedMessage );
		}
	}
}
