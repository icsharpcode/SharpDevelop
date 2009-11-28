// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Interface that allows a class to convert a Wix property name into a value.
	/// </summary>
	public interface IWixPropertyValueProvider
	{
		/// <summary>
		/// Gets the property value for the specified name. Wix property names are
		/// case sensitive.
		/// </summary>
		string GetValue(string name);
	}
}
