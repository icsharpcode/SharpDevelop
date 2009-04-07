// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Gui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of XamlColorizerServer.
	/// </summary>
	public static class XamlColorizerServer
	{
		public static void InitializeServer()
		{
			WorkbenchSingleton.Workbench.ViewOpened += new ViewContentEventHandler(WorkbenchSingleton_Workbench_ViewOpened);
		}

		static void WorkbenchSingleton_Workbench_ViewOpened(object sender, ViewContentEventArgs e)
		{
			if (e.Content is AvalonEditViewContent) {
				AvalonEditViewContent content = e.Content as AvalonEditViewContent;
				if (!Path.GetExtension(content.CodeEditor.FileName).Equals(".xaml", StringComparison.OrdinalIgnoreCase))
					return;
				content.CodeEditor.TextArea.TextView.LineTransformers.Insert(1, new XamlColorizer(content));
			}
		}
	}
	
	public class ColorizerDoozer : IDoozer
	{
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		public object BuildItem(object caller, Codon codon, System.Collections.ArrayList subItems)
		{
			return new ColorizerDescriptor(codon);
		}
	}
	
	public class ColorizerDescriptor
	{
		Codon codon;
		ColorizingTransformer colorizer;
		
		public ColorizingTransformer Colorizer {
			get {
				if (colorizer == null)
					this.colorizer = (ColorizingTransformer)codon.AddIn.CreateObject(codon.Properties["class"]);
				
				return colorizer;
			}
		}
		
		public ColorizerDescriptor(Codon codon)
		{
			this.codon = codon;
		}
		
		
	}
}
