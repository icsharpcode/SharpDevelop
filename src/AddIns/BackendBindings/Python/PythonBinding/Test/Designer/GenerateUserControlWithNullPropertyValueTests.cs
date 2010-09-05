// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Designer;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class GenerateUserControlWithNullPropertyValueTests : GenerateUserControlWithNullPropertyValueTestsBase
	{		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = 
				"self._userControl1 = ICSharpCode.Scripting.Tests.Utils.NullPropertyUserControl()\r\n" +
				"self.SuspendLayout()\r\n" +
				"# \r\n" +
				"# userControl1\r\n" +
				"# \r\n" +
				"self._userControl1.FooBar = None\r\n" +
				"self._userControl1.Location = System.Drawing.Point(0, 0)\r\n" +
				"self._userControl1.Name = \"userControl1\"\r\n" +
				"self._userControl1.Size = System.Drawing.Size(10, 10)\r\n" +
				"self._userControl1.TabIndex = 0\r\n" +
				"# \r\n" +
				"# MainControl\r\n" +
				"# \r\n" +
				"self.Controls.Add(self._userControl1)\r\n" +
				"self.Name = \"MainControl\"\r\n" +
				"self.Size = System.Drawing.Size(200, 300)\r\n" +
				"self.ResumeLayout(False)\r\n";
			
			Assert.AreEqual(expectedCode, generatedCode, generatedCode);
		}
		
		protected override IScriptingCodeDomSerializer CreateSerializer()
		{
			return PythonCodeDomSerializerHelper.CreateSerializer();
		}
	}
}
