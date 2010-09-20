// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonStandardModuleResolveResult : ResolveResult
	{
		PythonStandardModuleType standardModuleType;
		
		public PythonStandardModuleResolveResult(PythonStandardModuleType standardModuleType)
			: base(null, null, null)
		{
			this.standardModuleType = standardModuleType;
		}
		
		public override List<ICompletionEntry> GetCompletionData(IProjectContent projectContent)
		{
			PythonModuleCompletionItems completionItems = PythonModuleCompletionItemsFactory.Create(standardModuleType);
			List<ICompletionEntry> items = new List<ICompletionEntry>(completionItems);
			return items;
		}
	}
}
