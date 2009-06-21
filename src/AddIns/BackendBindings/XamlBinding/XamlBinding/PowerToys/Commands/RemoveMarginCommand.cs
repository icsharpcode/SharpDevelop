// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;
using System.Linq;
using System.Xml;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.XamlBinding;

namespace ICSharpCode.XamlBinding.PowerToys.Commands
{
	public class RemoveMarginCommand : XamlMenuCommand
	{
		protected override void Refactor(ITextEditor editor, XmlDocument document)
		{
			RemoveRecursive(document, "Margin");
		}
		
		protected void RemoveRecursive(XmlNode pNode, string name)
		{
			foreach (XmlNode node in pNode.ChildNodes) {
				node.Attributes.Remove(name);
				RemoveRecursive(node, name);
			}
		}
		
		protected void RemoveRecursive(XmlNode pNode, string name, string namespaceURI)
		{
			foreach (XmlNode node in pNode.ChildNodes) {
				node.Attributes.Remove(name, namespaceURI);
				RemoveRecursive(node, name, namespaceURI);
			}
		}
	}
	
	public class GroupIntoMenuItem : XamlMenuCommand
	{
		protected sealed override void Refactor(ITextEditor editor, XmlDocument document)
		{
			if (editor.SelectionLength == 0) {
				MessageService.ShowError("The selected XAML is invalid!");
			}
		}
	}
	
	public class ExtractPropertiesAsStyleCommand : XamlMenuCommand
	{
		protected override void Refactor(ITextEditor editor, XmlDocument document)
		{
			string[] attributes = Utils.GetListOfExistingAttributeNames(editor.Document.Text, editor.Caret.Line, editor.Caret.Column);
			
			
		}
	}
}
