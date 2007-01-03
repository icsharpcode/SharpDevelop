// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

// Turn this on to ensure event handlers on model properties are removed correctly:
#define EventHandlerDebugging

using System;
using System.Diagnostics;
using ICSharpCode.WpfDesign.XamlDom;

namespace ICSharpCode.WpfDesign.Designer.Xaml
{
	sealed class XamlModelProperty : DesignItemProperty, IEquatable<XamlModelProperty>
	{
		readonly XamlDesignItem _designItem;
		readonly XamlProperty _property;
		
		public XamlModelProperty(XamlDesignItem designItem, XamlProperty property)
		{
			Debug.Assert(designItem != null);
			Debug.Assert(property != null);
			
			this._designItem = designItem;
			this._property = property;
		}
		
		public override bool Equals(object obj)
		{
			return Equals(obj as XamlModelProperty);
		}
		
		public bool Equals(XamlModelProperty other)
		{
			return this._designItem == other._designItem && this._property == other._property;
		}
		
		public override int GetHashCode()
		{
			return _designItem.GetHashCode() ^ _property.GetHashCode();
		}
		
		public override string Name {
			get { return _property.PropertyName; }
		}
		
		public override bool IsCollection {
			get { return _property.IsCollection; }
		}
		
		public override Type ReturnType {
			get { return _property.ReturnType; }
		}
		
		public override Type DeclaringType {
			get { return _property.PropertyTargetType; }
		}
		
		public override string Category {
			get { return _property.Category; }
		}
		
		public override System.ComponentModel.TypeConverter TypeConverter {
			get { return _property.TypeConverter; }
		}
		
		public override System.Collections.Generic.IList<DesignItem> CollectionElements {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override DesignItem Value {
			get {
				if (IsCollection)
					throw new DesignerException("Cannot access Value for collection properties.");
				
				XamlComponentService componentService = _designItem.ComponentService;
				object valueOnInstance = this.ValueOnInstance;
				DesignItem designItem = componentService.GetDesignItem(valueOnInstance);
				if (designItem != null)
					return designItem;
				
				return componentService.RegisterComponentForDesigner(valueOnInstance);
			}
		}
		
		public override event EventHandler ValueChanged {
			add {
				#if EventHandlerDebugging
				if (ValueChangedEventHandlers == 0) {
					Debug.WriteLine("ValueChangedEventHandlers is now > 0");
				}
				ValueChangedEventHandlers++;
				#endif
				_property.ValueChanged += value;
			}
			remove {
				#if EventHandlerDebugging
				ValueChangedEventHandlers--;
				if (ValueChangedEventHandlers == 0) {
					Debug.WriteLine("ValueChangedEventHandlers reached 0");
				}
				#endif
				_property.ValueChanged -= value;
			}
		}
		
		public override object ValueOnInstance {
			get { return _property.ValueOnInstance; }
			set { _property.ValueOnInstance = value; }
		}
		
		public override bool IsSet {
			get { return _property.IsSet; }
		}
		
		#if EventHandlerDebugging
		static int IsSetChangedEventHandlers, ValueChangedEventHandlers;
		#endif
		
		public override event EventHandler IsSetChanged {
			add {
				#if EventHandlerDebugging
				if (IsSetChangedEventHandlers == 0) {
					Debug.WriteLine("IsSetChangedEventHandlers is now > 0");
				}
				IsSetChangedEventHandlers++;
				#endif
				_property.IsSetChanged += value;
			}
			remove {
				#if EventHandlerDebugging
				IsSetChangedEventHandlers--;
				if (IsSetChangedEventHandlers == 0) {
					Debug.WriteLine("IsSetChangedEventHandlers reached 0");
				}
				#endif
				_property.IsSetChanged -= value;
			}
		}
		
		public override void SetValue(object value)
		{
			_property.ValueOnInstance = value;
			
			XamlComponentService componentService = _designItem.ComponentService;
			
			XamlDesignItem designItem = (XamlDesignItem)componentService.GetDesignItem(value);
			if (designItem != null) {
				if (designItem.Parent != null)
					throw new DesignerException("Cannot set value to design item that already has a parent");
				_property.PropertyValue = designItem.XamlObject;
			} else {
				XamlPropertyValue val = _property.ParentObject.OwnerDocument.CreatePropertyValue(value, _property);
				designItem = componentService.RegisterXamlComponentRecursive(val as XamlObject);
				_property.PropertyValue = val;
			}
		}
		
		public override void Reset()
		{
			_property.Reset();
		}
	}
}
