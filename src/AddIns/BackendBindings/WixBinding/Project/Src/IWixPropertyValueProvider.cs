// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
