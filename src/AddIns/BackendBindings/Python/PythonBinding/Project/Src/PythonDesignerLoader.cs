// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.IO;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.FormsDesigner;
using IronPython.CodeDom;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Loads the form or control's code so the forms designer can
	/// display it.
	/// </summary>
	public class PythonDesignerLoader : CodeDomDesignerLoader
	{
		IDocument document;
		IDesignerGenerator generator;
		bool loading;
		ITypeResolutionService typeResolutionService;
		PythonProvider codeDomProvider = new PythonProvider();
		IDesignerLoaderHost designerLoaderHost;
		
		public PythonDesignerLoader(IDocument document, IDesignerGenerator generator)
		{
			if (document == null) {
				throw new ArgumentException("Document cannot be null", "document");
			}
			if (generator == null) {
				throw new ArgumentException("Generator cannot be null", "generator");
			}
			
			this.document = document;
			this.generator = generator;
		}

		public override void BeginLoad(IDesignerLoaderHost host)
		{
			loading = true;
			designerLoaderHost = host;
			typeResolutionService = (ITypeResolutionService)host.GetService(typeof(ITypeResolutionService));
			base.BeginLoad(host);
		}
		
		public override bool Loading {
			get { return loading; }
		}
		
		protected override CodeDomProvider CodeDomProvider {
			get { return codeDomProvider; }
		}
		
		protected override ITypeResolutionService TypeResolutionService {
			get { return typeResolutionService; }
		}
		
		protected override void OnEndLoad(bool successful, System.Collections.ICollection errors)
		{
			base.OnEndLoad(successful, errors);
			loading = false;
		}

		/// <summary>
		/// Gets the form or user control's code from the document and 
		/// parses it using the PythonProvider class.
		/// </summary>
		protected override CodeCompileUnit Parse()
		{
			return PythonDesignerCodeDomGenerator.Parse(document.TextContent);
		}
		
		protected override void Write(CodeCompileUnit unit)
		{
			generator.MergeFormChanges(unit);
		}
		
		protected override void Initialize()
		{
			CodeDomLocalizationProvider localizationProvider = new CodeDomLocalizationProvider(designerLoaderHost, CodeDomLocalizationModel.PropertyAssignment);
			IDesignerSerializationManager manager = (IDesignerSerializationManager)designerLoaderHost.GetService(typeof(IDesignerSerializationManager));
			manager.AddSerializationProvider(localizationProvider);
			base.Initialize();
		}	
	}
}
