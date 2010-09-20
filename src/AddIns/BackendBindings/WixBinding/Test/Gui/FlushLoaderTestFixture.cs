// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.FormsDesigner;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Gui
{
	/// <summary>
	/// Tests that a WixDialogDesignerLoader calls the IDesignerGenerator.MergeFormChanges
	/// method after being flushed. 
	/// </summary>
	[TestFixture]
	public class FlushLoaderTestFixture : IWixDialogDesignerGenerator, IWixDialogDesigner
	{				
		class DerivedWixDialogDesignerLoader : WixDialogDesignerLoader
		{
			public DerivedWixDialogDesignerLoader(IWixDialogDesigner designer, IWixDialogDesignerGenerator generator)
				: base(designer, generator)
			{
			}
			
			public void CallPerformFlush()
			{
				base.PerformFlush(null);
			}
		}

		string newDialogTitle;
		string dialogId;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			newDialogTitle = String.Empty;
			dialogId = String.Empty;
			
			DerivedWixDialogDesignerLoader loader = new DerivedWixDialogDesignerLoader(this, this);
			MockDesignerLoaderHost loaderHost = new MockDesignerLoaderHost();
			loader.BeginLoad(loaderHost);
			using (Form form = (Form)loaderHost.RootComponent) {
				form.Text = "NewTitle";		
				loader.CallPerformFlush();
			}
		}
		
		[Test]
		public void MergeFormChangesCalled()
		{
			Assert.AreEqual("NewTitle", newDialogTitle);
		}
		
		[Test]
		public void DialogId()
		{
			Assert.AreEqual("WelcomeDialog", dialogId);
		}
		
		string IWixDialogDesigner.DialogId {
			get { return "WelcomeDialog"; }
		}
		
		string IWixDialogDesigner.GetDocumentXml()
		{
			return GetWixXml();
		}
		
		public string DocumentFileName {
			get { return String.Empty; }
		}
		
		public WixProject Project {
			get { return WixBindingTestsHelper.CreateEmptyWixProject(); }
		}
		
		string GetWixXml()
		{
			return 
				"<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id=\"WelcomeDialog\" Height=\"100\" Width=\"200\"/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
		
		public void MergeFormChanges(string dialogId, XmlElement dialogElement)
		{
			this.dialogId = dialogId;
			newDialogTitle = dialogElement.GetAttribute("Title");
		}
	}
}
