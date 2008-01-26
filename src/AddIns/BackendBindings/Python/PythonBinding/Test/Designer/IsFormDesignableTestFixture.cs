// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.FormsDesigner;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the SecondaryDisplayBinding detects that the form
	/// is designable:
	/// 
	/// 1) Has an InitializeComponents method.
	/// 2) Has a base class of Form.
	/// </summary>
	[TestFixture]
	public class IsFormDesignableTestFixture
	{
		IMethod initializeComponentsMethod;
		protected IClass mainFormClass;
		ParseInformation parseInfo;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			PythonParser parser = new PythonParser();
			MockProjectContent mockProjectContent = new MockProjectContent();
			ICompilationUnit compilationUnit = parser.Parse(mockProjectContent, @"C:\Projects\Test\MainForm.py", GetPythonCode());

			parseInfo = new ParseInformation();
			parseInfo.SetCompilationUnit(compilationUnit);
			
			if (compilationUnit.Classes.Count > 0) {
				mainFormClass = compilationUnit.Classes[0];
				initializeComponentsMethod = FormsDesignerSecondaryDisplayBinding.GetInitializeComponents(mainFormClass);
			}
		}
		
		[Test]
		public void InitializeComponentsIsNotNull()
		{
			Assert.IsNotNull(initializeComponentsMethod);
		}
		
		[Test]
		public void BaseClassIsForm()
		{
			Assert.IsTrue(FormsDesignerSecondaryDisplayBinding.BaseClassIsFormOrControl(mainFormClass));
		}
		
		[Test]
		public void IsDesignable()
		{
			Assert.IsTrue(FormsDesignerSecondaryDisplayBinding.IsDesignable(parseInfo));
		}
				
		protected virtual string GetPythonCode()
		{
			return "from System.Windows.Forms import Form\r\n" +
					"\r\n" + 
					"class MainForm(Form):\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tself.InitializeComponent()\r\n" +
					"\r\n" +
					"\tdef InitializeComponent(self):\r\n" +
					"\t\tpass\r\n" +
					"\r\n";
		}
	}
}
