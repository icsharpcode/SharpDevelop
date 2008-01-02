// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.FormsDesigner;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Gui
{
	/// <summary>
	/// Tests that a WixDialogDesignerLoader uses the dialog id to find the dialog 
	/// XML element. 
	/// </summary>
	[TestFixture]
	public class DialogIdSpecifiedForDesignerLoaderTestFixture : IWixDialogDesigner
	{
		Form formCreated;
		MockDesignerLoaderHost loaderHost;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDialogDesignerLoader loader = new WixDialogDesignerLoader(this, new WixDialogDesignerGenerator());
			loaderHost = new MockDesignerLoaderHost();
			loader.BeginLoad(loaderHost);
			IComponent rootComponent = loaderHost.RootComponent;
			formCreated = (Form)rootComponent;
		}
		
		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			if (formCreated != null) {
				formCreated.Dispose();
			}
		}
		
		[Test]
		public void FormName()
		{
			Assert.AreEqual("WelcomeDialog", formCreated.Name);
		}
		
		[Test]
		public void OneComponentCreated()
		{
			Assert.AreEqual(1, loaderHost.CreatedComponents.Count);
		}
		
		[Test]
		public void NameCreationServiceCreated()
		{
			INameCreationService nameCreationService = (INameCreationService)loaderHost.GetService(typeof(INameCreationService));
			Assert.IsTrue(nameCreationService is XmlDesignerLoader.NameCreationService);
		}
		
		[Test]
		public void ComponentSerializationServiceCreated()
		{
			ComponentSerializationService serializationService = (ComponentSerializationService)loaderHost.GetService(typeof(ComponentSerializationService));
			Assert.IsTrue(serializationService is ComponentSerializationService);
		}
		
		[Test]
		public void DesignerSerializationServiceCreated()
		{
			IDesignerSerializationService designerSerializationServiceCreated = (IDesignerSerializationService)loaderHost.GetService(typeof(IDesignerSerializationService));
			Assert.IsTrue(designerSerializationServiceCreated is DesignerSerializationService);
		}
		
		string IWixDialogDesigner.DialogId {
			get {
				return "WelcomeDialog";
			}
		}
		
		string IWixDialogDesigner.GetDocumentXml()
		{
			return GetWixXml();
		}
		
		public string DocumentFileName {
			get {
				return String.Empty;
			}
		}
		
		public WixProject Project {
			get {
				return WixBindingTestsHelper.CreateEmptyWixProject();
			}
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id=\"WelcomeDialog\" Height=\"100\" Width=\"200\"/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
