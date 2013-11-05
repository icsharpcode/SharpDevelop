// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop
{
	public interface ILanguageService
	{
		ILanguageBinding GetLanguageByFileName(FileName fileName);
		ILanguageBinding GetLanguageByExtension(string extension);
		ILanguageBinding GetLanguageByName(string name);
	}
	
	class SDLanguageService : ILanguageService
	{
		const string languageBindingPath = "/SharpDevelop/Workbench/LanguageBindings";
		readonly List<LanguageBindingDescriptor> bindings;
		
		public SDLanguageService()
		{
			bindings = AddInTree.BuildItems<LanguageBindingDescriptor>(languageBindingPath, null, false);
		}
		
		public ILanguageBinding GetLanguageByFileName(FileName fileName)
		{
			return GetLanguageByExtension(Path.GetExtension(fileName));
		}
		
		public ILanguageBinding GetLanguageByExtension(string extension)
		{
			foreach (var language in bindings) {
				if (language.CanAttach(extension))
					return language.Binding;
			}
			return DefaultLanguageBinding.DefaultInstance;
		}
		
		public ILanguageBinding GetLanguageByName(string name)
		{
			foreach (var language in bindings) {
				if (language.Name == name)
					return language.Binding;
			}
			return DefaultLanguageBinding.DefaultInstance;
		}
	}
}
