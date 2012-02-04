// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.JavaScriptBinding;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using JavaScriptBinding.Tests.Helpers;
using NUnit.Framework;

namespace JavaScriptBinding.Tests
{
	[TestFixture]
	public class JavaScriptParserTests
	{
		JavaScriptParser parser;
		ICompilationUnit compilationUnit;
		
		void CreateParser()
		{
			parser = new JavaScriptParser();
		}
		
		IProject CreateCSharpProject()
		{
			var createInfo = new ProjectCreateInformation();
			createInfo.Solution = new Solution(new MockProjectChangeWatcher());
			createInfo.OutputProjectFileName = @"d:\projects\MyProject\MyProject.csproj";
			return new MSBuildBasedProject(createInfo);
		}
		
		void ParseEmptyJavaScriptFileWithFileName(string fileName)
		{
			ParseJavaScript(fileName, String.Empty);
		}
		
		void ParseEmptyJavaScriptFileWithProjectContent(IProjectContent projectContent)
		{
			Parse(projectContent, String.Empty, String.Empty);
		}
		
		void ParseJavaScript(string code)
		{
			ParseJavaScript("test.js", code);
		}
		
		void ParseJavaScript(string fileName, string code)
		{
			var projectContent = new DefaultProjectContent();
			Parse(projectContent, fileName, code);
		}
		
		void Parse(IProjectContent projectContent, string fileName, string code)
		{
			var textBuffer = new FakeTextBuffer(code);
			CreateParser();
			compilationUnit = parser.Parse(projectContent, fileName, textBuffer);
		}
		
		IClass FirstClass {
			get { return compilationUnit.Classes[0]; }
		}
		
		IMethod FirstClassFirstMethod {
			get { return FirstClass.Methods[0]; }
		}
		
		FoldingRegion FirstRegion {
			get { return compilationUnit.FoldingRegions[0]; }
		}
		
		FoldingRegion SecondRegion {
			get { return compilationUnit.FoldingRegions[1]; }
		}
		
		[Test]
		public void CanParse_CSharpProjectPassed_ReturnsTrue()
		{
			CreateParser();
			IProject project = CreateCSharpProject();
			bool result = parser.CanParse(project);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void CanParse_JavaScriptFileNamePassed_ReturnsTrue()
		{
			CreateParser();
			bool result = parser.CanParse("test.js");
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void CanParse_TextFileNamePassed_ReturnsFalse()
		{
			CreateParser();
			bool result = parser.CanParse("test.txt");
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void CanParse_NullFileNamePassed_ReturnsFalse()
		{
			CreateParser();
			bool result = parser.CanParse((string)null);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void CanParse_JavaScriptFileNameInUpperCasePassed_ReturnsTrue()
		{
			CreateParser();
			bool result = parser.CanParse("TEST.JS");
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Parse_FileNamePassed_ReturnsCompilationUnitWithFileName()
		{
			string expectedFileName = @"d:\projects\test\MyScript.js";
			ParseEmptyJavaScriptFileWithFileName(expectedFileName);
			
			string fileName = compilationUnit.FileName;
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void Parse_ProjectContentPassed_ReturnsCompilationUnitWithProjectContent()
		{
			var expectedProjectContent = new DefaultProjectContent();
			ParseEmptyJavaScriptFileWithProjectContent(expectedProjectContent);
			
			IProjectContent projectContent = compilationUnit.ProjectContent;
			Assert.AreEqual(expectedProjectContent, projectContent);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasOneFunction_ReturnsOneClassInCompilationUnit()
		{
			string code = 
				"function test() {\r\n " +
				"}\r\n";
			
			ParseJavaScript(code);
			
			Assert.AreEqual(1, compilationUnit.Classes.Count);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasOneFunction_ClassNameIsFileNameWithoutExtension()
		{
			string code = 
				"function test() {\r\n " +
				"}\r\n";
			
			ParseJavaScript("myscript.js", code);
			
			string className = FirstClass.Name;
			
			Assert.AreEqual("myscript", className);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasOneFunction_ReturnsOneMethodInClass()
		{
			string code = 
				"function test() {\r\n " +
				"}\r\n";
			
			CreateParser();
			ParseJavaScript(code);
			
			Assert.AreEqual(1, FirstClass.Methods.Count);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasOneFunction_MethodNameExistsInClass()
		{
			string code = 
				"function test() {\r\n " +
				"}\r\n";
			
			CreateParser();
			ParseJavaScript(code);
			
			string methodName = FirstClassFirstMethod.Name;
			
			Assert.AreEqual("test", methodName);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasOneFunctionCalledMyFunction_MethodNameExistsInClass()
		{
			string code = 
				"function MyFunction() {\r\n " +
				"}\r\n";
			
			ParseJavaScript(code);
			
			string methodName = FirstClassFirstMethod.Name;
			
			Assert.AreEqual("MyFunction", methodName);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasOnlyFunctionKeyword_NoMethodsParsed()
		{
			string code = "function";
			
			ParseJavaScript(code);
			
			Assert.AreEqual(0, FirstClass.Methods.Count);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasFunctionKeywordAndFunctionNameOnly_NoOverflowExceptionThrown()
		{
			string code = "function test";
			
			Assert.DoesNotThrow(() => ParseJavaScript(code));
			
			string methodName = FirstClassFirstMethod.Name;
			
			Assert.AreEqual("test", methodName);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasTwoFunctions_TwoMethodsReturned()
		{
			string code = 
				"function one() { }\r\n" +
				"function two() { }\r\n";
			
			ParseJavaScript(code);
			
			int count = FirstClass.Methods.Count;
			
			Assert.AreEqual(2, count);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasOneFunctionSpanningThreeLines_MethodHeaderRegionCoversStartOfFunctionToMethodRightParenthesis()
		{
			string code = 
				"function myfunction() {\r\n" +
				"    alert('test');\r\n" +
				"}\r\n";
			
			ParseJavaScript(code);
			
			DomRegion methodHeaderRegion = FirstClassFirstMethod.Region;
			
			int beginLine = 1;
			int endLine = 1;
			int beginColumn = 1;
			int endColumn = 22;
			
			var expectedRegion = new DomRegion(beginLine, beginColumn, endLine, endColumn);
			
			Assert.AreEqual(expectedRegion, methodHeaderRegion);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasOneFunctionSpanningThreeLines_MethodBodyRegionCoversFunctionFromAfterParametersToRightBrace()
		{
			string code = 
				"function myfunction() {\r\n" +
				"    alert('test');\r\n" +
				"}\r\n";
			
			ParseJavaScript(code);
			
			DomRegion methodBodyRegion = FirstClassFirstMethod.BodyRegion;
			
			int beginLine = 1;
			int endLine = 3;
			int beginColumn = 22;
			int endColumn = 2;
			
			var expectedRegion = new DomRegion(beginLine, beginColumn, endLine, endColumn);
			
			Assert.AreEqual(expectedRegion, methodBodyRegion);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasOneRegion_OneFoldingRegionAddedToCompilationUnit()
		{
			string code = 
				"//#region MyRegion\r\n" +
				"var a = 1;\r\n" +
				"//#endregion\r\n";
			
			ParseJavaScript(code);
			
			int count = compilationUnit.FoldingRegions.Count;
			
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasOneRegionWithName_FoldingRegionAddedWithName()
		{
			string code = 
				"//#region MyRegion\r\n" +
				"var a = 1;\r\n" +
				"//#endregion\r\n";
			
			ParseJavaScript(code);
			
			string name = FirstRegion.Name;
			
			Assert.AreEqual("MyRegion", name);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasNoRegions_NoFoldingRegionsAddedToCompilationUnit()
		{
			string code = "var a = 1;";
			
			ParseJavaScript(code);
			
			int count = compilationUnit.FoldingRegions.Count;
			
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void Parse_JavaScriptCodeSingleCommentEndsWithRegionTextButDoesNotStartWithRegion_NoRegionsAddedToCompilationUnit()
		{
			string code = 
				"//not a #region test\r\n" +
				"var a = 1;\r\n" +
				"//not an #endregion\r\n";
			
			ParseJavaScript(code);
			
			Assert.AreEqual(0, compilationUnit.FoldingRegions.Count);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasOneRegion_RegionLocationIsSpecified()
		{
			string code = 
				"//#region MyRegion\r\n" +
				"var a = 1;\r\n" +
				"//#endregion\r\n";
			
			ParseJavaScript(code);
			
			DomRegion location = FirstRegion.Region;
			
			int beginLine = 1;
			int endLine = 3;
			int beginColumn = 1;
			int endColumn = 13;
			
			var expectedLocation = new DomRegion(beginLine, beginColumn, endLine, endColumn);
			
			Assert.AreEqual(expectedLocation, location);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasRegionStartOnly_NoRegionsAddedToCompilationUnit()
		{
			string code = 
				"//#region MyRegion\r\n" +
				"var a = 1;\r\n";
			
			ParseJavaScript(code);
			
			Assert.AreEqual(0, compilationUnit.FoldingRegions.Count);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasOneRegionWithEndRegionFollowedByText_TextFollowingEndRegionIsNotPartOfFold()
		{
			string code = 
				"//#region MyRegion\r\n" +
				"var a = 1;\r\n" +
				"//#endregion abc\r\n";
			
			ParseJavaScript(code);
			
			DomRegion location = FirstRegion.Region;
			
			int beginLine = 1;
			int endLine = 3;
			int beginColumn = 1;
			int endColumn = 13;
			
			var expectedLocation = new DomRegion(beginLine, beginColumn, endLine, endColumn);
			
			Assert.AreEqual(expectedLocation, location);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasTwoRegions_TwoRegionsAddedToCompilationUnit()
		{
			string code = 
				"//#region One\r\n" +
				"var a = 1;\r\n" +
				"//#endregion\r\n" +
				"\r\n" +
				"//#region Two\r\n" +
				"var b = 1;\r\n" +
				"//#endregion\r\n";
			
			ParseJavaScript(code);
			
			Assert.AreEqual(2, compilationUnit.FoldingRegions.Count);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasTwoRegions_RegionsHaveCorrectLocationsAndNames()
		{
			string code = 
				"//#region One\r\n" +
				"var a = 1;\r\n" +
				"//#endregion\r\n" +
				"\r\n" +
				"//#region Two\r\n" +
				"var b = 1;\r\n" +
				"//#endregion\r\n";
			
			ParseJavaScript(code);
			
			DomRegion firstRegionLocation = FirstRegion.Region;
			
			int beginLine = 1;
			int endLine = 3;
			int beginColumn = 1;
			int endColumn = 13;
			
			var expectedFirstRegionLocation = new DomRegion(beginLine, beginColumn, endLine, endColumn);
			
			DomRegion secondRegionLocation = SecondRegion.Region;
			
			beginLine = 5;
			endLine = 7;
			beginColumn = 1;
			endColumn = 13;
			
			var expectedSecondRegionLocation = new DomRegion(beginLine, beginColumn, endLine, endColumn);
			
			Assert.AreEqual(expectedFirstRegionLocation, firstRegionLocation);
			Assert.AreEqual("One", FirstRegion.Name);
			Assert.AreEqual(expectedSecondRegionLocation, secondRegionLocation);
			Assert.AreEqual("Two", SecondRegion.Name);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasEndRegionButNoStartRegion_NoRegionsAddedToCompilationUnit()
		{
			string code = 
				"var a = 1;\r\n" +
				"//#endregion\r\n";
			
			ParseJavaScript(code);
			
			Assert.AreEqual(0, compilationUnit.FoldingRegions.Count);
		}
		
		[Test]
		public void Parse_JavaScriptCodeHasTwoNestedRegions_RegionsHaveCorrectLocationsAndNames()
		{
			string code = 
				"//#region One\r\n" +
				"var a = 1;\r\n" +
				"//#region Two\r\n" +
				"var b = 1;\r\n" +
				"//#endregion\r\n" +
				"var c = 1;\r\n" +
				"//#endregion\r\n";
			
			ParseJavaScript(code);
			
			DomRegion firstRegionLocation = FirstRegion.Region;
			
			int beginLine = 3;
			int endLine = 5;
			int beginColumn = 1;
			int endColumn = 13;
			
			var expectedFirstRegionLocation = new DomRegion(beginLine, beginColumn, endLine, endColumn);
			
			DomRegion secondRegionLocation = SecondRegion.Region;
			
			beginLine = 1;
			endLine = 7;
			beginColumn = 1;
			endColumn = 13;
			
			var expectedSecondRegionLocation = new DomRegion(beginLine, beginColumn, endLine, endColumn);
			
			Assert.AreEqual(expectedFirstRegionLocation, firstRegionLocation);
			Assert.AreEqual("Two", FirstRegion.Name);
			Assert.AreEqual(expectedSecondRegionLocation, secondRegionLocation);
			Assert.AreEqual("One", SecondRegion.Name);
		}
	}
}
