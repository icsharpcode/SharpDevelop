// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
