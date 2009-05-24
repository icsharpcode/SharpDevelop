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
			get {
				return Codon.Properties["class"];
			}
		}
		
		/// <summary>
		/// Full name of routed UI command which will trigger this binding
		/// </summary>
		public string Command {
			get {
				return Codon.Properties["command"];
			}
		}
		
		/// <summary>
		/// Full name of context class.
		/// 
		/// UI element in which this binding will be valid
		/// </summary>
		public string Context {
			get {
				return Codon.Properties["context"];
			}
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
		}
	}
}
