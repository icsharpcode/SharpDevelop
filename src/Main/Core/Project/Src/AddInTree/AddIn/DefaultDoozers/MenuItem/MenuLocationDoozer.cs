using System;
using System.Collections;

namespace ICSharpCode.Core
{
	/// <attribute name="menupath" use="required">
	/// Path to menu root category
	/// </attribute>
	/// <attribute name="name" use="required">
	/// Category name displayed to user
	/// </attribute>
	/// <usage>Only in /SharpDevelop/CommandManager/MenuLocations</usage>
	/// <returns>
	/// MenuLocationDoozer object
	/// </returns>
	/// <summary>
	/// Describes shortcuts in menu affiliation to input binding categories
	/// 
	/// Root-level ICSharpCode.Core.Presentation.InputBindingInfo are associated with provided category. For ICSharpCode.Core.Presentation.InputBindingInfo 
	/// in sub-menus new ICSharpCode.Core.Presentation.InputBindingCategory is created from sub-menu id and title
	/// </summary>
	public class MenuLocationDoozer : IDoozer
	{
		/// <inheritdoc />
		public bool HandleConditions {
			get {
				return true;
			}
		}
		
		/// <summary>
		/// Builds instance of <see cref="MenuLocationDescriptor" /> from codon
		/// </summary>
		/// <param name="caller">Caller object</param>
		/// <param name="codon">Codon</param>
		/// <param name="subItems">Codon sub-items</param>
		/// <returns>Instance of <see cref="MenuLocationDescriptor" /></returns>
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			return new MenuLocationDescriptor(caller, codon);
		}
	}
}
