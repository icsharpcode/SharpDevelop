// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Gui;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;

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
			this.PropertyForegroundBrush = Brushes.Red;
			
			this.EventBackgroundBrush = Brushes.Transparent;
			this.EventForegroundBrush = Brushes.Green;
			
			this.NamespaceDeclarationBackgroundBrush = Brushes.Transparent;
			this.NamespaceDeclarationForegroundBrush = Brushes.Orange;
		}
	}
}
