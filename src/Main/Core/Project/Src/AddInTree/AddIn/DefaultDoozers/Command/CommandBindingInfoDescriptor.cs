using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Describes <see cref="ICSharpCode.Core.Presentation.CommandBindingInfo" /> 
	/// </summary>
	public class CommandBindingInfoDescriptor
	{		
		/// <summary>
		/// Gets codon used to create this descriptor
		/// </summary>
		public Codon Codon 
		{
			get; private set;
		}
		
		/// <summary>
		/// Gets full name of the command class containing user instructions
		/// 
		/// The class must implement <see cref="System.Windows.Input.ICommand" /> or <see cref="ICSharpCode.Core.ICommand" />
		/// classes
		/// </summary>
		public string Class 
		{
			get; private set;
		}
		
		/// <summary>
		/// Gets name of <see cref="System.Window.Input.RoutedUICommand" /> associated with descriptor
		/// The name should be associated to a command in code using <see cref="ICSharpCode.Core.Presentation.CommandManager" />
		/// </summary>
		public string Command 
		{
			get; private set;
		}
		
		/// <summary>
		/// Gets overriden routed command text (string visible to user)
		/// 
		/// If not provided <see cref="System.Window.Input.RoutedUICommand.Text" /> property value is used
		/// </summary>
		public string CommandText 
		{
			get; private set;
		}
		
		/// <summary>
		/// Gets name of binding owner type name. 
		/// 
		/// A name can be assigned to type in code using <see cref="ICSharpCode.Core.Presentation.CommandManager" />.
		/// If this property is set then <see cref="OwnerTypeName" /> property can be ignored
		/// If neither owner type nor owner instance is specified default owner type is applied
		/// </summary>
		public string OwnerInstanceName 
		{
			get; private set;
		}
		
		/// <summary>
		/// Name of binding owner instance name. 
		/// 
		/// A name can be assigned to instance in code using <see cref="ICSharpCode.Core.Presentation.CommandManager" />.
		/// If this property is set then <see cref="OwnerInstanceName" /> property can be ignored
		/// If neither owner type nor owner instance is specified default owner type is applied
		/// </summary>
		public string OwnerTypeName {
			get; private set;
		}
		
		/// <summary>
		/// Gets gestures associated with created <see cref="ICSharpCode.Core.Presentation.InputBindingInfo" /> 
		/// 
		/// Optional, if provided input binding info with the same owner will be created
		/// </summary>
		public string Gestures {
			get; private set;
		}
		
		/// <summary>
		/// Gets list of paths to input binding categories associated with created <see cref="ICSharpCode.Core.Presentation.InputBindingInfo" /> (separated by comma) 
		/// 
		/// Can only be used together with <see cref="Gestures" /> property
		/// </summary>
		public string Categories {
			get; private set;
		}
		
		/// <summary>
		/// Lazy loading
		/// 
		/// If true add-in referenced assemblies are loaded when command is invoked.
		/// Otherwise command can't be invoked until addin is loaded
		/// </summary>
		public bool Lazy {
			get {
				return Codon.Properties["lazy"] == "1" || Codon.Properties["lazy"] == "true";
			}
		}
		
		/// <summary>
		/// Creates new instance of <see cref="CommandBindingInfoDescriptor" />
		/// </summary>
		/// <param name="codon">Reference to codon used to create this descriptor</param>
		public CommandBindingInfoDescriptor(Codon codon)
		{
			Codon = codon;
			Class = Codon.Properties["class"];
			Command = Codon.Properties["command"];
			CommandText = Codon.Properties["commandtext"];
			OwnerInstanceName = Codon.Properties["ownerinstance"];
			OwnerTypeName = Codon.Properties["ownertype"];
			Gestures = Codon.Properties["gestures"];
			Categories = Codon.Properties["categories"];
		}
	}
}
