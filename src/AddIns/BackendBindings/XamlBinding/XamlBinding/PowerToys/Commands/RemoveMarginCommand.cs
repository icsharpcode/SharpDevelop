// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.XmlEditor;
using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.XamlBinding;

namespace ICSharpCode.XamlBinding.PowerToys.Commands
{
	public class RemoveMarginCommand : XamlMenuCommand
	{
		protected override void Refactor(ITextEditor editor, XDocument document)
		{
			RemoveRecursive(document.Root, "Margin");
		}
		
		protected void RemoveRecursive(XElement element, string name)
		{
			foreach (XAttribute a in element.Attributes(name))
				a.Remove();
			
			foreach (XElement e in element.Elements())
				RemoveRecursive(e, name);
		}
	}
	
	public class GroupIntoMenuItem : XamlMenuCommand
	{
		protected sealed override void Refactor(ITextEditor editor, XDocument document)
		{
			if (editor.SelectionLength == 0) {
				MessageService.ShowError("The selected XAML is invalid!");
			}
		}
	}
	
	public class ExtractPropertiesAsStyleCommand : XamlMenuCommand
	{
		protected override void Refactor(ITextEditor editor, XDocument document)
		{
			XmlElementPath path = XmlParser.GetParentElementPath(editor.Document.Text.Substring(0, editor.SelectionStart));
		}
	}
}
