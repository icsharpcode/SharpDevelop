// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class FormBaseClassCreatedOnLoadTestFixture : LoadFormTestFixtureBase
	{
		public override string RubyCode {
			get {
				ComponentCreator.AddType("FormBase.FormBase", typeof(Component));
				
				return
					"class TestForm < FormBase::FormBase\r\n" +
					"    def InitializeComponent()\r\n" +
					"    end\r\n" +
					"end";
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
