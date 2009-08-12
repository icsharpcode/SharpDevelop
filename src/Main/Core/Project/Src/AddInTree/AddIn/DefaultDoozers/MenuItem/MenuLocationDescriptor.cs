using System;

namespace ICSharpCode.Core
{	
	/// <summary>
	/// Describes shortcuts in menu affiliation to input binding category
	/// 
	/// Root-level ICSharpCode.Core.Presentation.InputBindingInfo are associated with provided categories. For ICSharpCode.Core.Presentation.InputBindingInfo 
	/// in sub-menus new ICSharpCode.Core.Presentation.InputBindingCategory is created from sub-menu id and title
	/// </summary>
	public class MenuLocationDescriptor
	{
		/// <summary>
		/// Gets paths to categories associated with menu
		/// </summary>
		public string CategoryPaths
		{
			get; private set;
		}
	
		/// <summary>
		/// Gets path to menu root
		/// </summary>
		public string MenuPath
		{
			get; private set;
		}
	
		/// <summary>
		/// Creates new instance of <see cref="MenuLocationDescriptor" />
		/// </summary>
		/// <param name="caller">Caller object</param>
		/// <param name="codon">Reference to codon used to create this descriptor</param>
		public MenuLocationDescriptor(object caller, Codon codon)
		{
			if(!codon.Properties.Contains("menupath") || !codon.Properties.Contains("categorypaths")){
				throw new ArgumentException("Menu location should have both menu path and category paths assigned");
			}
			
			MenuPath = codon.Properties["menupath"];
			CategoryPaths = codon.Properties["categorypaths"];
		}
	}
}
