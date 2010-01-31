// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
