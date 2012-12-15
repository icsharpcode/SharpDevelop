/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 09.05.2012
 * Time: 19:54
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Description of KeyValuePair.
	/// </summary>
	public class KeyItemPair
	{
		public KeyItemPair(string key,string displayValue)
		{
			this.Key = key;
			this.DisplayValue = displayValue;
		}
		
		public string Key {get;private set;}
		
		public string DisplayValue {get; private set;}
	}
}
