// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.DialogLoading
{
	/// <summary>
	/// Creates a dialog from a Wix XML dialog element but uses an IComponentCreator to
	/// create the form and its contained controls.
	/// </summary>
	[TestFixture]
	public class SimpleDialogUsingObjectCreatorTestFixture : DialogLoadingTestFixtureBase
	{				
		CreatedComponent formComponent;
		string formName;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			CreatedComponents.Clear();
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {
				formComponent = CreatedComponents[0];
				formName = dialog.Name;
			}
		}
		
		[Test]
		public void OneComponentCreated()
		{
			Assert.AreEqual(1, CreatedComponents.Count);
		}
		
		[Test]
		public void ComponentName()
		{
			Assert.AreEqual("WelcomeDialog", formComponent.Name);
		}
		
		[Test]
		public void ComponentType()
		{
			Assert.AreEqual("System.Windows.Forms.Form", formComponent.TypeName);
		}
		
		[Test]
		public void Name()
		{
			Assert.AreEqual("WelcomeDialog", formName);
		}
				
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='100' Width='200'/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
