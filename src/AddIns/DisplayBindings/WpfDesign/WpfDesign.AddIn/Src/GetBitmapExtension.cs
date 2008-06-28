using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

using ICSharpCode.Core;

namespace ICSharpCode.WpfDesign.AddIn
{
	class GetBitmapExtension : MarkupExtension
	{
		public GetBitmapExtension(string key)
		{
			this.key = key;
		}
		
		static Dictionary<string, BitmapSource> cache = new Dictionary<string, BitmapSource>();
		
		protected string key;
		
		public override object ProvideValue(IServiceProvider sp)
		{
			lock (cache) {
				BitmapSource result;
				if (!cache.TryGetValue(key, out result)) {
					result = GetBitmapSource();
					result.Freeze();
					cache[key] = result;
				}
				return result;
			}
		}
		
		BitmapSource GetBitmapSource()
		{
			using (Bitmap bitmap = ResourceService.GetBitmap(key)) {
				return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
				                                             Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			}
		}
	}
}
