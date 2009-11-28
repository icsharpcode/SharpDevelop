// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;

using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class WixBindingTestsHelperTests
	{
		WixProject project;
		
		[SetUp]
		public void Init()
		{
			project = WixBindingTestsHelper.CreateEmptyWixProject();
		}
		
		[Test]
		public void CanGetEditorAttributeFromCollection()
		{
			BindableAttribute bindableAttribute = new BindableAttribute(false);
			EditorAttribute editorAttribute = new EditorAttribute();
			
			Attribute[] attributes = new Attribute[] { bindableAttribute, editorAttribute };
			AttributeCollection attributeCollection = new AttributeCollection(attributes);
			
			Assert.AreSame(editorAttribute, WixBindingTestsHelper.GetEditorAttribute(attributeCollection));
		}
		
		[Test]
		public void WixProjectNameIsTest()
		{
			Assert.AreEqual("Test", project.Name);
		}
		
		[Test]
		public void WixProjectHasFileName()
		{
			Assert.AreEqual(@"C:\Projects\Test\Test.wixproj", project.FileName);
		}
	}
}
