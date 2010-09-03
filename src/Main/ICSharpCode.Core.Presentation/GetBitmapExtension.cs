// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
