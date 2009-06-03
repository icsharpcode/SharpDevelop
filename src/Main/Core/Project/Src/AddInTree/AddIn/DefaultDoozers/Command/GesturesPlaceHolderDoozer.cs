using System;

namespace ICSharpCode.Core
{
	/// <attribute name="name" use="required">
	/// Place holder name. This name should be unique application wide
	/// </attribute>
	/// <attribute name="text" use="required">
	/// Text displayed to use
	/// </attribute>
	/// <attribute name="lazy" use="optional" enum="1;0;true;false">
	/// Use lazy loading. If addin containing binded command is 
	/// not loaded yet, load asseblies referenced in add-in and then 
	/// invoke command
	/// </attribute>
	/// <attribute name="gestures" use="optional">
	/// Keys which will invoke command
	/// </attribute>
	/// <usage>Only in /SharpDevelop/Workbench/CommandBindings</usage>
	/// <returns>
	/// GesturesPlaceHolder object
	/// </returns>
	/// <summary>
	/// Creates descriptor containing information about command binding
	/// </summary>
	public class GesturesPlaceHolderDoozer : IDoozer
	{
		/// <see cref="IDoozer.HandleConditions" />
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		/// <see cref="IDoozer.BuildItem(object, Codon, System.Collections.ArrayList)">
		/// Builds GesturesPlaceHolderDoozer
		/// </see>
		public object BuildItem(object caller, Codon codon, System.Collections.ArrayList subItems)
		{
			return new GesturesPlaceHolderDescriptor(codon);
		}
	}
}
