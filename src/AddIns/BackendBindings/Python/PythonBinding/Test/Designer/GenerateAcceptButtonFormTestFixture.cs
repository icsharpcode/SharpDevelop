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
	public class GenerateAcceptButtonFormTestFixture : GenerateAcceptButtonFormTestsBase
	{
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = 
				"self._button1 = System.Windows.Forms.Button()\r\n" +
				"self.SuspendLayout()\r\n" +
				"# \r\n" +
				"# button1\r\n" +
				"# \r\n" +
				"self._button1.Location = System.Drawing.Point(0, 0)\r\n" +
				"self._button1.Name = \"button1\"\r\n" +
				"self._button1.Size = System.Drawing.Size(10, 10)\r\n" +
				"self._button1.TabIndex = 0\r\n" +
				"self._button1.Text = \"button1\"\r\n" +
				"# \r\n" +
				"# MainForm\r\n" +
				"# \r\n" +
				"self.AcceptButton = self._button1\r\n" +
				"self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
				"self.Controls.Add(self._button1)\r\n" +
				"self.Name = \"MainForm\"\r\n" +
				"self.ResumeLayout(False)\r\n";
			
			Assert.AreEqual(expectedCode, generatedCode, generatedCode);
		}
		
		protected override IScriptingCodeDomSerializer CreateSerializer()
		{
			return PythonCodeDomSerializerHelper.CreateSerializer();
		}
	}
}
