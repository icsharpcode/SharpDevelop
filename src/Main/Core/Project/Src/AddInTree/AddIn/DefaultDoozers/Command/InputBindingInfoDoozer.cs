using System;

namespace ICSharpCode.Core
{
	/// <attribute name="command" use="required">
	/// Name of System.Window.Input.RoutedUICommand associated with built InputBindingInfoDescriptor
	/// The name should be first registered in code using ICSharpCode.Core.Presentation.CommandManager
	/// or in addin file using RoutedUICommand node
	/// </attribute>
	/// <attribute name="ownertype" use="optional">
	/// Name of binding owner type. 
	/// 
	/// A name can be assigned to type in code using ICSharpCode.Core.Presentation.CommandManager.
	/// If this attribute is used "ownerinstance" attribute cannot be used
	/// If neither owner type nor owner instance is specified default owner type is applied
	/// </attribute>
	/// <attribute name="ownertype" use="optional">
	/// Name of binding owner instance. 
	/// 
	/// A name can be assigned to instance in code using ICSharpCode.Core.Presentation.CommandManager.
	/// If this attribute is used "ownertype" attribute cannot be used
	/// If neither owner type nor owner instance is specified default owner type is applied
	/// </attribute>
	/// <attribute name="gestures" use="optional">
	/// Gestures associated with created System.Windows.Input.InputBinding instances
	/// </attribute>
	/// <attribute name="commandtext" use="optional">
	/// Text displayed to user when managing input bindings (shortcuts)
	/// 
	/// If this attribute is not used System.Window.Input.RoutedUICommand.Text property is used
	/// This attribute can only be used together with "gestures" attribute
	/// </attribute>
	/// <attribute name="categories" use="optional">
	/// List of input binding category paths (separated using comma) associated with
	/// created input binding infos
	/// 
	/// Input binding categories can be created in code using ICSharpCode.Core.Presentation.CommandManager or by using InputBindingCategory node in addin file
	/// Input binding categories are used to group shortcuts into separate sections when displaying to user.
	/// </attribute>
	/// <usage>Only in /SharpDevelop/Workbench/InputBindings</usage>
	/// <returns>
	/// CommandBindingInfoDescriptor object
	/// </returns>
	/// <summary>
	/// Registers ICSharpCode.Core.Presentation.InputBindingInfo with specified parameters
	/// </summary>
	public class InputBindingInfoDoozer : IDoozer
	{
		/// <inheritdoc />
		public bool HandleConditions 
		{
			get {
				return true;
			}
		}
		
		/// <summary>
		/// Builds instance of <see cref="InputBindingInfoDescriptor" /> from codon
		/// </summary>
		/// <param name="caller">Caller object</param>
		/// <param name="codon">Codon</param>
		/// <param name="subItems">Codon sub-items</param>
		/// <returns>Instance of <see cref="InputBindingInfoDescriptor" /></returns>
		public object BuildItem(object caller, Codon codon, System.Collections.ArrayList subItems)
		{
			return new InputBindingInfoDescriptor(codon);
		}
	}
}
