/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 19.10.2005
 * Time: 17:02
 */

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace HtmlPreview
{
	/// <summary>
	/// Description of PreviewDisplayBinding.
	/// </summary>
	public class PreviewDisplayBinding : ISecondaryDisplayBinding
	{
		public bool ReattachWhenParserServiceIsReady {
			get {
				return false;
			}
		}
		
		public bool CanAttachTo(IViewContent content) {
			return true;
		}
		
		public ISecondaryViewContent[] CreateSecondaryViewContent(IViewContent viewContent) {
			return new ISecondaryViewContent[] { new PreviewViewContent() };
		}
	}
}
