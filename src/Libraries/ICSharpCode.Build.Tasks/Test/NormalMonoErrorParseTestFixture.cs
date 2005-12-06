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
	public class NormalMonoErrorParseTestFixture
	{
		Match match;
		
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			string error = "MyClass.cs(19,7): warning CS0169: The private field `Foo.MyClass.foo' is never used";

			Regex regex = new Regex(CompilerResultsParser.NormalErrorPattern, RegexOptions.Compiled);
			match = regex.Match(error);
		}
		
		[Test]
		public void Column()
		{
			Assert.AreEqual("7", match.Result("${column}"));
		}
		
		[Test]
		public void Line()
		{
			Assert.AreEqual("19", match.Result("${line}"));
		}
		
		[Test]
		public void FileName()
		{
			Assert.AreEqual("MyClass.cs", match.Result("${file}"));
		}
		
		[Test]
		public void Warning()
		{
			Assert.AreEqual("warning", match.Result("${error}"));
		}		
		
		[Test]
		public void ErrorNumber()
		{
			Assert.AreEqual("CS0169", match.Result("${number}"));
		}		
		
		[Test]
		public void ErrorText()
		{
			Assert.AreEqual("The private field `Foo.MyClass.foo' is never used", match.Result("${message}"));
		}				
	}
}
