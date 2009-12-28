// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.FormsDesigner;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
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
			RubyParser parser = new RubyParser();
			MockProjectContent mockProjectContent = new MockProjectContent();
			ICompilationUnit compilationUnit = parser.Parse(mockProjectContent, @"C:\Projects    est\MainForm.py", GetRubyCode());

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
				
		protected virtual string GetRubyCode()
		{
			return "require \"System.Windows.Forms\"\r\n" +
					"\r\n" + 
					"class MainForm < Form\r\n" +
					"    def initialize()\r\n" +
					"        self.InitializeComponent()\r\n" +
					"    end\r\n" +
					"\r\n" +
					"    def InitializeComponent()\r\n" +
					"    end\r\n" +
					"end\r\n" +
					"\r\n";
		}
	}
}
