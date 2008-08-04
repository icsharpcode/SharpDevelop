// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;

namespace ICSharpCode.WpfDesign.PropertyEditor
{
	sealed class MultipleSelectionDataProperty : IPropertyEditorDataProperty
	{
		readonly MultipleSelectionDataSource ownerDataSource;
		readonly IPropertyEditorDataProperty[] data;
		
		public MultipleSelectionDataProperty(MultipleSelectionDataSource ownerDataSource, IPropertyEditorDataProperty[] data)
		{
			Debug.Assert(ownerDataSource != null);
			Debug.Assert(data != null);
			Debug.Assert(data.Length >= 2);
			
			this.ownerDataSource = ownerDataSource;
			this.data = data;
		}
		
		#region IsSet + IsSetChanged
		EventHandler isSetChangedHandlers;
		bool cachedIsSet;
		
		public event EventHandler IsSetChanged {
			add {
				if (isSetChangedHandlers == null) {
					cachedIsSet = GetIsSetInternal();
					foreach (IPropertyEditorDataProperty p in data) {
						p.IsSetChanged += OnIsSetChanged;
					}
				}
				isSetChangedHandlers += value;
			}
			remove {
				isSetChangedHandlers -= value;
				if (isSetChangedHandlers == null) {
					foreach (IPropertyEditorDataProperty p in data) {
						p.IsSetChanged -= OnIsSetChanged;
					}
				}
			}
		}
		
		void OnIsSetChanged(object sender, EventArgs e)
		{
			bool newIsSet = GetIsSetInternal();
			if (newIsSet != cachedIsSet) {
				cachedIsSet = newIsSet;
				isSetChangedHandlers(this, e);
			}
		}
		
		bool GetIsSetInternal()
		{
			foreach (IPropertyEditorDataProperty p in data) {
				if (p.IsSet)
					return true;
			}
			return false;
		}
		
		public bool IsSet {
			get {
				if (isSetChangedHandlers != null)
					return cachedIsSet;
				else
					return GetIsSetInternal();
			}
			set {
				foreach (IPropertyEditorDataProperty p in data) {
					p.IsSet = value;
				}
			}
		}
		#endregion
		
		#region Value
		EventHandler valueChangedHandlers;
		object cachedValue;
		bool cachedIsAmbiguous;
		/// <summary>0 = don't prevent, 1 = prevent, 2 = prevented</summary>
		byte preventOnValueChanged;
		
		public event EventHandler ValueChanged {
			add {
				if (valueChangedHandlers == null) {
					cachedValue = GetValueInternal();
					cachedIsAmbiguous = GetIsAmbiguousInternal();
					foreach (IPropertyEditorDataProperty p in data) {
						p.ValueChanged += OnValueChanged;
					}
				}
				valueChangedHandlers += value;
			}
			remove {
				valueChangedHandlers -= value;
				if (valueChangedHandlers == null) {
					foreach (IPropertyEditorDataProperty p in data) {
						p.ValueChanged -= OnValueChanged;
					}
				}
			}
		}
		
		void OnValueChanged(object sender, EventArgs e)
		{
			if (preventOnValueChanged > 0) {
				preventOnValueChanged = 2;
				return;
			}
			cachedValue = GetValueInternal();
			cachedIsAmbiguous = GetIsAmbiguousInternal();
			valueChangedHandlers(this, e);
		}
		
		object GetValueInternal()
		{
			object val = data[0].Value;
			for (int i = 1; i < data.Length; i++) {
				if (!object.Equals(data[i].Value, val))
					return null;
			}
			return val;
		}
		
		bool GetIsAmbiguousInternal()
		{
			object val = data[0].Value;
			for (int i = 1; i < data.Length; i++) {
				if (!object.Equals(data[i].Value, val))
					return true;
			}
			return false;
		}
		
		public object Value {
			get {
				if (valueChangedHandlers != null)
					return cachedValue;
				else
					return GetValueInternal();
			}
			set {
				preventOnValueChanged = 1;
				foreach (IPropertyEditorDataProperty p in data) {
					p.Value = value;
				}
				if (preventOnValueChanged == 2) {
					preventOnValueChanged = 0;
					OnValueChanged(null, EventArgs.Empty);
				}
				preventOnValueChanged = 0;
			}
		}
		
		public bool IsAmbiguous {
			get {
				if (valueChangedHandlers != null)
					return cachedIsAmbiguous;
				else
					return GetIsAmbiguousInternal();
			}
		}
		#endregion
		
		public IPropertyEditorDataSource OwnerDataSource {
			get { return ownerDataSource; }
		}
		
		public string Category {
			get { return data[0].Category; }
		}
		
		public string Name {
			get { return data[0].Name; }
		}
		
		public Type ReturnType {
			get { return data[0].ReturnType; }
		}
		
		public Type DeclaringType {
			get { return data[0].DeclaringType; }
		}
		
		public System.ComponentModel.TypeConverter TypeConverter {
			get { return data[0].TypeConverter; }
		}
		
		public object GetDescription()
		{
			return data[0].GetDescription();
		}
		
		public bool CanUseCustomExpression {
			get {
				foreach (IPropertyEditorDataProperty p in data) {
					if (!p.CanUseCustomExpression)
						return false;
				}
				return true;
			}
		}
		
		public void SetCustomExpression(string expression)
		{
			foreach (IPropertyEditorDataProperty p in data) {
				p.SetCustomExpression(expression);
			}
		}
		
		public System.Windows.UIElement CreateEditor()
		{
			EditorManager manager = ownerDataSource.Services.GetService<EditorManager>();
			if (manager != null) {
				return manager.CreateEditor(this);
			} else {
				return new FallbackEditor(this);
			}
		}
	}
}
