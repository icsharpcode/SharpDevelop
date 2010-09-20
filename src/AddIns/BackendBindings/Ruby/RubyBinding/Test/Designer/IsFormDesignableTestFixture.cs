// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.FormsDesigner;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
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
			ICompilationUnit compilationUnit = parser.Parse(mockProjectContent, @"C:\Projects\test\MainForm.py", GetRubyCode());

			parseInfo = new ParseInformation(compilationUnit);
			
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
