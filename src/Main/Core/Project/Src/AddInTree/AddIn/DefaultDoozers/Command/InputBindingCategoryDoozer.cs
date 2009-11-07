using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.Core
{
	/// <attribute name="id" use="required">
	/// Category Id used in category path
	/// </attribute>
	/// <attribute name="name" use="required">
	/// Category name displayed to user
	/// </attribute>
	/// <usage>Only in /SharpDevelop/CommandManager/InputBindingCategories</usage>
	/// <returns>
	/// InputBindingCategory object
	/// </returns>
	/// <summary>
	/// Associates ICSharpCode.Core.Presentation.InputBindingCategory with path which can later be used to reference created ICSharpCode.Core.Presentation.InputBindingCategory
	/// </summary>
	public class InputBindingCategoryDoozer : IDoozer
	{
		/// <inheritdoc />
		public bool HandleConditions {
			get {
				return true;
			}
		}
		
		/// <summary>
		/// Builds instance of <see cref="InputBindingCategoryDescriptor" /> from codon
		/// </summary>
		/// <param name="caller">Caller object</param>
		/// <param name="codon">Codon</param>
		/// <param name="subItems">Codon sub-items</param>
		/// <returns>Instance of <see cref="InputBindingCategoryDescriptor" /></returns>
		public object BuildItem(object caller, Codon codon, System.Collections.ArrayList subItems)
		{
			return new InputBindingCategoryDescriptor(codon, subItems);
		}
	}
	

}
