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

// Turn this on to ensure event handlers on model properties are removed correctly:
//#define EventHandlerDebugging

using System;
using System.Diagnostics;
using ICSharpCode.WpfDesign.XamlDom;
using ICSharpCode.WpfDesign.Designer.Services;
using System.Windows;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.Designer.Xaml
{
	[DebuggerDisplay("XamlModelProperty: {Name}")]
	sealed class XamlModelProperty : DesignItemProperty, IEquatable<XamlModelProperty>
	{
		readonly XamlDesignItem _designItem;
		readonly XamlProperty _property;
		readonly XamlModelCollectionElementsCollection _collectionElements;

		internal XamlDesignItem XamlDesignItem {
			get { return _designItem; }
		}
		
		public XamlModelProperty(XamlDesignItem designItem, XamlProperty property)
		{
			Debug.Assert(designItem != null);
			Debug.Assert(property != null);
			
			this._designItem = designItem;
			this._property = property;
			if (property.IsCollection) {
				_collectionElements = new XamlModelCollectionElementsCollection(this, property);
			}
		}
		
		public override bool Equals(object obj)
		{
			return Equals(obj as XamlModelProperty);
		}
		
		public bool Equals(XamlModelProperty other)
		{
			if (other == null)
				return false;
			else
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
		
		public override bool IsEvent {
			get { return _property.IsEvent; }
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
				if (!IsCollection)
					throw new DesignerException("Cannot access CollectionElements for non-collection properties.");
				else
					return _collectionElements;
			}
		}
		
		public override DesignItem Value {
			get {
				// Binding...
				//if (IsCollection)
				//	throw new DesignerException("Cannot access Value for collection properties.");

				var xamlObject = _property.PropertyValue as XamlObject;
				if (xamlObject != null) {
					return _designItem.ComponentService.GetDesignItem(xamlObject.Instance);
				}

				return null;
			}
		}
		
		public override string TextValue
		{
			get {
				var xamlTextValue = _property.PropertyValue as XamlTextValue;
				if (xamlTextValue != null) {
					return xamlTextValue.Text;
				}

				return null;
			}
		}

		internal void SetValueOnInstance(object value)
		{
			_property.ValueOnInstance = value;
		}

		// There may be multiple XamlModelProperty instances for the same property,
		// so this class may not have any mutable fields / events - instead,
		// we forward all event handlers to the XamlProperty.
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

		public override event EventHandler ValueOnInstanceChanged {
			add { _property.ValueOnInstanceChanged += value; }
			remove { _property.ValueOnInstanceChanged -= value; }
		}
		
		public override object ValueOnInstance {
			get { return _property.ValueOnInstance; }
			//set { _property.ValueOnInstance = value; }
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
			XamlPropertyValue newValue;
			if (value == null) {
				newValue = _property.ParentObject.OwnerDocument.CreateNullValue();
			} else {
				XamlComponentService componentService = _designItem.ComponentService;

				XamlDesignItem designItem = value as XamlDesignItem;
				if (designItem == null) designItem = (XamlDesignItem)componentService.GetDesignItem(value);
				if (designItem != null) {
					if (designItem.Parent != null)
						throw new DesignerException("Cannot set value to design item that already has a parent");
					newValue = designItem.XamlObject;
				} else {
					XamlPropertyValue val = _property.ParentObject.OwnerDocument.CreatePropertyValue(value, _property);
					designItem = componentService.RegisterXamlComponentRecursive(val as XamlObject);
					newValue = val;
				}
			}
			
			UndoService undoService = _designItem.Services.GetService<UndoService>();
			if (undoService != null)
				undoService.Execute(new PropertyChangeAction(this, newValue, true));
			else
				SetValueInternal(newValue);
		}
		
		void SetValueInternal(XamlPropertyValue newValue)
		{
			_property.PropertyValue = newValue;
			_designItem.NotifyPropertyChanged(this);
		}
		
		public override void Reset()
		{
			UndoService undoService = _designItem.Services.GetService<UndoService>();
			if (undoService != null)
				undoService.Execute(new PropertyChangeAction(this, null, false));
			else
				ResetInternal();
		}
		
		void ResetInternal()
		{
			if (_property.IsSet) {
				_property.Reset();
				_designItem.NotifyPropertyChanged(this);
			}
		}
		
		public sealed class PropertyChangeAction : ITransactionItem
		{
			readonly XamlModelProperty property;
			readonly XamlPropertyValue oldValue;
			readonly object oldValueOnInstance;
			XamlPropertyValue newValue;
			readonly bool oldIsSet;
			bool newIsSet;
			readonly ITransactionItem collectionTransactionItem;
			
			public PropertyChangeAction(XamlModelProperty property, XamlPropertyValue newValue, bool newIsSet)
			{
				this.property = property;
				this.newValue = newValue;
				this.newIsSet = newIsSet;
				
				oldIsSet = property._property.IsSet;
				oldValue = property._property.PropertyValue;
				oldValueOnInstance = property._property.ValueOnInstance;
				
				if (oldIsSet && oldValue == null && property.IsCollection) {
					collectionTransactionItem = property._collectionElements.CreateResetTransaction();
				}
			}
			
			public string Title {
				get {
					if (newIsSet)
						return "Set " + property.Name;
					else
						return "Reset " + property.Name;
				}
			}
			
			public void Do()
			{
				if (collectionTransactionItem != null) {
					collectionTransactionItem.Do();
				}
				
				if (newIsSet)
					property.SetValueInternal(newValue);
				else
					property.ResetInternal();
			}
			
			public void Undo()
			{
				if (oldIsSet) {
					if (collectionTransactionItem != null) {
						collectionTransactionItem.Undo();
					} else {
						property.SetValueInternal(oldValue);
					}
				}
				else {
					if (property.DependencyProperty == null) {
						try
						{							
							property.SetValueOnInstance(oldValueOnInstance);
						}
						catch(Exception)
						{ }
					}

					property.ResetInternal();
				}
			}
			
			public System.Collections.Generic.ICollection<DesignItem> AffectedElements {
				get {
					return new DesignItem[] { property._designItem };
				}
			}

			public bool MergeWith(ITransactionItem other)
			{
				PropertyChangeAction o = other as PropertyChangeAction;
				if (o != null && property._property == o.property._property) {
					newIsSet = o.newIsSet;
					newValue = o.newValue;
					return true;
				}
				return false;
			}
		}

		public override DesignItem DesignItem {
			get { return _designItem; }
		}

		public override DependencyProperty DependencyProperty {
			get { return _property.DependencyProperty; }
		}

		public override bool IsAdvanced {
			get { return _property.IsAdvanced; }
		}
	}
}
