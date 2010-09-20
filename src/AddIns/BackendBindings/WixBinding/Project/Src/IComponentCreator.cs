// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
