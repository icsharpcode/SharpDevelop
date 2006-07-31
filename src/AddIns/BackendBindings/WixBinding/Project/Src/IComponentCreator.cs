// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Interface that creates an IComponent given a type. Used by the WixDialog
	/// class so it can be wired up to a IDesignerHost
	/// </summary>
	public interface IComponentCreator
	{
		/// <summary>
		/// Creates a named component of the specified type.
		/// </summary>
		/// <param name="componentClass">The type of the component to be created.</param>
		/// <param name="name">The component name.</param>
		IComponent CreateComponent(Type componentClass, string name);
	}
}
