// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

// enable this define to test that event handlers are removed correctly
//#define EventHandlerDebugging

using System;
using System.Diagnostics;
using System.Windows;
using ICSharpCode.WpfDesign.XamlDom;
using ICSharpCode.WpfDesign.Designer.Services;

namespace ICSharpCode.WpfDesign.Designer.Xaml
{
	sealed class XamlDesignItem : DesignItem
	{
		readonly XamlObject _xamlObject;
		readonly XamlDesignContext _designContext;
		readonly XamlModelPropertyCollection _properties;
		UIElement _view;
		
		public XamlDesignItem(XamlObject xamlObject, XamlDesignContext designContext)
		{
			this._xamlObject = xamlObject;
			this._designContext = designContext;
			this._properties = new XamlModelPropertyCollection(this);
		}
		
		internal XamlComponentService ComponentService {
			get {
				return _designContext._componentService;
			}
		}
		
		internal XamlObject XamlObject {
			get { return _xamlObject; }
		}
		
		public override object Component {
			get {
				return _xamlObject.Instance;
			}
		}
		
		public override string Name {
			get { return null; }
			set { throw new NotImplementedException(); }
		}
		
		#if EventHandlerDebugging
		static int totalEventHandlerCount;
		#endif
		
		/// <summary>
		/// Is raised when the name of the design item changes.
		/// </summary>
		public override event EventHandler NameChanged {
			add {
				#if EventHandlerDebugging
				Debug.WriteLine("Add event handler to " + this.Component.GetType().Name + " (handler count=" + (++totalEventHandlerCount) + ")");
				#endif
			}
			remove {
				#if EventHandlerDebugging
				Debug.WriteLine("Remove event handler from " + this.Component.GetType().Name + " (handler count=" + (--totalEventHandlerCount) + ")");
				#endif
			}
		}
		
		public override DesignItem Parent {
			get {
				if (_xamlObject.ParentProperty == null)
					return null;
				else
					return ComponentService.GetDesignItem(_xamlObject.ParentProperty.ParentObject.Instance);
			}
		}
		
		public override UIElement View {
			get {
				if (_view != null)
					return _view;
				else
					return this.Component as UIElement;
			}
		}
		
		public override ChangeGroup OpenGroup(string changeGroupTitle)
		{
			UndoService undoService = this.Services.GetService<UndoService>();
			if (undoService == null)
				throw new ServiceRequiredException(typeof(UndoService));
			UndoTransaction g = undoService.StartTransaction();
			g.Title = changeGroupTitle;
			return g;
		}
		
		internal void SetView(UIElement newView)
		{
			_view = newView;
		}
		
		public override DesignContext Context {
			get { return _designContext; }
		}
		
		public override DesignItemPropertyCollection Properties {
			get { return _properties; }
		}
	}
}
