// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Markup extension that gets a BitmapSource object for a ResourceService bitmap.
	/// </summary>
	[MarkupExtensionReturnType(typeof(BitmapSource))]
	public class GetBitmapExtension : MarkupExtension
	{
		public GetBitmapExtension(string key)
		{
			this.key = key;
		}
		
		protected string key;
		
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return PresentationResourceService.GetBitmapSource(key);
		}
	}
}
