// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Represents a Control's Text Property.
	/// </summary>
	public class PythonControlTextProperty : PythonControlProperty
	{
		public PythonControlTextProperty()
		{
		}

		/// <summary>
		/// Returns true if the Text property's value is an empty string.
		/// </summary>
		public override bool IsDefaultValue(object propertyValue)
		{
			return (string)propertyValue == String.Empty;
		}
	}
}
