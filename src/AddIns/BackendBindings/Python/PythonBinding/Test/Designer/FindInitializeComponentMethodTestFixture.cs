// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
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
			PythonParser parser = new PythonParser();
			MockProjectContent mockProjectContent = new MockProjectContent();
			ICompilationUnit compilationUnit = parser.Parse(mockProjectContent, @"C:\Projects\Test\MainForm.py", GetFormCode());
	
			// Create parse info to return from ParseFile method.
			parseInfo = new ParseInformation(compilationUnit);
			
			// Get the InitializeComponent method from the
			// compilation unit.
			expectedInitializeComponentMethod = GetInitializeComponentMethod(compilationUnit);
			
			// Find the InitializeComponent method using the designer generator.			
			initializeComponentMethod = PythonDesignerGenerator.GetInitializeComponents(parseInfo);
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
			ParseInformation parseInfo = new ParseInformation(new DefaultCompilationUnit(new MockProjectContent()));
			Assert.IsNull(PythonDesignerGenerator.GetInitializeComponents(parseInfo));
		}		
		
		/// <summary>
		/// Tests that the PythonDesignerGenerator handles the InitializeComponent
		/// method being "InitializeComponents" and not "InitializeComponent".
		/// </summary>
		[Test]
		public void InitializeComponentsUsedInsteadOfInitializeComponent()
		{
			PythonParser parser = new PythonParser();
			MockProjectContent mockProjectContent = new MockProjectContent();
			string code = GetFormCode().Replace("InitializeComponent", "InitializeComponents");
			ICompilationUnit compilationUnit = parser.Parse(mockProjectContent, @"C:\Projects\Test\MainForm.py", code);
			ParseInformation parseInfo = new ParseInformation(compilationUnit);
			IMethod expectedMethod = GetInitializeComponentMethod(compilationUnit);

			IMethod method = PythonDesignerGenerator.GetInitializeComponents(parseInfo);

			Assert.IsNotNull(method);
			Assert.AreSame(expectedMethod, method);
		}
		
		string GetFormCode()
		{
			return "from System.Windows.Forms import Form\r\n" +
					"\r\n" +
					"class IgnoreMe:\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tpass\r\n"+
					"\r\n" +
					"class IgnoreMeSinceIHaveNoInitializeComponentMethod(Form):\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tpass\r\n" +
					"\r\n" +
					"class MainForm(Form):\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tself.InitializeComponent()\r\n" +
					"\t\r\n" +
					"\tdef InitializeComponent(self):\r\n" +
					"\t\tpass\r\n"; 
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
