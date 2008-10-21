using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.WpfDesign.Designer.Services;
using ICSharpCode.FormsDesigner.Services;

namespace ICSharpCode.WpfDesign.AddIn
{
	public class IdeChooseClassService : ChooseClassServiceBase
	{
		public override IEnumerable<Assembly> GetAssemblies()
		{
			var pc = ParserService.CurrentProjectContent;
			if (pc != null) {
				var a = XamlMapper.TypeResolutionServiceInstance.LoadAssembly(pc);
				if (a != null) yield return a;
				foreach (var r in pc.ReferencedContents) {
					a = XamlMapper.TypeResolutionServiceInstance.LoadAssembly(r);
					if (a != null) yield return a;
				}
			}
		}
	}
}