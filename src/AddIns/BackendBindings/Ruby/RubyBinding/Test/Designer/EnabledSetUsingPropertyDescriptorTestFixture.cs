// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Designer;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// The RubyComponentWalker class should be getting the value from the PropertyDescriptor and not
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
				"self.ClientSize = System::Drawing::Size.new(284, 264)\r\n" +
				"self.Enabled = false\r\n" +
				"self.Name = \"MainForm\"\r\n" +
				"self.ResumeLayout(false)\r\n";
			
			Assert.AreEqual(expectedCode, generatedCode);
		}
		
		protected override IScriptingCodeDomSerializer CreateSerializer()
		{
			return RubyCodeDomSerializerHelper.CreateSerializer();
		}
	}
}
