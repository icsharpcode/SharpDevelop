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
	public class MenuRootDoozer : IDoozer
	{
		public bool HandleConditions {
			get {
				return true;
			}
		}
		
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			return new MenuRootDescriptor(caller, codon);
		}
	}
	

	public class MenuRootDescriptor 
	{
		private Codon codon;
		
		public string Category
		{
			get; private set;
		}
	
		public string Path
		{
			get; private set;
		}
	
		public MenuRootDescriptor(object caller, Codon codon)
		{
			this.codon = codon;
			
			Path = codon.Properties["path"];
			
			if(codon.Properties.Contains("category")){
				Category = codon.Properties["category"];
			}
		}
	}
}
