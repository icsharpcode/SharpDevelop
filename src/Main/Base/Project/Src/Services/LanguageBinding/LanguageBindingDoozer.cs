// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Creates LanguageBindingDescriptor objects for the language binding service.
	/// </summary>
	/// <attribute name="extensions" use="required">
	/// Semicolon-separated list of file extensions that are handled by the language binding (e.g. .xaml)
	/// </attribute>
	/// <attribute name="class" use="required">
	/// Name of the ILanguageBinding class.
	/// </attribute>
	/// <usage>Only in /SharpDevelop/Workbench/LanguageBindings</usage>
	/// <returns>
	/// A LanguageBindingDescriptor object that wraps the ILanguageBinding object.
	/// </returns>
	public class LanguageBindingDoozer : IDoozer
	{
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		public object BuildItem(object caller, Codon codon, System.Collections.ArrayList subItems)
		{
			return new LanguageBindingDescriptor(codon);
		}
	}
}
