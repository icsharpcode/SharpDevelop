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

using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.WixBinding
{
	[PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
	[PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
	public class WixDialogDesignerLoader : BasicDesignerLoader, IComponentCreator
	{
		WixDialog wixDialog;
		IWixDialogDesignerGenerator generator;
		IFileLoader fileLoader;
		IWixDialogDesigner designer;
		
		/// <summary>
		/// Creates a new WixDialogDesignerLoader that will load the specified
		/// dialog id from the Wix xml.
		/// </summary>
		public WixDialogDesignerLoader(IWixDialogDesigner designer, IWixDialogDesignerGenerator generator = null, IFileLoader fileLoader = null)
		{
			if (designer == null) {
				throw new ArgumentNullException("designer");
			}
			
			this.designer = designer;
			this.generator = generator ?? new WixDialogDesignerGenerator(designer);
			this.fileLoader = fileLoader ?? new DefaultFileLoader();
		}
		
		public IWixDialogDesigner Designer {
			get { return designer; }
		}
		
		public IWixDialogDesignerGenerator Generator {
			get { return generator; }
		}

		public override void BeginLoad(IDesignerLoaderHost host)
		{
			VerifyDesignerHasDialogId();
			
			GetDialogElement();
			VerifyDialogElementFound();
			
			AddServicesToHost(host);
			
			base.BeginLoad(host);
		}
		
		void VerifyDesignerHasDialogId()
		{
			if (DesignerHasDialogId) {
				ThrowNoDialogSelectedInDocumentException();
			}
		}
		
		bool DesignerHasDialogId {
			get { return designer.DialogId == null; }
		}
		
		void ThrowNoDialogSelectedInDocumentException()
		{
			string message = StringParser.Parse("${res:ICSharpCode.WixBinding.WixDialogDesigner.NoDialogSelectedInDocumentMessage}");
			throw new FormsDesignerLoadException(message);
		}
		
		void GetDialogElement()
		{
			WixDocument document = CreateWixDocument();
			document.LoadXml(designer.GetDocumentXml());
			wixDialog = document.CreateWixDialog(designer.DialogId, new WorkbenchTextFileReader());
		}
		
		WixDocument CreateWixDocument()
		{
			WixDocument document = new WixDocument(designer.Project, fileLoader);
			document.FileName = designer.DocumentFileName;
			return document;
		}		

		void VerifyDialogElementFound()
		{
			if (wixDialog == null) {
				ThrowDialogIdNotFoundException(designer.DialogId);
			}
		}
		
		void ThrowDialogIdNotFoundException(string dialogId)
		{
			string messageFormat = StringParser.Parse("${res:ICSharpCode.WixBinding.DialogDesignerGenerator.DialogIdNotFoundMessage}");
			string message = String.Format(messageFormat, designer.DialogId);
			throw new FormsDesignerLoadException(message);
		}
		
		void AddServicesToHost(IDesignerLoaderHost host)
		{
			host.AddService(typeof(ComponentSerializationService), new CodeDomComponentSerializationService((IServiceProvider)host));
			host.AddService(typeof(INameCreationService), new XmlDesignerNameCreationService(host));
			host.AddService(typeof(IDesignerSerializationService), new DesignerSerializationService(host));
		}
		
		/// <summary>
		/// Creates a component of the specified type.
		/// </summary>
		public IComponent CreateComponent(Type componentClass, string name)
		{
			return base.LoaderHost.CreateComponent(componentClass, name);
		}
		
		/// <summary>
		/// Updates the dialog xml element and then passes this to the generator so the
		/// Wix document is updated.
		/// </summary>
		protected override void PerformFlush(IDesignerSerializationManager serializationManager)
		{
			XmlElement updatedDialogElement = GenerateNewDialogElementFromDesignedForm();
			MergeDialogChangesIntoFullWixDocument(updatedDialogElement);
		}
		
		XmlElement GenerateNewDialogElementFromDesignedForm()
		{
			Form form = (Form)base.LoaderHost.RootComponent;
			return wixDialog.UpdateDialogElement(form);
		}
		
		void MergeDialogChangesIntoFullWixDocument(XmlElement updatedDialogElement)
		{
			generator.MergeFormChanges(designer.DialogId, updatedDialogElement);
		}
		
		protected override void PerformLoad(IDesignerSerializationManager serializationManager)
		{
			wixDialog.CreateDialog(this);
		}
	}
}
