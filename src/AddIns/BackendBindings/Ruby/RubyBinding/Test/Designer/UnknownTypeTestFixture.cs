// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using IronRuby.Compiler.Ast;
using Microsoft.Scripting;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the RubyFormWalker throws a RubyFormWalkerException if a unknown type is used in the
	/// form.
	/// </summary>
	[TestFixture]
	public class UnknownTypeTestFixture
	{
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			ResourceManager rm = new ResourceManager("RubyBinding.Tests.Strings", GetType().Assembly);
			ResourceService.RegisterNeutralStrings(rm);			
		}
		
		[Test]
		public void SelfAssignmentWithUnknownTypeRhs()
		{
			string RubyCode =
				"require \"System.Windows.Forms\"\r\n" +
				"\r\n" +
				"class MainForm < System::Windows::Forms::Form\r\n" +
				"    def initialize()\r\n" +
				"        self.InitializeComponent()\r\n" +
				"    end\r\n "+
				"\r\n" +
				"    def InitializeComponent()\r\n" +
				"        self.ClientSize = Unknown::Type(10)\r\n" +
				"    end\r\n" +
				"end";
			
			try {
				RubyComponentWalker walker = new RubyComponentWalker(new MockComponentCreator());
				walker.CreateComponent(RubyCode);
				Assert.Fail("Exception should have been thrown before this.");
			} catch (RubyComponentWalkerException ex) {
				string expectedMessage = String.Format(StringParser.Parse("${res:ICSharpCode.PythonBinding.UnknownTypeName}"), "Unknown.Type");
				Assert.AreEqual(expectedMessage, ex.Message);
			}
		}
		
		[Test]
		public void LocalVariableAssignmentWithUnknownTypeRhs()
		{
			string RubyCode =
				"require \"System.Windows.Forms\"\r\n" +
				"\r\n" +
				"class MainForm < System::Windows::Forms::Form\r\n" +
				"    def initialize()\r\n" +
				"        self.InitializeComponent()\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    def InitializeComponent()\r\n" +
				"        abc = Unknown::Type(10)\r\n" +
				"   end\r\n" +
				"end";

			try {
				RubyComponentWalker walker = new RubyComponentWalker(new MockComponentCreator());
				walker.CreateComponent(RubyCode);
				Assert.Fail("Exception should have been thrown before this.");
			} catch (RubyComponentWalkerException ex) {
				string expectedMessage = String.Format(StringParser.Parse("${res:ICSharpCode.PythonBinding.UnknownTypeName}"), "Unknown.Type");
				Assert.AreEqual(expectedMessage, ex.Message);
			}
		}
	}
}
