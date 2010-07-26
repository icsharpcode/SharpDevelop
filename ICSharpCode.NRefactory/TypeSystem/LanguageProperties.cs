// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Diagnostics.Contracts;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Describes the properties of a language.
	/// </summary>
	public class LanguageProperties
	{
		// TODO: is this class actually useful? consider removing it
		
		public virtual StringComparer NameComparer {
			get {
				Contract.Ensures(Contract.Result<StringComparer>() != null);
				Contract.Assume(StringComparer.Ordinal != null);
				return StringComparer.Ordinal;
			}
		}
	}
}
