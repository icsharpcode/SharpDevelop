// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Windows;

namespace ICSharpCode.WpfDesign.PropertyEditor
{
	/// <summary>
	/// wraps a DesignItemDataProperty for the property editor/grid.
	/// </summary>
	sealed class DesignItemDataProperty : IPropertyEditorDataProperty
	{
		readonly DesignItemDataSource ownerDataSource;
		readonly DesignItemProperty property;
		
		internal DesignItemDataProperty(DesignItemDataSource ownerDataSource, DesignItemProperty property)
		{
			Debug.Assert(ownerDataSource != null);
			Debug.Assert(property != null);
			
			this.ownerDataSource = ownerDataSource;
			this.property = property;
		}
		
		public IPropertyEditorDataSource OwnerDataSource {
			get { return ownerDataSource; }
		}
		
		public string Category {
			get { return "Misc"; }
		}
		
		public string Name {
			get { return property.Name; }
		}
		
		public string Description {
			get { return "Description for " + property.Name; }
		}
		
		public System.ComponentModel.TypeConverter TypeConverter {
			get { return property.TypeConverter; }
		}
		
		public bool IsSet {
			get {
				return property.IsSet;
			}
			set {
				if (value != property.IsSet) {
					if (value) {
						// copy default value to local value
						property.SetValue(property.ValueOnInstance);
					} else {
						property.Reset();
					}
				}
			}
		}
		
		public event EventHandler IsSetChanged {
			add    { property.IsSetChanged += value; }
			remove { property.IsSetChanged -= value; }
		}
		
		public object Value {
			get {
				return property.ValueOnInstance;
			}
			set {
				property.SetValue(value);
			}
		}
		
		public event EventHandler ValueChanged {
			add { property.ValueChanged += value; }
			remove { property.ValueChanged -= value; }
		}
		
		/// <summary>
		/// Gets the type of the property value.
		/// </summary>
		public Type ReturnType {
			get { return property.ReturnType; }
		}
		
		/// <summary>
		/// Gets the type that declares the property.
		/// </summary>
		public Type DeclaringType {
			get { return property.DeclaringType; }
		}
		
		public bool CanUseCustomExpression {
			get {
				return true;
			}
		}
		
		public void SetCustomExpression(string expression)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Creates a UIElement that can edit the property value.
		/// </summary>
		public UIElement CreateEditor()
		{
			EditorManager manager = ownerDataSource.DesignItem.Services.GetService<EditorManager>();
			if (manager != null) {
				return manager.CreateEditor(this);
			} else {
				return new FallbackEditor(this);
			}
		}
	}
}
