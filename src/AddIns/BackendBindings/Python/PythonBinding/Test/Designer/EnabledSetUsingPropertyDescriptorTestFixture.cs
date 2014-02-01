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
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Designer;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// The PythonForm class should be getting the value from the PropertyDescriptor and not
	/// the PropertyInfo information returned from the form object. If this is not done then when the
	/// user sets Enabled to false in the designer the value is not generated in the InitializeComponent method.
	/// </summary>
	[TestFixture]
	public class EnabledSetUsingPropertyDescriptorTestFixture : GenerateEnabledUsingPropertyDescriptorTestsBase
	{		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = 
				"self.SuspendLayout()\r\n" +
				"# \r\n" +
				"# MainForm\r\n" +
				"# \r\n" +
				"self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
				"self.Enabled = False\r\n" +
				"self.Name = \"MainForm\"\r\n" +
				"self.ResumeLayout(False)\r\n";
			
			Assert.AreEqual(expectedCode, generatedCode);
		}
		
		protected override IScriptingCodeDomSerializer CreateSerializer()
		{
			return PythonCodeDomSerializerHelper.CreateSerializer();
		}
	}
}
