using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Stores information about command binding loaded from add-in tree
	/// </summary>
	public class CommandBindingDescriptor
	{		
		/// <summary>
		/// Codon used to create this descriptor
		/// </summary>
		public Codon Codon {
			get; private set;
		}
		
		public string Name {
			get; private set;
		}
		
		/// <summary>
		/// Full name of the command class which will be executed when this
		/// binding is triggered
		/// </summary>
		public string Class {
			get; private set;
		}
		
		/// <summary>
		/// Full name of routed UI command which will trigger this binding
		/// 
		/// If command with provided name is not yet registered it's created automatically
		/// </summary>
		public string Command {
			get; private set;
		}
		
		/// <summary>
		/// Override routed command text (string visible to user) if specified
		/// </summary>
		public string CommandText {
			get; private set;
		}
		
		public string OwnerInstanceName {
			get; private set;
		}
		
		public string OwnerTypeName {
			get; private set;
		}
		
		/// <summary>
		/// Gestures.
		/// 
		/// Optional, if provided input bindings in the same context will be created
		/// </summary>
		public string Gestures {
			get; private set;
		}
		
		/// <summary>
		/// If input binding is created in the same context (this is done by setting <see cref="Gestures" /> property)
		/// assign this input binding to provided category
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
		/// Constructor
		/// </summary>
		/// <param name="codon">Reference to codon used to create this descriptor</param>
		public CommandBindingDescriptor(Codon codon)
		{
			Codon = codon;
			Class = Codon.Properties["class"];
			Command = Codon.Properties["command"];
			CommandText = Codon.Properties["commandtext"];
			OwnerInstanceName = Codon.Properties["owner-instance"];
			OwnerTypeName = Codon.Properties["owner-type"];
			Gestures = Codon.Properties["gestures"];
			Categories = Codon.Properties["categories"];
			Name = Codon.Properties["name"];
		}
	}
}
