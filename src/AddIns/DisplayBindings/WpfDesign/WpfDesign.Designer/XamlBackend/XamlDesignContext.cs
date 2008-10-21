using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Xaml;
using System.IO;
using ICSharpCode.WpfDesign.Designer.Services;
using System.ComponentModel;
using System.Windows.Input;

namespace ICSharpCode.WpfDesign.Designer.XamlBackend
{
	public class XamlDesignContext : DesignContext
	{
		public XamlDesignContext(XamlDocument doc)
		{
			this.doc = doc;
			var modelService = new XamlModelService(this);

			AddService(typeof(IModelService), modelService);
			AddService(typeof(IDesignPanel), new DesignPanel() { Context = this });			
			AddService(typeof(ICommandService), new CommandService(this));
			AddService(typeof(ISelectionService), new DefaultSelectionService());			
			AddService(typeof(IToolService), Services.ToolService.Instance);
			AddService(typeof(IUndoService), new UndoService(this));
			AddService(typeof(IErrorService), new DefaultErrorService(this));
			AddService(typeof(IViewService), new ViewService(this));
			AddService(typeof(OptionService), new OptionService());
			AddService(typeof(ITopLevelWindowService), new WpfTopLevelWindowService());

			foreach (var assembly in Metadata.RegisteredAssemblies) {
				ExtensionManager.RegisterAssembly(assembly);
			}

			modelService.Initialize();
		}

		XamlDocument doc;

		public XamlDocument Document
		{
			get { return doc; }
		}	

		public override void Parse(string text)
		{
			doc.Parse(text);
			ParseSuggested = false;
		}

		public override bool CanSave
		{
			get { return doc.CanSave; }
		}

		public override string Save()
		{
			return doc.Save();
		}
	}
}
