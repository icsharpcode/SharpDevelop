// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.XamlBinding.PowerToys.Dialogs;

namespace ICSharpCode.XamlBinding.PowerToys.Commands
{
	/// <summary>
	/// Description of ExtractPropertiesAsStyleCommand.
	/// </summary>
	public class ExtractPropertiesAsStyleCommand : XamlMenuCommand
	{
		protected override bool Refactor(ITextEditor editor, XDocument document)
		{
			if (editor.SelectionLength == 0) {
				MessageService.ShowError("Nothing selected!");
				return false;
			}
			
			Location startLoc = editor.Document.OffsetToPosition(editor.SelectionStart);
			Location endLoc = editor.Document.OffsetToPosition(editor.SelectionStart + editor.SelectionLength);
			
			XElement selectedItem = (from item in document.Root.Descendants()
			                         where item.IsInRange(startLoc, endLoc) select item).FirstOrDefault();
			
			if (selectedItem != null) {
				var attributes = selectedItem.Attributes().Select(item => new PropertyEntry(item, Resolve(item, editor))).ToList();
				attributes = attributes.Concat(selectedItem.Elements().Where(el => el.Name.LocalName.Contains("."))
				                               .Select(element => new PropertyEntry(element, Resolve(element, editor)))
				                              ).ToList();
				ExtractPropertiesAsStyleDialog dialog = new ExtractPropertiesAsStyleDialog(attributes);
				if (dialog.ShowDialog() ?? false) {
					XElement resourcesRoot = document.Root.Descendants(document.Root.Name + ".Resources").FirstOrDefault();
					
					if (resourcesRoot == null) {
						resourcesRoot = new XElement(document.Root.Name + ".Resources");
						document.Root.AddFirst(resourcesRoot);
					}
					
					string currentWpfXamlNamespace = resourcesRoot.GetCurrentNamespaces()
						.First(i => CompletionDataHelper.WpfXamlNamespaces.Contains(i));
					
					var selectedAttributes = attributes.Where(item => item.Selected);
					
					resourcesRoot.AddFirst(CreateStyle(dialog.StyleName, selectedItem.Name.LocalName, selectedAttributes, currentWpfXamlNamespace));
					
					selectedAttributes.Where(p => p.Attribute != null).Select(prop => prop.Attribute).Remove();
					selectedAttributes.Where(p => p.Element != null).Select(prop => prop.Element).Remove();
					
					if (!string.IsNullOrEmpty(dialog.StyleName) && selectedItem.Attributes(XName.Get("Style")).Any()) {
						if (MessageService.AskQuestion("The selected control has a style assigned already. " +
						                               "Do you want to replace the style with the newly created style?"))
							selectedItem.SetAttributeValue("Style", "{StaticResource " + dialog.StyleName + "}");
					}
					
					return true;
				}
			}
			
			return false;
		}
		
		static IMember Resolve(XAttribute attribute, ITextEditor editor)
		{
			XamlContext context = CompletionDataHelper.ResolveCompletionContext(editor, default(char), editor.Document.PositionToOffset(attribute.GetLineNumber(), attribute.GetLinePosition()));
			string prefix = context.XmlnsDefinitions.GetKeyByValue(attribute.Name.NamespaceName);
			if (!string.IsNullOrEmpty(prefix))
				prefix += ":";
			var mrr = XamlResolver.Resolve(prefix + attribute.Name.LocalName, context) as MemberResolveResult;
			if (mrr != null)
				return mrr.ResolvedMember;
			
			return null;
		}
		
		static IMember Resolve(XElement element, ITextEditor editor)
		{
			XamlContext context = CompletionDataHelper.ResolveCompletionContext(editor, default(char), editor.Document.PositionToOffset(element.GetLineNumber(), element.GetLinePosition()));
			string prefix = context.XmlnsDefinitions.GetKeyByValue(element.Name.NamespaceName);
			if (!string.IsNullOrEmpty(prefix))
				prefix += ":";
			var mrr = XamlResolver.Resolve(prefix + element.Name.LocalName, context) as MemberResolveResult;
			if (mrr != null)
				return mrr.ResolvedMember;
			
			return null;
		}
		
		// TODO : make the methods xmlns independent
		
		static XElement CreateStyle(string name, string targetType, IEnumerable<PropertyEntry> entries, string currentWpfXamlNamespace)
		{
			XElement style = new XElement(XName.Get("Style", currentWpfXamlNamespace));
			if (!string.IsNullOrEmpty(name))
				style.SetAttributeValue(XName.Get("Key", CompletionDataHelper.XamlNamespace), name);
			style.SetAttributeValue(XName.Get("TargetType"), targetType);
			
			foreach (var entry in entries) {
				if (entry.Attribute != null) {
					if (entry.Member is IEvent)
						style.Add(CreateEventSetter(entry.PropertyName, entry.PropertyValue, currentWpfXamlNamespace));
					else
						style.Add(CreateSetter(entry.PropertyName, entry.PropertyValue, currentWpfXamlNamespace));
				} else {
					if (entry.Member is IEvent)
						style.Add(CreateExtendedEventSetter(entry.PropertyName, entry.PropertyValue, currentWpfXamlNamespace));
					else
						style.Add(CreateExtendedSetter(entry.PropertyName, entry.PropertyValue, currentWpfXamlNamespace));
				}
			}
			
			return style;
		}
		
		static XElement CreateEventSetter(string eventName, string handler, string currentWpfXamlNamespace)
		{
			XElement eventSetter = new XElement(XName.Get("EventSetter", currentWpfXamlNamespace));
			eventSetter.SetAttributeValue(XName.Get("Event"), eventName);
			eventSetter.SetAttributeValue(XName.Get("Handler"), handler);
			return eventSetter;
		}
		
		static XElement CreateSetter(string property, string value, string currentWpfXamlNamespace)
		{
			XElement setter = new XElement(XName.Get("Setter", currentWpfXamlNamespace));
			setter.SetAttributeValue(XName.Get("Property"), property);
			setter.SetAttributeValue(XName.Get("Value"), value);
			return setter;
		}
		
		static XElement CreateExtendedEventSetter(string eventName, string handler, string currentWpfXamlNamespace)
		{
			XElement eventSetter = new XElement(XName.Get("EventSetter", currentWpfXamlNamespace));
			eventSetter.SetAttributeValue(XName.Get("Event"), eventName);
			eventSetter.Add(new XElement(XName.Get("EventSetter.Handler", currentWpfXamlNamespace), handler));
			return eventSetter;
		}
		
		static XElement CreateExtendedSetter(string property, string value, string currentWpfXamlNamespace)
		{
			XElement setter = new XElement(XName.Get("Setter", currentWpfXamlNamespace));
			setter.SetAttributeValue(XName.Get("Property"), property);
			setter.Add(new XElement(XName.Get("Setter.Value", currentWpfXamlNamespace), value));
			return setter;
		}
	}
}
