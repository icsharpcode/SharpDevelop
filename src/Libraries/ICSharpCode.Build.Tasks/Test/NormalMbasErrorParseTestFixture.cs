// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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
	public class NormalMbasErrorParseTestFixture
	{
		Match match;
		
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			string error = "Form1.vb(38,3) error BC30201: Expression expected";

			Regex regex = new Regex(MonoBasicCompilerResultsParser.NormalErrorPattern, RegexOptions.Compiled);
			match = regex.Match(error);
		}
		
		[Test]
		public void Column()
		{
			Assert.AreEqual("3", match.Result("${column}"));
		}
		
		[Test]
		public void Line()
		{
			Assert.AreEqual("38", match.Result("${line}"));
		}
		
		[Test]
		public void FileName()
		{
			Assert.AreEqual("Form1.vb", match.Result("${file}"));
		}
		
		[Test]
		public void Warning()
		{
			Assert.AreEqual("error", match.Result("${error}"));
		}		
		
		[Test]
		public void ErrorNumber()
		{
			Assert.AreEqual("BC30201", match.Result("${number}"));
		}		
		
		[Test]
		public void ErrorText()
		{
			Assert.AreEqual("Expression expected", match.Result("${message}"));
		}	
	}
}
