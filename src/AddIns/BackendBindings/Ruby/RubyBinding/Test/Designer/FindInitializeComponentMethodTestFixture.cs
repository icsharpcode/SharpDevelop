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
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the GeneratedInitializeComponentMethod class locates 
	/// the InitializeComponents method in the source code. 
	/// 
	/// Note that the source code contains a non-designable class
	/// before the Form. This tests that the first designable class
	/// is correctly picked up.
	/// </summary>
	[TestFixture]
	public class FindInitializeComponentMethodTestFixture
	{
		IMethod initializeComponentMethod;
		ParseInformation parseInfo;
		IMethod expectedInitializeComponentMethod;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{			
			RubyParser parser = new RubyParser();
			MockProjectContent mockProjectContent = new MockProjectContent();
			ICompilationUnit compilationUnit = parser.Parse(mockProjectContent, @"C:\Projects\Test\MainForm.rb", GetFormCode());
	
			// Create parse info to return from ParseFile method.
			parseInfo = new ParseInformation(compilationUnit);
			
			// Get the InitializeComponent method from the
			// compilation unit.
			expectedInitializeComponentMethod = GetInitializeComponentMethod(compilationUnit);
			
			// Find the InitializeComponent method using the designer generator.			
			initializeComponentMethod = RubyDesignerGenerator.GetInitializeComponents(compilationUnit);
		}
		
		/// <summary>
		/// Sanity check. Make sure we found the InitializeComponent method
		/// from the compilation unit during the SetUpFixture method.
		/// </summary>
		[Test]
		public void ExpectedInitializeComponentMethodFound()
		{
			Assert.IsNotNull(expectedInitializeComponentMethod);
		}
		
		[Test]
		public void InitializeComponentMethodFound()
		{
			Assert.AreSame(expectedInitializeComponentMethod, initializeComponentMethod);
		}
				
		[Test]
		public void GetInitializeComponentWhenNoClassesInCompilationUnit()
		{
			DefaultCompilationUnit unit = new DefaultCompilationUnit(new MockProjectContent());
			ParseInformation parseInfo = new ParseInformation(unit);
			Assert.IsNull(RubyDesignerGenerator.GetInitializeComponents(unit));
		}		
		
		/// <summary>
		/// Tests that the RubyDesignerGenerator handles the InitializeComponent
		/// method being "InitializeComponents" and not "InitializeComponent".
		/// </summary>
		[Test]
		public void InitializeComponentsUsedInsteadOfInitializeComponent()
		{
			RubyParser parser = new RubyParser();
			MockProjectContent mockProjectContent = new MockProjectContent();
			string code = GetFormCode().Replace("InitializeComponent", "InitializeComponents");
			ICompilationUnit compilationUnit = parser.Parse(mockProjectContent, @"C:\Projects\Test\MainForm.rb", code);
			ParseInformation parseInfo = new ParseInformation(compilationUnit);
			IMethod expectedMethod = GetInitializeComponentMethod(compilationUnit);

			IMethod method = RubyDesignerGenerator.GetInitializeComponents(compilationUnit);

			Assert.IsNotNull(method);
			Assert.AreSame(expectedMethod, method);
		}
		
		string GetFormCode()
		{
			return "require \"System.Windows.Forms\"\r\n" +
					"\r\n" +
					"class IgnoreMe\r\n" +
					"\tdef initialize()\r\n" +
					"\tend\r\n"+
					"end\r\n" +
					"\r\n" +
					"class IgnoreMeSinceIHaveNoInitializeComponentMethod < Form\r\n" +
					"\tdef initialize()\r\n" +
					"\tend\r\n" +
					"end\r\n" +
					"\r\n" +
					"class MainForm < Form\r\n" +
					"\tdef initialize()\r\n" +
					"\t\tself.InitializeComponent()\r\n" +
					"\tend\r\n" +
					"\t\r\n" +
					"\tdef InitializeComponent()\r\n" +
					"\tend\r\n" +
					"end"; 
		}
		
		static IMethod GetInitializeComponentMethod(ICompilationUnit unit)
		{
			IClass c = unit.Classes[2];
			foreach (IMethod m in c.Methods) {
				if (m.Name == "InitializeComponent" || m.Name == "InitializeComponents") {
					return m;
				}
			}
			return null;
		}
	}
}
