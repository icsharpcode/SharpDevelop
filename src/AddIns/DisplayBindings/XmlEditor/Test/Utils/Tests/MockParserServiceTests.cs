// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Utils.Tests
{
	[TestFixture]
	public class MockParserServiceTests
	{
		MockParserService parserService;
		ParseInformation existingParseInfo;
		
		[SetUp]
		public void Init()
		{
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			DefaultCompilationUnit unit = new DefaultCompilationUnit(projectContent);
			existingParseInfo = new ParseInformation(unit);
			
			parserService = new MockParserService();
			parserService.SetExistingParseInformation(@"d:\projects\test.xml", existingParseInfo);
		}
		
		[Test]
		public void CanGetProjectContentPassedToGetExistingParseInfoMethod()
		{
			DefaultProjectContent projectContent = new DefaultProjectContent();
			parserService.GetExistingParseInformation(projectContent, "file.xml");
			Assert.AreSame(projectContent, parserService.ProjectContentPassedToGetExistingParseInforMethod);
		}
		
		[Test]
		public void GetExistingParseInfoReturnsParseInfoObjectForKnownFileName()
		{
			Assert.AreSame(existingParseInfo, parserService.GetExistingParseInformation(null, @"d:\projects\test.xml"));
		}
		
		[Test]
		public void GetExistingParseInfoReturnsNullForKnownFileName()
		{
			Assert.IsNull(parserService.GetExistingParseInformation(null, @"d:\projects\unknown.xml"));
		}
	}
}
