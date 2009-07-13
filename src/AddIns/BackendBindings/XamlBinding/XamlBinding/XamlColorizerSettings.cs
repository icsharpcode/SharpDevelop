// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Media;

namespace ICSharpCode.XamlBinding
{
	public class XamlColorizerSettings
	{
		public Brush PropertyForegroundBrush { get; set; }
		public Brush PropertyBackgroundBrush { get; set; }
		
		public Brush EventForegroundBrush { get; set; }
		public Brush EventBackgroundBrush { get; set; }
		
		public Brush NamespaceDeclarationForegroundBrush { get; set; }
		public Brush NamespaceDeclarationBackgroundBrush { get; set; }
		
		public XamlColorizerSettings()
		{
			this.PropertyBackgroundBrush = Brushes.Transparent;
			this.PropertyForegroundBrush = Brushes.CadetBlue;
			
			this.EventBackgroundBrush = Brushes.Transparent;
			this.EventForegroundBrush = Brushes.Green;
			
			this.NamespaceDeclarationBackgroundBrush = Brushes.Transparent;
			this.NamespaceDeclarationForegroundBrush = Brushes.Orange;
		}
	}
}
