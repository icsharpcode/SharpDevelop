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
	/// Represents a named item in an array.
	/// </summary>
	public interface IArrayItem
	{
		string Name { get; }
	}
}
