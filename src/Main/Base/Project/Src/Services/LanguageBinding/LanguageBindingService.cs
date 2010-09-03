// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop
{
	public static class LanguageBindingService
	{
		const string languageBindingPath = "/SharpDevelop/Workbench/LanguageBindings";
		static readonly List<LanguageBindingDescriptor> bindings;
		
		static LanguageBindingService()
		{
			bindings = AddInTree.BuildItems<LanguageBindingDescriptor>(languageBindingPath, null, false);
		}
		
		/// <summary>
		/// Creates the binding for the specified text editor. This method never returns null.
		/// </summary>
		public static ILanguageBinding CreateBinding(ITextEditor editor)
		{
			return new AggregatedLanguageBinding(FindMatchingBindings(editor));
		}
		
		static ILanguageBinding[] FindMatchingBindings(ITextEditor editor)
		{
			List<ILanguageBinding> matches = new List<ILanguageBinding>();
			foreach (var binding in bindings) {
				if (binding.CanAttach(editor))
					matches.Add(binding.CreateBinding(editor));
			}
			
			return matches.ToArray();
		}
	}
}
