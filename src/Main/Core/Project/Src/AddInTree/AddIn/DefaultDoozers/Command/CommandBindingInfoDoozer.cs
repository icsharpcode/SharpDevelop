using System;

namespace ICSharpCode.Core
{
	/// <attribute name="command" use="required">
	/// Name of System.Window.Input.RoutedUICommand associated with built CommandBindingInfoDescriptor
	/// The name should be first registered in code using ICSharpCode.Core.Presentation.CommandManager
	/// or in addin file using RoutedUICommand node
	/// </attribute>
	/// <attribute name="class" use="required">
	/// Full name of the command class (should implement ICSharpCode.Core.ICommand or System.Window.Input.ICommand) containing user instructions associated with
	/// System.Windows.Input.CommandBinding
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
	/// <attribute name="lazy" use="optional" enum="1;0;true;false">
	/// Use lazy loading. 
	/// </attribute>
	/// <attribute name="gestures" use="optional">
	/// Create input bindings assigned to the same owner type or owner instance and associated 
	/// with the same System.Window.Input.RoutedUICommand
	/// </attribute>
	/// <attribute name="commandtext" use="optional">
	/// Text displayed to user when managing input bindings (shortcuts)
	/// 
	/// If this attribute is not used System.Window.Input.RoutedUICommand.Text property is used
	/// This attribute can only be used together with "gestures" attribute
	/// </attribute>
	/// <attribute name="categories" use="optional">
	/// List of paths to input binding categories associated with created input binding info (separated by comma) 
	/// 
	/// Input binding categories can be created in code using ICSharpCode.Core.Presentation.CommandManager or by using InputBindingCategory node in addin file
	/// Input binding categories are used to group shortcuts into separate sections when displaying to user.
	/// This attribute can only be used together with "gestures" attribute
	/// </attribute>	
	/// <usage>Only in /SharpDevelop/Workbench/CommandBindings</usage>
	/// <returns>
	/// CommandBindingInfoDescriptor object
	/// </returns>
	/// <summary>
	/// Registers ICSharpCode.Core.Presentation.CommandBindingInfo with specified pa
	/// </summary>
	public class CommandBindingInfoDoozer : IDoozer
	{
		/// <inheritdoc />
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Builds instance of <see cref="CommandBindingInfoDescriptor" /> from codon
		/// </summary>
		/// <param name="caller">Caller object</param>
		/// <param name="codon">Codon</param>
		/// <param name="subItems">Codon sub-items</param>
		/// <returns>Instance of <see cref="CommandBindingInfoDescriptor" /></returns>
		public object BuildItem(object caller, Codon codon, System.Collections.ArrayList subItems)
		{
			return new CommandBindingInfoDescriptor(codon);
		}
	}
}
