using System;
using System.Windows.Markup;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.WpfDesign.AddIn
{
	class GetBitmapExtension : MarkupExtension
	{
		public GetBitmapExtension(string key)
		{
			this.key = key;
		}
		
		protected string key;
		
		public override object ProvideValue(IServiceProvider sp)
		{
			return PresentationResourceService.GetBitmapSource(key);
		}
	}
}
