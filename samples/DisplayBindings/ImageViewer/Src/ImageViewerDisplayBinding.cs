/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 19.10.2005
 * Time: 21:41
 */
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ImageViewer
{
	public class ImageViewerDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName) {
			return true;
		}
		public IViewContent CreateContentForFile(string fileName) {
			ImageViewContent vc = new ImageViewContent();
			vc.Load(fileName);
			return vc;
		}
		public bool CanCreateContentForLanguage(string languageName) {
			return false;
		}
		public IViewContent CreateContentForLanguage(string languageName, string content) {
			throw new NotImplementedException();
		}
	}
}
