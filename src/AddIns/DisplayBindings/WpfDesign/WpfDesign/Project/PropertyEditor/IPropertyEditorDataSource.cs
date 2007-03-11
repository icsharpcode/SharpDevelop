// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;

namespace ICSharpCode.WpfDesign.PropertyEditor
{
	/// <summary>
	/// Interface for data sources used by the property editor to display the list of properties.
	/// </summary>
	public interface IPropertyEditorDataSource
	{
		/// <summary>
		/// Gets/Sets the name of the item. Returns null when the item does not support having a name.
		/// </summary>
		string Name { get; set; }
		
		/// <summary>
		/// Is raised whenever the Name property changes value.
		/// </summary>
		event EventHandler NameChanged;
		
		/// <summary>
		/// Gets the type of the item (for display only).
		/// </summary>
		string Type { get; }
		
		/// <summary>
		/// Gets the icon of the item (for display only)
		/// </summary>
		ImageSource Icon { get; }
		
		/// <summary>
		/// Gets the collection of properties.
		/// </summary>
		ICollection<IPropertyEditorDataProperty> Properties { get; }
		
		/// <summary>
		/// Gets if adding attached properties is supported.
		/// </summary>
		bool CanAddAttachedProperties { get; }
		
		/// <summary>
		/// Gets the service container attached to this data source.
		/// </summary>
		ServiceContainer Services { get; }
		
		/// <summary>
		/// Gets a brush used as a preview for the data source.
		/// </summary>
		Brush CreateThumbnailBrush();
	}
	
	/// <summary>
	/// Represents a property inside a <see cref="IPropertyEditorDataSource"/>.
	/// </summary>
	public interface IPropertyEditorDataProperty
	{
		/// <summary>
		/// Gets the data source that own this property.
		/// </summary>
		IPropertyEditorDataSource OwnerDataSource { get; }
		
		/// <summary>
		/// Gets the category this property uses.
		/// </summary>
		string Category { get; }
		
		/// <summary>
		/// Gets the name of the property.
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// Gets the type of the property value.
		/// </summary>
		Type ReturnType { get; }
		
		/// <summary>
		/// Gets the type that declares the property.
		/// </summary>
		Type DeclaringType { get; }
		
		/// <summary>
		/// Gets the type converter used to convert property values to/from string.
		/// </summary>
		TypeConverter TypeConverter { get; }
		
		/// <summary>
		/// Gets the description of the property. The returned object should be something that
		/// can be used as Content for a WPF tooltip.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		object GetDescription(); // is not a property because it can create a new instance on every call
		
		/// <summary>
		/// Gets/Sets if the property has been assigned a local value.
		/// Setting this property to false has the effect of resetting the value, setting it to true
		/// copies the default value to a local value.
		/// </summary>
		bool IsSet { get; set; }
		
		/// <summary>
		/// Is raised when the IsSet property has changed.
		/// </summary>
		event EventHandler IsSetChanged;
		
		/// <summary>
		/// Gets if the property value is ambiguous.
		/// </summary>
		bool IsAmbiguous { get; }
		
		/// <summary>
		/// Gets/Sets the value of the property.
		/// </summary>
		object Value { get; set; }
		
		/// <summary>
		/// Is raised when the Value property has changed.
		/// </summary>
		event EventHandler ValueChanged;
		
		/// <summary>
		/// Gets if using a custom expression is supported.
		/// </summary>
		bool CanUseCustomExpression { get; }
		
		/// <summary>
		/// Sets a custom expression.
		/// </summary>
		void SetCustomExpression(string expression);
		
		/// <summary>
		/// Creates a UIElement that can edit the property value.
		/// </summary>
		UIElement CreateEditor();
	}
}
