using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.WpfDesign.AddIn
{
	public class ChooseClassService
	{
		public IClass ChooseClass()
		{
			var core = new ChooseClass(ParserService.CurrentProjectContent);
			var window = new ChooseClassDialog(core);
			
			if (window.ShowDialog().Value) {
				return core.CurrentClass;
			}
			return null;
		}
	}
}
