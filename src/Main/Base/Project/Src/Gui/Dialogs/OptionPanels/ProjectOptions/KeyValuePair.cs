// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
