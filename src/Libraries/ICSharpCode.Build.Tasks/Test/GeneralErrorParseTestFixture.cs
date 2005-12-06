// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Build.Tasks;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace ICSharpCode.Build.Tasks.Tests
{
	[TestFixture]
	public class GeneralMonoErrorParseTestFixture
	{
		Match match;
		
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			string error = "error CS1904: `CS0169' is not a valid warning number";

			Regex regex = new Regex(CompilerResultsParser.GeneralErrorPattern, RegexOptions.Compiled);
			match = regex.Match(error);
		}
		
		[Test]
		public void Error()
		{
			Assert.AreEqual("error", match.Result("${error}"));
		}		
		
		[Test]
		public void ErrorNumber()
		{
			Assert.AreEqual("CS1904", match.Result("${number}"));
		}		
		
		[Test]
		public void ErrorText()
		{
			Assert.AreEqual("`CS0169' is not a valid warning number", match.Result("${message}"));
		}				
	}
}
