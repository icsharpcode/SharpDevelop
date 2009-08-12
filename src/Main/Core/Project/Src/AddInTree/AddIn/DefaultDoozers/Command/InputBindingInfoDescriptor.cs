using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Describes <see cref="ICSharpCode.Core.Presentation.InputBindingInfo" /> 
	/// </summary>
	public class InputBindingInfoDescriptor
	{
		/// <summary>
		/// Codon used to create this descriptor
		/// </summary>
		public Codon Codon {
			get; private set;
		}
		
		/// <summary>
		/// Gets name of <see cref="System.Window.Input.RoutedUICommand" /> associated with descriptor
		/// The name should be associated to a command in code using <see cref="ICSharpCode.Core.Presentation.CommandManager" />
		/// </summary>
		public string Command {
			get; private set;
		}
		
		
		/// <summary>
		/// Gets overriden routed command text (string visible to user)
		/// 
		/// If not provided <see cref="System.Window.Input.RoutedUICommand.Text" /> property value is used
		/// </summary>
		public string CommandText {
			get; private set;
		}
		
		/// <summary>
		/// Gets name of binding owner type name. 
		/// 
		/// A name can be assigned to type in code using <see cref="ICSharpCode.Core.Presentation.CommandManager" />.
		/// If this property is set then <see cref="OwnerTypeName" /> property can be ignored
		/// If neither owner type nor owner instance is specified default owner type is applied
		/// </summary>
		public string OwnerInstanceName {
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
		/// </summary>
		public string Gestures {
			get; private set;
		}
		
		
		/// <summary>
		/// List of paths to input binding categories associated with created <see cref="ICSharpCode.Core.Presentation.InputBindingInfo" /> (separated by comma) 
		/// </summary>
		public string Categories {
			get; private set;
		}
		
		/// <summary>
		/// Creates new instance of <see cref="InputBindingInfoDescriptor" />
		/// </summary>
		/// <param name="codon">Reference to codon used to create this descriptor</param>
		public InputBindingInfoDescriptor(Codon codon)
		{
			Codon = codon;
			Command = codon.Properties["command"];
			CommandText = codon.Properties["commandtext"];
			OwnerInstanceName = codon.Properties["ownerinstance"];
			OwnerTypeName = codon.Properties["ownertype"];
			Gestures = codon.Properties["gestures"];
			Categories = codon.Properties["categories"];
		}
	}
}
