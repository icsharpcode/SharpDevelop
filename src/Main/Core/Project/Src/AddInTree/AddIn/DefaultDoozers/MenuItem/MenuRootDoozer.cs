/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 6/28/2009
 * Time: 10:12 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Description of MenuDescriptionDoozer.
	/// </summary>
	public class MenuLocationDoozer : IDoozer
	{
		public bool HandleConditions {
			get {
				return true;
			}
		}
		
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			return new MenuLocationDescriptor(caller, codon);
		}
	}
	

	public class MenuLocationDescriptor 
	{
		public string Category
		{
			get; private set;
		}
	
		public string Path
		{
			get; private set;
		}
	
		public MenuLocationDescriptor(object caller, Codon codon)
		{
			if(!codon.Properties.Contains("path") || !codon.Properties.Contains("category")){
				throw new ArgumentException("Menu location should have path and category");
			}
			
			Path = codon.Properties["path"];
			Category = codon.Properties["category"];
		}
	}
}
