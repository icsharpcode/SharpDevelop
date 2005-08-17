#region Copyright (c) 2002-2003, James W. Newkirk, Michael C. Two, Alexei A. Vorontsov, Charlie Poole, Philip A. Craig, Douglas de la Torre
/************************************************************************************
'
' Copyright  2002-2003 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov, Charlie Poole
' Copyright  2000-2002 Philip A. Craig
' Copyright  2001 Douglas de la Torre
'
' This software is provided 'as-is', without any express or implied warranty. In no 
' event will the authors be held liable for any damages arising from the use of this 
' software.
' 
' Permission is granted to anyone to use this software for any purpose, including 
' commercial applications, and to alter it and redistribute it freely, subject to the 
' following restrictions:
'
' 1. The origin of this software must not be misrepresented; you must not claim that 
' you wrote the original software. If you use this software in a product, an 
' acknowledgment (see the following) in the product documentation is required.
'
' Portions Copyright  2002 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov 
' Copyright  2000-2002 Philip A. Craig, or Copyright  2001 Douglas de la Torre
'
' 2. Altered source versions must be plainly marked as such, and must not be 
' misrepresented as being the original software.
'
' 3. This notice may not be removed or altered from any source distribution.
'
'***********************************************************************************/
#endregion

using System;
using System.Text;
using System.IO;

namespace NUnit.Framework
{
	/// <summary>
	/// Summary description for AssertionFailureMessage.
	/// </summary>
	public class AssertionFailureMessage : StringWriter
	{
		#region Static Constants

		/// <summary>
		/// Number of characters before a highlighted position before
		/// clipping will occur.  Clipped text is replaced with an
		/// elipsis "..."
		/// </summary>
		static public readonly int PreClipLength = 35;

		/// <summary>
		/// Number of characters after a highlighted position before
		/// clipping will occur.  Clipped text is replaced with an
		/// elipsis "..."
		/// </summary>
		static public readonly int PostClipLength = 35;

		static protected readonly string ExpectedText = "expected:<";
		static protected readonly string ButWasText = " but was:<";

		static private readonly string expectedFmt = "\texpected:<{0}>";
		static private readonly string butWasFmt = "\t but was:<{0}>"; 
		static private readonly string diffStringLengthsFmt 
			= "\tString lengths differ.  Expected length={0}, but was length={1}.";
		static private readonly string sameStringLengthsFmt
			= "\tString lengths are both {0}.";
		static private readonly string diffArrayLengthsFmt
			= "Array lengths differ.  Expected length={0}, but was length={1}.";
		static private readonly string sameArrayLengthsFmt
			= "Array lengths are both {0}.";
		static private readonly string stringsDifferAtIndexFmt
			= "\tStrings differ at index {0}.";
		static private readonly string arraysDifferAtIndexFmt
			= "Arrays differ at index {0}.";

		#endregion

		#region Constructors

		/// <summary>
		/// Construct an AssertionFailureMessage with a message
		/// and optional arguments.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		public AssertionFailureMessage( string message, params object[] args )
			: base( CreateStringBuilder( message, args ) ) { }

		/// <summary>
		/// Construct an empty AssertionFailureMessage
		/// </summary>
		public AssertionFailureMessage() : this( null, null ) { }

		#endregion

		/// <summary>
		/// Display two lines that communicate the expected value, and the actual value
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value found</param>
		public void DisplayExpectedAndActual( Object expected, Object actual )
		{
			WriteLine();
			Write( expectedFmt, DisplayString( expected ) );
			WriteLine();
			Write( butWasFmt, DisplayString( actual ) );
		}

		/// <summary>
		/// Draws a marker under the expected/actual strings that highlights
		/// where in the string a mismatch occurred.
		/// </summary>
		/// <param name="iPosition">The position of the mismatch</param>
		public void DisplayPositionMarker( int iPosition )
		{
			WriteLine();
			Write( "\t" + new String( '-', ButWasText.Length + 1 ) );
			if( iPosition > 0 )
			{
				Write( new string( '-', iPosition ) );
			}
			Write( "^" );
		}

		/// <summary>
		/// Reports whether the string lengths are the same or different, and
		/// what the string lengths are.
		/// </summary>
		/// <param name="sExpected">The expected string</param>
		/// <param name="sActual">The actual string value</param>
		protected void BuildStringLengthReport( string sExpected, string sActual )
		{
			WriteLine();
			if( sExpected.Length != sActual.Length )
				Write( diffStringLengthsFmt, sExpected.Length, sActual.Length );
			else
				Write( sameStringLengthsFmt, sExpected.Length );
		}

		/// <summary>
		/// Called to create additional message lines when two objects have been 
		/// found to be unequal.  If the inputs are strings, a special message is
		/// rendered that can help track down where the strings are different,
		/// based on differences in length, or differences in content.
		/// 
		/// If the inputs are not strings, the ToString method of the objects
		/// is used to show what is different about them.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="caseInsensitive">True if a case-insensitive comparison is being performed</param>
		public void DisplayDifferences( object expected, object actual, bool caseInsensitive )
		{
			if( InputsAreStrings( expected, actual ) )
			{
				DisplayStringDifferences( 
					(string)expected, 
					(string)actual,
					caseInsensitive );
			}
			else
			{
				DisplayExpectedAndActual( expected, actual );
			}
		}

		/// <summary>
		/// Constructs a message that can be displayed when the content of two
		/// strings are different, but the string lengths are the same.  The
		/// message will clip the strings to a reasonable length, centered
		/// around the first position where they are mismatched, and draw 
		/// a line marking the position of the difference to make comparison
		/// quicker.
		/// </summary>
		/// <param name="sExpected">The expected string value</param>
		/// <param name="sActual">The actual string value</param>
		/// <param name="caseInsensitive">True if a case-insensitive comparison is being performed</param>
		protected void DisplayStringDifferences( string sExpected, string sActual, bool caseInsensitive )
		{
			//
			// If they mismatch at a specified position, report the
			// difference.
			//
			int iPosition = caseInsensitive
				? FindMismatchPosition( sExpected.ToLower(), sActual.ToLower(), 0 )
				: FindMismatchPosition( sExpected, sActual, 0 );
			//
			// If the lengths differ, but they match up to the length,
			// show the difference just past the length of the shorter
			// string
			//
			if( iPosition == -1 ) 
				iPosition = Math.Min( sExpected.Length, sActual.Length );
			
			BuildStringLengthReport( sExpected, sActual );

			WriteLine();
			Write( stringsDifferAtIndexFmt, iPosition );

			//
			// Clips the strings, then turns any hidden whitespace into visible
			// characters
			//
			string sClippedExpected = ConvertWhitespace(ClipAroundPosition( sExpected, iPosition ));
			string sClippedActual   = ConvertWhitespace(ClipAroundPosition( sActual,   iPosition ));

			DisplayExpectedAndActual( 
				sClippedExpected, 
				sClippedActual );

			// Add a line showing where they differ.  If the string lengths are
			// different, they start differing just past the length of the 
			// shorter string
			DisplayPositionMarker( caseInsensitive
				? FindMismatchPosition( sClippedExpected.ToLower(), sClippedActual.ToLower(), 0 )
				: FindMismatchPosition( sClippedExpected, sClippedActual, 0 ) );
		}

		private void DisplayAdditionalElements( string label, Array array, int index, int max )
		{
			WriteLine();
			Write( "{0}<", label );

			for( int i = 0; i < max; i++ )
			{
				Write( DisplayString( array.GetValue(index++) ) );
				
				if ( index >= array.Length )
					break;

				Write( "," );
			}

			if ( index < array.Length )
				Write( "..." );

			Write( ">" );
		}

		#region Static Methods

		/// <summary>
		/// Display an object as a string
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		static protected string DisplayString( object  obj )
		{
			if ( obj == null ) 
				return "(null)";
			else if ( obj is string )
				return Quoted( (string)obj );
			else
				return obj.ToString();
		}

		/// <summary>
		/// Quote a string
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		static protected string Quoted( string text )
		{
			return string.Format( "\"{0}\"", text );
		}

		/// <summary>
		/// Tests two objects to determine if they are strings.
		/// </summary>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <returns></returns>
		static protected bool InputsAreStrings( Object expected, Object actual )
		{
			return expected != null && actual != null && 
				expected is string && actual is string;
		}

		/// <summary>
		/// Used to create a StringBuilder that is used for constructing
		/// the output message when text is different.  Handles initialization
		/// when a message is provided.  If message is null, an empty
		/// StringBuilder is returned.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		static protected StringBuilder CreateStringBuilder( string message, params object[] args )
		{
			if (message != null) 
				if ( args != null && args.Length > 0 )
					return new StringBuilder( string.Format( message, args ) );
				else
					return new StringBuilder( message );
			else
				return new StringBuilder();
		}

		/// <summary>
		/// Renders up to M characters before, and up to N characters after
		/// the specified index position.  If leading or trailing text is
		/// clipped, and elipses "..." is added where the missing text would
		/// be.
		/// 
		/// Clips strings to limit previous or post newline characters,
		/// since these mess up the comparison
		/// </summary>
		/// <param name="sString"></param>
		/// <param name="iPosition"></param>
		/// <returns></returns>
		static protected string ClipAroundPosition( string sString, int iPosition )
		{
			if( sString == null || sString.Length == 0 )
				return "";

			bool preClip = iPosition > PreClipLength;
			bool postClip = iPosition + PostClipLength < sString.Length;

			int start = preClip 
				? iPosition - PreClipLength : 0;
			int length = postClip 
				? iPosition + PostClipLength - start : sString.Length - start;

			if ( start + length > iPosition + PostClipLength )
				length = iPosition + PostClipLength - start;

			StringBuilder sb = new StringBuilder();
			if ( preClip ) sb.Append("...");
			sb.Append( sString.Substring( start, length ) );
			if ( postClip ) sb.Append("...");

			return sb.ToString();
		}

		/// <summary>
		/// Shows the position two strings start to differ.  Comparison 
		/// starts at the start index.
		/// </summary>
		/// <param name="sExpected"></param>
		/// <param name="sActual"></param>
		/// <param name="iStart"></param>
		/// <returns>-1 if no mismatch found, or the index where mismatch found</returns>
		static private int FindMismatchPosition( string sExpected, string sActual, int iStart )
		{
			int iLength = Math.Min( sExpected.Length, sActual.Length );
			for( int i=iStart; i<iLength; i++ )
			{
				//
				// If they mismatch at a specified position, report the
				// difference.
				//
				if( sExpected[i] != sActual[i] )
				{
					return i;
				}
			}
			//
			// Strings have same content up to the length of the shorter string.
			// Mismatch occurs because string lengths are different, so show
			// that they start differing where the shortest string ends
			//
			if( sExpected.Length != sActual.Length )
			{
				return iLength;
			}
            
			//
			// Same strings
			//
			Assert.IsTrue( sExpected.Equals( sActual ) );
			return -1;
		}

		/// <summary>
		/// Turns CR, LF, or TAB into visual indicator to preserve visual marker 
		/// position.   This is done by replacing the '\r' into '\\' and 'r' 
		/// characters, and the '\n' into '\\' and 'n' characters, and '\t' into
		/// '\\' and 't' characters.  
		/// 
		/// Thus the single character becomes two characters for display.
		/// </summary>
		/// <param name="sInput"></param>
		/// <returns></returns>
		static protected string ConvertWhitespace( string sInput )
		{
			if( null != sInput )
			{
				sInput = sInput.Replace( "\r", "\\r" );
				sInput = sInput.Replace( "\n", "\\n" );
				sInput = sInput.Replace( "\t", "\\t" );
			}
			return sInput;
		}

		/// <summary>
		/// Called to create a message when two arrays are not equal. 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <param name="message"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		static public string FormatMessageForFailArraysNotEqual(int index, Array expected, Array actual, 
			string message, params object[] args) 
		{
			AssertionFailureMessage msg = new AssertionFailureMessage( message, args );
			
			msg.WriteLine();
			if( expected.Length != actual.Length )
				msg.Write( diffArrayLengthsFmt, expected.Length, actual.Length );
			else
				msg.Write( sameArrayLengthsFmt, expected.Length );
			
			msg.WriteLine();
			msg.Write( arraysDifferAtIndexFmt, index );
				
			if ( index < expected.Length && index < actual.Length )
				msg.DisplayDifferences( expected.GetValue( index ), actual.GetValue( index ), false );
			else if( expected.Length < actual.Length )
				msg.DisplayAdditionalElements( "   extra:", actual, index, 3 );
			else
				msg.DisplayAdditionalElements( " missing:", expected, index, 3 );

			return msg.ToString();
		}

		#endregion
	}
}
