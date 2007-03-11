// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.WpfDesign.PropertyEditor
{
	/// <summary>
	/// Implements IPropertyEditorDataProperty by forwarding all calls to another IPropertyEditorDataProperty.
	/// </summary>
	public abstract class ProxyPropertyEditorDataProperty : IPropertyEditorDataProperty
	{
		readonly IPropertyEditorDataProperty data;
		
		/// <summary></summary>
		protected ProxyPropertyEditorDataProperty(IPropertyEditorDataProperty data)
		{
			if (data == null)
				throw new ArgumentNullException("data");
			this.data = data;
		}
		
		/// <summary>See <see cref="IPropertyEditorDataProperty"/></summary>
		public virtual event EventHandler IsSetChanged {
			add    { data.IsSetChanged += value; }
			remove { data.IsSetChanged -= value; }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataProperty"/></summary>
		public virtual event EventHandler ValueChanged {
			add    { data.ValueChanged += value; }
			remove { data.ValueChanged -= value; }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataProperty"/></summary>
		public virtual IPropertyEditorDataSource OwnerDataSource {
			get { return data.OwnerDataSource; }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataProperty"/></summary>
		public virtual string Category {
			get { return data.Category; }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataProperty"/></summary>
		public virtual string Name {
			get { return data.Name; }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataProperty"/></summary>
		public virtual Type ReturnType {
			get { return data.ReturnType; }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataProperty"/></summary>
		public virtual Type DeclaringType {
			get { return data.DeclaringType; }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataProperty"/></summary>
		public virtual System.ComponentModel.TypeConverter TypeConverter {
			get { return data.TypeConverter; }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataProperty"/></summary>
		public virtual bool IsSet {
			get { return data.IsSet; }
			set { data.IsSet = value; }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataProperty"/></summary>
		public virtual bool IsAmbiguous {
			get { return data.IsAmbiguous; }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataProperty"/></summary>
		public virtual object Value {
			get { return data.Value; }
			set { data.Value = value; }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataProperty"/></summary>
		public virtual bool CanUseCustomExpression {
			get { return data.CanUseCustomExpression; }
		}
		
		/// <summary>See <see cref="IPropertyEditorDataProperty"/></summary>
		public virtual object GetDescription()
		{
			return data.GetDescription();
		}
		
		/// <summary>See <see cref="IPropertyEditorDataProperty"/></summary>
		public virtual void SetCustomExpression(string expression)
		{
			data.SetCustomExpression(expression);
		}
		
		/// <summary>See <see cref="IPropertyEditorDataProperty"/></summary>
		public virtual System.Windows.UIElement CreateEditor()
		{
			return data.CreateEditor();
		}
	}
}
