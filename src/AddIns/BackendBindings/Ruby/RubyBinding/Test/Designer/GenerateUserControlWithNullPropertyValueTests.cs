// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Designer;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests that a null property value does not cause a NullReferenceException.
	/// </summary>
	[TestFixture]
	public class GenerateUserControlWithNullPropertyValueTests : GenerateUserControlWithNullPropertyValueTestsBase
	{		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode =
				"@userControl1 = ICSharpCode::Scripting::Tests::Utils::NullPropertyUserControl.new()\r\n" +
				"self.SuspendLayout()\r\n" +
				"# \r\n" +
				"# userControl1\r\n" +
				"# \r\n" +
				"@userControl1.FooBar = nil\r\n" +
				"@userControl1.Location = System::Drawing::Point.new(0, 0)\r\n" +
				"@userControl1.Name = \"userControl1\"\r\n" +
				"@userControl1.Size = System::Drawing::Size.new(10, 10)\r\n" +
				"@userControl1.TabIndex = 0\r\n" +
				"# \r\n" +
				"# MainControl\r\n" +
				"# \r\n" +
				"self.Controls.Add(@userControl1)\r\n" +
				"self.Name = \"MainControl\"\r\n" +
				"self.Size = System::Drawing::Size.new(200, 300)\r\n" +
				"self.ResumeLayout(false)\r\n";
			
			Assert.AreEqual(expectedCode, generatedCode, generatedCode);
		}
		
		protected override IScriptingCodeDomSerializer CreateSerializer()
		{
			return RubyCodeDomSerializerHelper.CreateSerializer();
		}
	}
}
