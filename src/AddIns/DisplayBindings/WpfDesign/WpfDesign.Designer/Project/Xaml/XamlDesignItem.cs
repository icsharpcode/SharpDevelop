// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

// enable this define to test that event handlers are removed correctly
//#define EventHandlerDebugging

using System;
using System.Diagnostics;
using System.Windows;
using ICSharpCode.WpfDesign.XamlDom;
using ICSharpCode.WpfDesign.Designer.Services;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.Designer.Xaml
{
	[DebuggerDisplay("XamlDesignItem: {ComponentType.Name}")]
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
		
		public override Type ComponentType {
			get { return _xamlObject.ElementType; }
		}
		
		public override string Name {
			get { return (string)this.Properties["Name"].ValueOnInstance; }
			set { this.Properties["Name"].SetValue(value); }
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
				Debug.WriteLine("Add event handler to " + this.ComponentType.Name + " (handler count=" + (++totalEventHandlerCount) + ")");
				#endif
				this.Properties["Name"].ValueChanged += value;
			}
			remove {
				#if EventHandlerDebugging
				Debug.WriteLine("Remove event handler from " + this.ComponentType.Name + " (handler count=" + (--totalEventHandlerCount) + ")");
				#endif
				this.Properties["Name"].ValueChanged -= value;
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
		
		public override DesignItemProperty ParentProperty {
			get {
				DesignItem parent = this.Parent;
				if (parent == null)
					return null;
				XamlProperty prop = _xamlObject.ParentProperty;
				if (prop.IsAttached) {
					return parent.Properties.GetAttachedProperty(prop.PropertyTargetType, prop.PropertyName);
				} else {
					return parent.Properties.GetProperty(prop.PropertyName);
				}
			}
		}
		
		/// <summary>
		/// Occurs when the parent of this design item changes.
		/// </summary>
		public override event EventHandler ParentChanged {
			add    { _xamlObject.ParentPropertyChanged += value; }
			remove { _xamlObject.ParentPropertyChanged += value; }
		}
		
		public override UIElement View {
			get {
				if (_view != null)
					return _view;
				else
					return this.Component as UIElement;
			}
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
		
		internal void NotifyPropertyChanged(XamlModelProperty property)
		{
			Debug.Assert(property != null);
			OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(property.Name));
		}

		public override string ContentPropertyName {
			get {
				return XamlObject.ContentPropertyName; 
			}
		}
		
		public override DesignItem Clone()
		{
			throw new NotImplementedException();
		}
	}
}
