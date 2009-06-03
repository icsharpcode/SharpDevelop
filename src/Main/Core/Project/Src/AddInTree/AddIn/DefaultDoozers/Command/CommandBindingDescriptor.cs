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
		
		/// <summary>
		/// Full name of the command class which will be executed when this
		/// binding is triggered
		/// </summary>
		public string Class {
			get; private set;
		}
		
		/// <summary>
		/// Full name of routed UI command which will trigger this binding
		/// </summary>
		public string Command {
			get; private set;
		}
		
		/// <summary>
		/// Full name of context class.
		/// 
		/// UI element in which this binding will be valid
		/// </summary>
		public string Context {
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
			Context = Codon.Properties["context"];
			Gestures = Codon.Properties["gestures"];
		}
	}
}
