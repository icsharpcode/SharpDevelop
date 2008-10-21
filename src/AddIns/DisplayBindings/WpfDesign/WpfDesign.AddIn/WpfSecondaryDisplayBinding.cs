// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 3497 $</version>
// </file>

using System;
using System.IO;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using System.Xml;
using ICSharpCode.Xaml;

namespace ICSharpCode.WpfDesign.AddIn
{
	public class WpfSecondaryDisplayBinding : ISecondaryDisplayBinding
	{
		public bool ReattachWhenParserServiceIsReady
		{
			get
			{
				return false;
			}
		}

		public bool CanAttachTo(IViewContent content)
		{
			return XamlConstants.HasXamlExtension(content.PrimaryFileName);
		}

		public IViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			return new IViewContent[] { new WpfSecondaryViewContent(viewContent as WpfPrimaryViewContent) };
		}
	}
}
