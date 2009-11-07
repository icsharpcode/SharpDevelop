using System;
using System.Linq;
using System.Collections.Generic;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Describes <see cref="ICSharpCode.Core.Presentation.CommandBindingInfo" /> 
	/// </summary>
	public class InputBindingCategoryDescriptor
	{
		/// <summary>
		/// Gets category Id used to create category path
		/// </summary>
		public string Id {
			get; private set;
		}
		
		/// <summary>
		/// Gets category name displayed to user
		/// </summary>
		public string Text {
			get; private set;
		}
		
		/// <summary>
		/// Gets children category descriptors
		/// </summary>
		public List<InputBindingCategoryDescriptor> Children {
			get; private set;
		}
		
		/// <summary>
		/// Creates instance of <see cref="InputBindingCategoryDescriptor" />
		/// </summary>
		/// <param name="codon">Reference to codon used to create this descriptor</param>
		/// <param name="subItems">List of sub-category descriptors</param>
		public InputBindingCategoryDescriptor(Codon codon, System.Collections.ArrayList subItems) 
		{
			Id = codon.Properties["id"]; 
			Text = codon.Properties["text"];
			Children = subItems != null ? subItems.Cast<InputBindingCategoryDescriptor>().ToList() : new List<InputBindingCategoryDescriptor>();
		}
	}
}
