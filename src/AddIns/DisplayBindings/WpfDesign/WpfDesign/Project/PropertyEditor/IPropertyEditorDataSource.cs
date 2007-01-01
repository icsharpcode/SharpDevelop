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
		/// Gets the description of the property.
		/// </summary>
		string Description { get; }
		
		/// <summary>
		/// Gets/Sets if the property has been assigned a local value.
		/// Setting this property to false has the effect of resetting the value, setting it to true
		/// copies the default value to a local value.
		/// </summary>
		bool IsSet { get; set; }
		
		/// <summary>
		/// Gets/Sets the value of the property.
		/// </summary>
		object Value { get; set; }
		
		/// <summary>
		/// Gets if using a custom expression is supported.
		/// </summary>
		bool CanUseCustomExpression { get; }
		
		/// <summary>
		/// Sets a custom expression.
		/// </summary>
		void SetCustomExpression(string expression);
	}
}
