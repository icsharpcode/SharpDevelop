// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.TextTemplating;
using NUnit.Framework;
using TextTemplating.Tests.Helpers;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class TextTemplatingVariablesTests
	{
		TextTemplatingVariables variables;
		FakeTextTemplatingStringParser fakeStringParser;
		
		void CreateTextTemplatingVariables()
		{
			fakeStringParser = new FakeTextTemplatingStringParser();
			variables = new TextTemplatingVariables(fakeStringParser);
		}
		
		void AddProperty(string name, string value)
		{
			fakeStringParser.AddProperty(name, value);
		}
		
		[Test]
		public void ExpandVariables_SolutionDirProperty_SolutionDirPropertyIsExpanded()
		{
			CreateTextTemplatingVariables();
			AddProperty("SolutionDir", @"d:\projects\MyProject\");
			
			string result = variables.ExpandVariables(@"$(SolutionDir)bin\Debug\Test.dll");
			
			string expectedResult = @"d:\projects\MyProject\bin\Debug\Test.dll";
			Assert.AreEqual(expectedResult, result);
		}
		
		[Test]
		public void ExpandVariables_ProjectDirProperty_ProjectDirPropertyIsExpanded()
		{
			CreateTextTemplatingVariables();
			AddProperty("ProjectDir", @"d:\projects\MyProject\");
			
			string result = variables.ExpandVariables(@"$(ProjectDir)bin\Debug\Test.dll");
			
			string expectedResult = @"d:\projects\MyProject\bin\Debug\Test.dll";
			Assert.AreEqual(expectedResult, result);
		}
		
		[Test]
		public void GetVariableLocations_SolutionDirProperty_ReturnsSolutionDirVariable()
		{
			CreateTextTemplatingVariables();
			
			TextTemplatingVariableLocation location = variables.GetVariables("$(SolutionDir)").First();
			
			var expectedLocation = new TextTemplatingVariableLocation() {
				VariableName = "SolutionDir",
				Index = 0,
				Length = 14
			};
			
			Assert.AreEqual(expectedLocation, location);
		}
		
		[Test]
		public void GetVariableLocations_NullPassed_ReturnsNoVariables()
		{
			CreateTextTemplatingVariables();
			List<TextTemplatingVariableLocation> locations = 
				variables.GetVariables(null).ToList();
			
			Assert.AreEqual(0, locations.Count);
		}
		
		[Test]
		public void GetVariableLocations_EmptyStringPassed_ReturnsNoVariables()
		{
			CreateTextTemplatingVariables();
			List<TextTemplatingVariableLocation> locations = 
				variables.GetVariables(String.Empty).ToList();
			
			Assert.AreEqual(0, locations.Count);
		}
		
		[Test]
		public void GetVariableLocations_TextHasNoVariables_ReturnsNoVariables()
		{
			CreateTextTemplatingVariables();
			List<TextTemplatingVariableLocation> locations = 
				variables.GetVariables("No Variables").ToList();
			
			Assert.AreEqual(0, locations.Count);
		}
		
		[Test]
		public void GetVariableLocations_TextHasVariableStartButNoEnd_ReturnsNoVariables()
		{
			CreateTextTemplatingVariables();
			List<TextTemplatingVariableLocation> locations = 
				variables.GetVariables("$(No Variables").ToList();
			
			Assert.AreEqual(0, locations.Count);
		}
		
		[Test]
		public void GetVariableLocations_TwoProperties_ReturnsTwoVariables()
		{
			CreateTextTemplatingVariables();
			
			List<TextTemplatingVariableLocation> locations = 
				variables.GetVariables("$(SolutionDir)$(ProjectDir)").ToList();
			
			var expectedLocation1 = new TextTemplatingVariableLocation() {
				VariableName = "SolutionDir",
				Index = 0,
				Length = 14
			};
			
			var expectedLocation2 = new TextTemplatingVariableLocation() {
				VariableName = "ProjectDir",
				Index = 14,
				Length = 13
			};
			
			var expectedLocations = new TextTemplatingVariableLocation[] {
				expectedLocation1,
				expectedLocation2
			};
			
			CollectionAssert.AreEqual(expectedLocations, locations);
		}
		
		[Test]
		public void GetValue_ProjectDirProperty_ReturnsProjectDir()
		{
			CreateTextTemplatingVariables();
			AddProperty("ProjectDir", @"d:\projects\MyProject\");
			
			string variableValue = variables.GetValue("ProjectDir");
			
			string expectedVariableValue = @"d:\projects\MyProject\";
			Assert.AreEqual(expectedVariableValue, variableValue);
		}
		
		[Test]
		public void GetValue_ProjectDirPropertyHasNoTrailingSlash_ReturnsProjectDirWithTrailingSlash()
		{
			CreateTextTemplatingVariables();
			AddProperty("ProjectDir", @"d:\projects\MyProject");
			
			string variableValue = variables.GetValue("ProjectDir");
			
			string expectedVariableValue = @"d:\projects\MyProject\";
			Assert.AreEqual(expectedVariableValue, variableValue);
		}
		
		[Test]
		public void GetValue_PropertyReturnsEmptyStringAsValue_ReturnsEmptyString()
		{
			CreateTextTemplatingVariables();
			AddProperty("ProjectDir", String.Empty);
			
			string variableValue = variables.GetValue("ProjectDir");
			
			Assert.AreEqual(String.Empty, variableValue);
		}
		
		[Test]
		public void GetValue_PropertyReturnsNullAsValue_ReturnsEmptyString()
		{
			CreateTextTemplatingVariables();
			AddProperty("Test", null);
			
			string variableValue = variables.GetValue("Test");
			
			Assert.AreEqual(String.Empty, variableValue);
		}
		
		[Test]
		public void GetValue_NonDirectoryPropertyReturnsNonEmptyStringAsValue_DoesNotAppendTrailingSlash()
		{
			CreateTextTemplatingVariables();
			AddProperty("Test", "abc");
			
			string variableValue = variables.GetValue("Test");
			
			Assert.AreEqual("abc", variableValue);
		}
		
		[Test]
		public void GetValue_SolutionDirPropertyWithoutTrailingSlash_ReturnsExpandedSolutionDirWithTrailingSlash()
		{
			CreateTextTemplatingVariables();
			AddProperty("SolutionDir", @"d:\projects\MySolution");
			
			string variableValue = variables.GetValue("SolutionDir");
			
			string expectedVariableValue = @"d:\projects\MySolution\";
			Assert.AreEqual(expectedVariableValue, variableValue);
		}
		
		[Test]
		public void GetValue_SolutionDirPropertyInLowerCaseWithoutTrailingSlash_ReturnsExpandedSolutionDirWithTrailingSlash()
		{
			CreateTextTemplatingVariables();
			AddProperty("solutiondir", @"d:\projects\MySolution");
			
			string variableValue = variables.GetValue("solutiondir");
			
			string expectedVariableValue = @"d:\projects\MySolution\";
			Assert.AreEqual(expectedVariableValue, variableValue);
		}
		
		[Test]
		public void GetValue_NullPassed_ReturnsEmptyString()
		{
			CreateTextTemplatingVariables();
			
			string variableValue = variables.GetValue(null);
			
			Assert.AreEqual(String.Empty, variableValue);
		}
		
		[Test]
		public void ExpandVariables_ProjectDirPropertyHasNoTrailingSlash_ProjectDirPropertyIsExpandedWithTrailingSlash()
		{
			CreateTextTemplatingVariables();
			AddProperty("ProjectDir", @"d:\projects\MyProject");
			
			string result = variables.ExpandVariables(@"$(ProjectDir)bin\Debug\Test.dll");
			
			string expectedResult = @"d:\projects\MyProject\bin\Debug\Test.dll";
			Assert.AreEqual(expectedResult, result);
		}
		
		[Test]
		public void ExpandVariables_AddInPathHasNoTrailingSlash_AddInPathPropertyIsExpandedWithTrailingSlash()
		{
			CreateTextTemplatingVariables();
			AddProperty("addinpath:ICSharpCode.MyAddIn", @"d:\SD\AddIns\MyAddIn");
			
			string result = variables.ExpandVariables(@"$(addinpath:ICSharpCode.MyAddIn)MyAddIn.dll");
			
			string expectedResult = @"d:\SD\AddIns\MyAddIn\MyAddIn.dll";
			Assert.AreEqual(expectedResult, result);
		}
		
		[Test]
		public void ExpandVariables_AddInPathInUpperCaseWithoutTrailingSlash_AddInPathPropertyIsExpandedWithTrailingSlash()
		{
			CreateTextTemplatingVariables();
			AddProperty("ADDINPATH:ICSharpCode.MyAddIn", @"d:\SD\AddIns\MyAddIn");
			
			string result = variables.ExpandVariables(@"$(ADDINPATH:ICSharpCode.MyAddIn)MyAddIn.dll");
			
			string expectedResult = @"d:\SD\AddIns\MyAddIn\MyAddIn.dll";
			Assert.AreEqual(expectedResult, result);
		}
	}
}
