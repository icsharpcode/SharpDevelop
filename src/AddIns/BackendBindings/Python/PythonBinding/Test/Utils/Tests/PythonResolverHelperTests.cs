// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class PythonResolverHelperTests
	{
		[Test]
		public void ParseInfoCreatedWithEmptyUsingCollection()
		{
			string[] usings = new string[0];
			ParseInformation parseInfo = PythonResolverHelper.CreateParseInfoWithUsings(usings);
			Assert.AreEqual(0, parseInfo.MostRecentCompilationUnit.UsingScope.Usings.Count);
		}
		
		[Test]
		public void ParseInfoCreatedWithSingleUsingString()
		{
			string[] usings = new string[] { "System" };
			ParseInformation parseInfo = PythonResolverHelper.CreateParseInfoWithUsings(usings);
			
			IUsingScope scope = parseInfo.MostRecentCompilationUnit.UsingScope;
			Assert.AreEqual("System", scope.Usings[0].Usings[0]);
			Assert.AreEqual(1, scope.Usings.Count);
			Assert.AreEqual(1, scope.Usings[0].Usings.Count);
		}
		
		[Test]
		public void ParseInfoCreatedWithTwoUsingStrings()
		{
			string[] usings = new string[] { "System", "System.Console" };
			ParseInformation parseInfo = PythonResolverHelper.CreateParseInfoWithUsings(usings);
			IUsingScope scope = parseInfo.MostRecentCompilationUnit.UsingScope;
			
			Assert.AreEqual("System", scope.Usings[0].Usings[0]);
			Assert.AreEqual("System.Console", scope.Usings[0].Usings[1]);
			Assert.AreEqual(1, scope.Usings.Count);
			Assert.AreEqual(2, scope.Usings[0].Usings.Count);
		}
	}
}
