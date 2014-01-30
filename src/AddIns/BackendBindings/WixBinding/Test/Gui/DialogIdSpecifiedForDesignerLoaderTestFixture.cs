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
			WixDialogDesignerLoader loader = new WixDialogDesignerLoader(this);
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
			Assert.IsTrue(nameCreationService is XmlDesignerNameCreationService);
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
		
		ICSharpCode.SharpDevelop.Editor.ITextEditor IWixDialogDesigner.PrimaryViewContentTextEditor {
			get {
				throw new NotImplementedException();
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
