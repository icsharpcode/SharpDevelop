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
	/// Represents a property on a control or form.
	/// </summary>
	public abstract class PythonControlProperty
	{
		public PythonControlProperty()
		{
		}
		
		/// <summary>
		/// Returns true if the property value matches the default for this control's property.
		/// </summary>
		public virtual bool IsDefaultValue(object propertyValue)
		{
			return false;
		}
	}
}
