// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace ICSharpCode.SharpDevelop.Dom.Tests
{
	[TestFixture]
	public class CodeSnippetConverterTests
	{
		IProjectContent mscorlib, system;
		CodeSnippetConverter converter;
		string errors;
		
		public CodeSnippetConverterTests()
		{
			ProjectContentRegistry pcr = new ProjectContentRegistry();
			mscorlib = pcr.Mscorlib;
			system = pcr.GetProjectContentForReference("System", "System");
		}
		
		[SetUp]
		public void SetUp()
		{
			converter = new CodeSnippetConverter {
				ReferencedContents = {
					mscorlib, system
				}
			};
		}
		
		[Test]
		public void FixExpressionCase()
		{
			Assert.AreEqual("AppDomain.CurrentDomain", converter.VBToCSharp("appdomain.currentdomain", out errors));
		}
	}
}
