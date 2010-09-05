// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
