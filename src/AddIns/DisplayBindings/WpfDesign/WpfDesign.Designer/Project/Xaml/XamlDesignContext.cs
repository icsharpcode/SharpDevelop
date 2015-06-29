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
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using ICSharpCode.WpfDesign.XamlDom;
using ICSharpCode.WpfDesign.Designer.OutlineView;
using ICSharpCode.WpfDesign.Designer.Services;
using ICSharpCode.WpfDesign.Designer.Extensions;
using ICSharpCode.WpfDesign.Extensions;
using ICSharpCode.WpfDesign.PropertyGrid;
using System.Threading;
using System.Globalization;
using ICSharpCode.WpfDesign.Designer.PropertyGrid.Editors;

namespace ICSharpCode.WpfDesign.Designer.Xaml
{
	/// <summary>
	/// The design context implementation used when editing XAML.
	/// </summary>
	public sealed class XamlDesignContext : DesignContext
	{
		readonly XamlDocument _doc;
		readonly XamlDesignItem _rootItem;
		readonly XamlParserSettings _parserSettings;
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
			this.Services.AddService(typeof(IOutlineNodeNameService), new OutlineNodeNameService());
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
			
			EditorManager.SetDefaultTextBoxEditorType(typeof(TextBoxEditor));
			EditorManager.SetDefaultComboBoxEditorType(typeof(ComboBoxEditor));
			
			// register extensions from the designer assemblies:
			foreach (Assembly designerAssembly in loadSettings.DesignerAssemblies) {
				this.Services.ExtensionManager.RegisterAssembly(designerAssembly);
				EditorManager.RegisterAssembly(designerAssembly);
			}
			
			_parserSettings = new XamlParserSettings();
			_parserSettings.TypeFinder = loadSettings.TypeFinder;
			_parserSettings.CreateInstanceCallback = this.Services.ExtensionManager.CreateInstanceWithCustomInstanceFactory;
			_parserSettings.ServiceProvider = this.Services;
			_doc = XamlParser.Parse(xamlReader, _parserSettings);
			
			loadSettings.ReportErrors(xamlErrorService);
			
			if (_doc == null) {
				string message;
				if (xamlErrorService != null && xamlErrorService.Errors.Count > 0)
					message = xamlErrorService.Errors[0].Message;
				else
					message = "Could not load document.";
				throw new XamlLoadException(message);
			}

			_rootItem = _componentService.RegisterXamlComponentRecursive(_doc.RootElement);

			if (_rootItem != null) {
				var rootBehavior = new RootItemBehavior();
				rootBehavior.Intialize(this);
			}

			_xamlEditOperations = new XamlEditOperations(this, _parserSettings);
			
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
		/// Gets the parser Settings being used
		/// </summary>
		public XamlParserSettings ParserSettings {
			get { return _parserSettings; }
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
