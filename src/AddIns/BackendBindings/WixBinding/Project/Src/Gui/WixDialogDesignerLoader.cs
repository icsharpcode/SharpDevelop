// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Security.Permissions;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner;
using ICSharpCode.FormsDesigner.Services;

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
				
		public WixDialogDesignerLoader(IWixDialogDesigner designer, IWixDialogDesignerGenerator generator)
			: this(designer, generator, null)
		{
		}
		
		/// <summary>
		/// Creates a new WixDialogDesignerLoader that will load the specified
		/// dialog id from the Wix xml.
		/// </summary>
		public WixDialogDesignerLoader(IWixDialogDesigner designer, IWixDialogDesignerGenerator generator, IFileLoader fileLoader)
		{
			this.designer = designer;
			this.generator = generator;
			this.fileLoader = fileLoader;
			
			if (designer == null) {
				throw new ArgumentException("Cannot be null.", "designer");
			}
			if (generator == null) {
				throw new ArgumentException("Cannot be null.", "generator");
			}
		}
		
		/// <summary>
		/// Gets the designer used by the loader.
		/// </summary>
		public IWixDialogDesigner Designer {
			get {
				return designer;
			}
		}
		
		/// <summary>
		/// Gets the designer generator used by the loader.
		/// </summary>
		public IWixDialogDesignerGenerator Generator {
			get {
				return generator;
			}
		}

		public override void BeginLoad(IDesignerLoaderHost host)
		{
			// Check dialog id.
			if (designer.DialogId == null) {
				throw new FormsDesignerLoadException(StringParser.Parse("${res:ICSharpCode.WixBinding.WixDialogDesigner.NoDialogSelectedInDocumentMessage}"));
			}
			
			// Get dialog element.
			WixDocument document = CreateWixDocument();
			document.LoadXml(designer.GetDocumentXml());
			wixDialog = document.GetDialog(designer.DialogId, new WorkbenchTextFileReader());
			if (wixDialog == null) {
				throw new FormsDesignerLoadException(String.Format(StringParser.Parse("${res:ICSharpCode.WixBinding.DialogDesignerGenerator.DialogIdNotFoundMessage}"), designer.DialogId));
			}
			
			host.AddService(typeof(ComponentSerializationService), new CodeDomComponentSerializationService((IServiceProvider)host));
			host.AddService(typeof(INameCreationService), new XmlDesignerLoader.NameCreationService(host));
			host.AddService(typeof(IDesignerSerializationService), new DesignerSerializationService(host));

			base.BeginLoad(host);
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
			Form dialog = (Form)base.LoaderHost.RootComponent;
			generator.MergeFormChanges(designer.DialogId, wixDialog.UpdateDialogElement(dialog));
		}
		
		protected override void PerformLoad(IDesignerSerializationManager serializationManager)
		{
			wixDialog.CreateDialog(this);
		}
		
		WixDocument CreateWixDocument()
		{
			WixDocument document;
			
			if (fileLoader != null && designer != null) {
				document = new WixDocument(designer.Project, fileLoader);
			} else if (designer != null) {
				document = new WixDocument(designer.Project);
			} else {
				document = new WixDocument();
			}
			document.FileName = designer.DocumentFileName;
			return document;
		}
	}
}
