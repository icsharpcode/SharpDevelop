using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Stores information about input binding loaded from add-in tree
	/// </summary>
	public class InputBindingDescriptor
	{
		/// <summary>
		/// Codon used to create this descriptor
		/// </summary>
		public Codon Codon {
			get; private set;
		}
		
		/// <summary>
		/// Full name of routed UI command which will be invoked when this binding is triggered
		/// </summary>
		public string Command {
			get; private set;
		}

		public string Name {
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
		/// Description of gesture which will trigger this binding
		/// </summary>
		public string Gestures {
			get; private set;
		}
		
		/// <summary>
		/// Category to which this binding belongs
		/// </summary>
		public string Category {
			get; private set;
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="codon">Reference to codon used to create this descriptor</param>
		public InputBindingDescriptor(Codon codon)
		{
			Codon = codon;
			Command = codon.Properties["command"];
			CommandText = codon.Properties["commandtext"];
			OwnerInstanceName = codon.Properties["owner-instance"];
			OwnerTypeName = codon.Properties["owner-type"];
			Gestures = codon.Properties["gestures"];
			Category = codon.Properties["category"];
			Name = codon.Properties["name"];
		}
	}
}
