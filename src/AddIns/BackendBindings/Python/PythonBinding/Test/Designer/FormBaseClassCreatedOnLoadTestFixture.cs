// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class FormBaseClassCreatedOnLoadTestFixture : LoadFormTestFixtureBase
	{
		public override string PythonCode {
			get {
				ComponentCreator.AddType("FormBase.FormBase", typeof(Component));
				
				return "class TestForm(FormBase.FormBase):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        pass";
			}
		}
		
		[Test]
		public void BaseClassNamePassedAsGetTypeParam()
		{
			Assert.AreEqual("FormBase.FormBase", ComponentCreator.TypeNames[0]);
		}
		
		[Test]
		public void BaseClassTypePassedToCreateComponent()
		{
			Assert.AreEqual(typeof(Component).FullName, ComponentCreator.CreatedComponents[0].TypeName);
		}	
	}
}
