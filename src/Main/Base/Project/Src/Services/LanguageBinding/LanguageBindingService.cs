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
		
		/// <summary>
		/// Creates the binding for the specified text editor. This method never returns null.
		/// </summary>
		public static ILanguageBinding CreateBinding(ITextEditor editor)
		{
			var bindings = AddInTree.BuildItems<ILanguageBinding>(languageBindingPath, editor, false);
			return new AggregatedLanguageBinding(bindings);
		}
	}
}
