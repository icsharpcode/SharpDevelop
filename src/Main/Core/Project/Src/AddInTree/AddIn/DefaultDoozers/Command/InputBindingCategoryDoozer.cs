/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 7/3/2009
 * Time: 4:50 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Description of InputBindingInfoCategory.
	/// </summary>
	public class InputBindingCategoryDoozer : IDoozer
	{
		/// <see cref="IDoozer.HandleConditions" />
		public bool HandleConditions {
			get {
				return true;
			}
		}
		
		/// <see cref="IDoozer.BuildItem(object, Codon, System.Collections.ArrayList)">
		/// Builds InputBindingDescriptor
		/// </see>
		public object BuildItem(object caller, Codon codon, System.Collections.ArrayList subItems)
		{
			return new InputBindingCategoryDescriptor(codon, subItems);
		}
	}
	
	public class InputBindingCategoryDescriptor
	{
		public string Id;
		public string Text;
		public List<InputBindingCategoryDescriptor> Children;
		
		public InputBindingCategoryDescriptor(Codon codon, System.Collections.ArrayList subItems) {
			Id = codon.Properties["id"]; 
			Text = codon.Properties["text"];
			Children = subItems != null ? subItems.Cast<InputBindingCategoryDescriptor>().ToList() : new List<InputBindingCategoryDescriptor>();
		}
	}
}
