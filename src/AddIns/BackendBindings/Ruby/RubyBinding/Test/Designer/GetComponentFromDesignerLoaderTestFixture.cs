// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;
using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests the GetComponent method of the RubyDesignerLoader.
	/// </summary>
	[TestFixture]
	public class GetComponentFromDesignerLoaderTestFixture
	{
		RubyDesignerLoader loader;
		TextBox textBox;
		MockDesignerLoaderHost host;
		Label label;
		
		[SetUp]
		public void Init()
		{
			host = new MockDesignerLoaderHost();
			loader = new RubyDesignerLoader(new MockDesignerGenerator());
			loader.BeginLoad(host);
			
			textBox = (TextBox)loader.CreateComponent(typeof(TextBox), "textBox1");
			label = (Label)loader.CreateComponent(typeof(Label), "label1");
			loader.Add(label, "label1");
		}
		
		[TearDown]
		public void TearDown()
		{
			if (textBox != null) {
				textBox.Dispose();
			}
			if (label != null) {
				label.Dispose();
			}
			if (loader != null) {
				loader.Dispose();
			}
		}
		
		[Test]
		public void LabelAddedToContainer()
		{
			Assert.AreEqual(label, host.Container.Components["label1"]);
		}
		
		[Test]
		public void TextBoxIsNotAddedToContainer()
		{
			Assert.IsNull(host.Container.Components["textBox1"]);
		}		
		
		[Test]
		public void GetUnknownCreatedComponent()
		{
			Assert.IsNull(loader.GetComponent("unknown"));
		}
		
		[Test]
		public void GetTextBoxComponent()
		{
			Assert.IsNull(loader.GetComponent("textBox1"));
		}
	
		[Test]
		public void GetLabelComponent()
		{
			Assert.AreEqual(label, loader.GetComponent("label1"));
		}
	}
}
