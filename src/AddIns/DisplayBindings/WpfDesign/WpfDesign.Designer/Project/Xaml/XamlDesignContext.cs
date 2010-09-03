// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using ICSharpCode.WpfDesign.XamlDom;
using ICSharpCode.WpfDesign.Designer.Services;
using ICSharpCode.WpfDesign.Designer.Extensions;
using ICSharpCode.WpfDesign.Extensions;
using ICSharpCode.WpfDesign.PropertyGrid;
using System.Threading;
using System.Globalization;

namespace ICSharpCode.WpfDesign.Designer.Xaml
{
	/// <summary>
	/// The design context implementation used when editing XAML.
	/// </summary>
	public sealed class XamlDesignContext : DesignContext
	{
		readonly XamlDocument _doc;
		readonly XamlDesignItem _rootItem;
		internal readonly XamlComponentService _componentService;
		
		readonly XamlEditOperations _xamlEditOperations;
		
		public XamlEditOperations XamlEditAction {
			get { return _xamlEditOperations; }
		}
		
		internal XamlDocument Document {
			get { return _doc; }
		}
		
		/// <summary>
		/// Gets/Sets the value of the "x:class" property on the root item.
		/// </summary>
		public string ClassName {
			get { return _doc.RootElement.GetXamlAttribute("Class"); }
			//set { _doc.RootElement.SetXamlAttribute("Class", value); }
		}
		
		/// <summary>
		/// Creates a new XamlDesignContext instance.
		/// </summary>
		public XamlDesignContext(XmlReader xamlReader, XamlLoadSettings loadSettings)
		{
			if (xamlReader == null)
				throw new ArgumentNullException("xamlReader");
			if (loadSettings == null)
				throw new ArgumentNullException("loadSettings");
			
			this.Services.AddService(typeof(ISelectionService), new DefaultSelectionService());
			this.Services.AddService(typeof(IToolService), new DefaultToolService(this));
			this.Services.AddService(typeof(UndoService), new UndoService());
			this.Services.AddService(typeof(IErrorService), new DefaultErrorService(this));
			this.Services.AddService(typeof(ViewService), new DefaultViewService(this));
			this.Services.AddService(typeof(OptionService), new OptionService());

			var xamlErrorService = new XamlErrorService();
			this.Services.AddService(typeof(XamlErrorService), xamlErrorService);
			this.Services.AddService(typeof(IXamlErrorSink), xamlErrorService);
			
			_componentService = new XamlComponentService(this);
			this.Services.AddService(typeof(IComponentService), _componentService);
			
			foreach (Action<XamlDesignContext> action in loadSettings.CustomServiceRegisterFunctions) {
				action(this);
			}
			
			// register default versions of overridable services:
			if (this.Services.GetService(typeof(ITopLevelWindowService)) == null) {
				this.Services.AddService(typeof(ITopLevelWindowService), new WpfTopLevelWindowService());
			}
			
			// register extensions from the designer assemblies:
			foreach (Assembly designerAssembly in loadSettings.DesignerAssemblies) {
				this.Services.ExtensionManager.RegisterAssembly(designerAssembly);
				EditorManager.RegisterAssembly(designerAssembly);
			}
			
			XamlParserSettings parserSettings = new XamlParserSettings();
			parserSettings.TypeFinder = loadSettings.TypeFinder;
			parserSettings.CreateInstanceCallback = this.Services.ExtensionManager.CreateInstanceWithCustomInstanceFactory;
			parserSettings.ServiceProvider = this.Services;
			_doc = XamlParser.Parse(xamlReader, parserSettings);
			if(_doc==null)
				loadSettings.ReportErrors(xamlErrorService);
			
			_rootItem = _componentService.RegisterXamlComponentRecursive(_doc.RootElement);
			
			if(_rootItem!=null){
				var rootBehavior=new RootItemBehavior();
				rootBehavior.Intialize(this);
			}
				
			
			_xamlEditOperations=new XamlEditOperations(this,parserSettings);
		}
		
		
		/// <summary>
		/// Saves the XAML DOM into the XML writer.
		/// </summary>
		public override void Save(System.Xml.XmlWriter writer)
		{
			_doc.Save(writer);
		}
		
		/// <summary>
		/// Gets the root item being designed.
		/// </summary>
		public override DesignItem RootItem {
			get { return _rootItem; }
		}
		
		/// <summary>
		/// Opens a new change group used to batch several changes.
		/// ChangeGroups work as transactions and are used to support the Undo/Redo system.
		/// </summary>
		public override ChangeGroup OpenGroup(string changeGroupTitle, ICollection<DesignItem> affectedItems)
		{
			if (affectedItems == null)
				throw new ArgumentNullException("affectedItems");
			
			UndoService undoService = this.Services.GetRequiredService<UndoService>();
			UndoTransaction g = undoService.StartTransaction(affectedItems);
			g.Title = changeGroupTitle;
			return g;
		}
	}
}
