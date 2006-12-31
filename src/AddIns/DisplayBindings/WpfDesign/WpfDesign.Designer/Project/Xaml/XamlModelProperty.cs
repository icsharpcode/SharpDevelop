// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.WpfDesign.XamlDom;

namespace ICSharpCode.WpfDesign.Designer.Xaml
{
	sealed class XamlModelProperty : DesignItemProperty
	{
		readonly XamlDesignItem _designItem;
		readonly XamlProperty _property;
		
		public XamlModelProperty(XamlDesignItem designItem, XamlProperty property)
		{
			this._designItem = designItem;
			this._property = property;
		}
		
		public override bool IsCollection {
			get {
				return _property.IsCollection;
			}
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
		
		public override object ValueOnInstance {
			get {
				return _property.ValueOnInstance;
			}
			set {
				_property.ValueOnInstance = value;
			}
		}
		
		public override bool IsSet {
			get {
				return _property.IsSet;
			}
		}
		
		public override void SetValue(object value)
		{
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
			
			_property.ValueOnInstance = value;
		}
		
		public override void Reset()
		{
			_property.Reset();
		}
	}
}
