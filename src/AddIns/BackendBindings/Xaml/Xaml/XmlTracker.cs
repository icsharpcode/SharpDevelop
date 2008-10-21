using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml.Linq;

namespace ICSharpCode.Xaml
{
	class XmlTracker : Tracker
	{
		public override void Process(DocumentChangedEventArgs e)
		{
			if (e.OldNode != null) {
				if (!TryUpdateMarkupExtension(e.OldParent)) {
					Detach(e.OldNode, e.OldParent);
				}
				RemoveEmptyBranch(e.OldParent);
			}
			if (e.NewNode != null) {
				if (!TryUpdateMarkupExtension(e.OldParent)) {
					Attach(e.NewNode);
				}
			}
		}

		static void Attach(XamlNode node)
		{
			var value = node as XamlValue;
			if (value == null) return;
			if (!node.InDocument) return;

			var objectNode = node as ObjectNode;
			if (objectNode != null && objectNode.IsDocumentRoot) {
				var doc = objectNode.Document;
				if (doc.XmlDocument == null) {
					doc.XmlDocument = new XDocument();
				}
				PrintObject(objectNode, doc.XmlDocument, null, null);
				return;
			}

			if (!TryAttachUsingSiblings(value)) {
				AttachUsingPrint(value);
			}
		}

		static void Detach(XamlNode node, XamlNode nodeParent)
		{
			if (node.XmlObject != null) {
				RemoveXmlObject(node);
			}
			else {
				if (nodeParent.InDocument) {
					var currentXmlElement = nodeParent.FindAncestor(n => n.XmlObject != null).XmlObject as XElement;
					if (currentXmlElement != null) {
						currentXmlElement.RemoveNodes();
					}
				}
			}
		}

		static bool TryAttachUsingSiblings(XamlValue value)
		{
			var values = value.ParentMember.Values;
			if (values.Count > 1) {
				var index = values.IndexOf(value);
				if (index + 1 < values.Count) {
					var before = values[index + 1].XmlObject as XNode;
					PrintValue(value, null, null, before);
				}
				else {
					var after = values[index - 1].XmlObject as XNode;
					PrintValue(value, null, after, null);
				}
				return true;
			}
			return false;
		}

		static void AttachUsingPrint(XamlValue value)
		{
			var memberNode = value.FindAncestor(n => n is MemberNode &&
				(n as MemberNode).ParentObject.XmlObject != null) as MemberNode;
			// TODO: XamlNode.IsAttached
			if (memberNode != null) {
				PrintMember(memberNode, memberNode.ParentObject.XmlObject as XElement, null, null);
			}
		}

		static void RemoveEmptyBranch(XamlNode part)
		{
			XamlNode prev = null;
			while (IsEmpty(part)) {
				prev = part;
				part = part.ParentNode;
			}
			if (prev != null) {
				prev.Remove();
			}
		}

		static bool IsEmpty(XamlNode node)
		{
			var objectNode = node as ObjectNode;
			if (objectNode != null) {
				if (objectNode.MemberNodes.Count == 0 && objectNode.IsRetrieved) {
					return true;
				}
			}
			else {
				var memberNode = node as MemberNode;
				if (memberNode != null) {
					if (memberNode.Values.Count == 0) {
						return true;
					}
				}
			}
			return false;
		}

		static XName CreateMemberName(MemberNode node, bool forAttribute)
		{
			if (node.Member.IsDirective) {
				return Directive.GetDirectiveName(node.Member);
			}
			var type = node.Member.OwnerType;
			if (type.IsAssignableFrom(node.ParentObject.Type)) {
				if (forAttribute) {
					return node.Member.Name;
				}
				type = node.ParentObject.Type;
			}
			var dottedName = type.Name + "." + node.Member.Name;
			var typeNamespace = node.Document.Project.TypeFinder.GetXmlNamespaceForType(type);
			if (forAttribute) {
				var xmlElement = node.FindAncestor(n => n.XmlObject is XElement).XmlObject as XElement;
				if (typeNamespace == xmlElement.Name.Namespace) {
					return dottedName;
				}
			}
			return typeNamespace + dottedName;
		}

		static XName CreateTypeName(ObjectNode node)
		{
			var typeNamespace = node.Document.Project.TypeFinder.GetXmlNamespaceForType(node.Type);
			var typeName = node.Type.Name;
			if (node.Type.IsMarkupExtension && typeName.EndsWith("Extension")) {
				typeName = typeName.Substring(0, typeName.Length - "Extension".Length);
			}
			return typeNamespace + typeName;
		}

		static bool TryPrintMarkupExtension(ObjectNode node, out string text)
		{
			text = null;
			if (!node.Type.IsMarkupExtension) {
				return false;
			}

			StringBuilder sb = new StringBuilder();
			sb.Append("{");

			var typeName = CreateTypeName(node);
			//TODO
			//var typePrefix = GetPrefixOfNamespace(node, typeName.Namespace);
			//sb.Append(typePrefix + ":" + typeName.LocalName);
			sb.Append(typeName.LocalName);

			List<XamlValue> positionalValues = new List<XamlValue>();
			var ctorArgs = node.FindMemberNode(IntristicMember.ConsructorArgs);

			if (ctorArgs != null) {
				positionalValues.AddRange(ctorArgs.Values);
			}
			else {
				var ctor = node.Type.Constructors.FirstOrDefault();
				if (ctor != null && ctor.CorrespondingMembers != null) {
					foreach (var ctorMember in ctor.CorrespondingMembers) {
						var ctorMemberNode = node.FindMemberNode(ctorMember);
						if (ctorMemberNode == null || ctorMemberNode.SingleValue == null) {
							positionalValues.Clear();
							break;
						}
						positionalValues.Add(ctorMemberNode.SingleValue);
					}
				}
			}

			bool first = true;
			HashSet<MemberNode> printed = new HashSet<MemberNode>();

			if (positionalValues.Count > 0) {
				foreach (var value in positionalValues) {
					if (first) {
						sb.Append(" ");
						first = false;
					}
					else {
						sb.Append(", ");
					}

					string valueText;
					if (!TryPrintMarkupExtensionValue(value, out valueText)) {
						return false;
					}
					sb.Append(valueText);

					printed.Add(value.ParentMember);
				}
			}

			foreach (var memberNode in node.MemberNodes) {
				if (printed.Contains(memberNode)) {
					continue;
				}

				if (first) {
					sb.Append(" ");
					first = false;
				}
				else {
					sb.Append(", ");
				}

				var memberName = CreateMemberName(memberNode, true);
				sb.Append(memberName.LocalName);

				sb.Append("=");

				string valueText;
				if (!TryPrintMarkupExtensionValue(memberNode.SingleValue, out valueText)) {
					return false;
				}
				sb.Append(valueText);
			}

			sb.Append("}");
			text = sb.ToString();
			return true;
		}

		static bool TryPrintMarkupExtensionValue(XamlValue value, out string text)
		{
			text = null;
			if (value is TextNode) {
				text = (value as TextNode).Text;
			}
			else if (value is ObjectNode) {
				if (!TryPrintMarkupExtension(value as ObjectNode, out text)) {
					return false;
				}
			}
			return true;
		}

		static XNamespace GetPrefixOfNamespace(XamlNode node, XNamespace ns)
		{
			var xmlElement = node.FindAncestor(n => n.XmlObject is XElement).XmlObject as XElement;
			return xmlElement.GetPrefixOfNamespace(ns);
		}

		static bool TryUpdateMarkupExtension(XamlNode node)
		{
			MemberNode root = null;
			while (node != null) {
				MemberNode memberNode = node as MemberNode;
				if (memberNode != null) {
					var markupExtensionNode = memberNode.SingleValue as ObjectNode;
					if (markupExtensionNode != null && markupExtensionNode.Type.IsMarkupExtension) {
						root = memberNode;
					}
				}
				node = node.ParentNode;
			}

			if (root != null) {
				string text;
				if (TryPrintMarkupExtension(root.SingleValue as ObjectNode, out text)) {
					EnsureXmlAttribute(root, root.ParentObject.XmlObject as XElement).Value = text;
					foreach (var descendant in root.Descendants()) {
						descendant.XmlObject = null;
					}
					return true;
				}
			}

			return false;
		}

		static void PrintObject(ObjectNode node, XContainer currentXmlContainer, XNode after, XNode before)
		{
			if (node.IsRetrieved) {
				foreach (var memberNode in node.MemberNodes) {
					PrintMember(memberNode, currentXmlContainer as XElement, after, before);
				}
			}
			else {
				if (node.XmlObject == null) {
					var typeFinder = node.Document.Project.TypeFinder;
					var xmlElement = new XElement(typeFinder.GetXmlNamespaceForType(node.Type) + node.Type.Name);
					node.XmlObject = xmlElement;

					// preserve usual namespaces
					if (node.IsDocumentRoot) {
						xmlElement.Add(new XAttribute("xmlns", xmlElement.Name.Namespace));
						xmlElement.Add(new XAttribute(XNamespace.Xmlns + "x", XamlConstants.XamlNamespace));
					}

					AddXmlObject(node, currentXmlContainer, after, before);

					foreach (var memberNode in node.MemberNodes) {
						PrintMember(memberNode, xmlElement, after, before);
					}
				}
				else {
					AddXmlObject(node, currentXmlContainer, after, before);
				}
			}
		}

		static void PrintMember(MemberNode node, XElement currentXmlElement, XNode after, XNode before)
		{
			if (node.SingleValue is TextNode && node.Member != IntristicMember.Items) {
				if (node.Member == IntristicMember.InitializationText) {
					PrintValue(node.SingleValue, currentXmlElement, after, before);
				}
				else {
					EnsureXmlAttribute(node, currentXmlElement).Value = (node.SingleValue as TextNode).Text;
				}
			}
			else {
				var me = node.SingleValue as ObjectNode;
				if (me != null && me.Type.IsMarkupExtension) {
					string text;
					if (TryPrintMarkupExtension(me, out text)) {
						EnsureXmlAttribute(node, currentXmlElement).Value = text;
						return;
					}
				}
				var xmlElement = EnsureXmlElement(node, currentXmlElement);
				foreach (var value in node.Values) {
					PrintValue(value, xmlElement, after, before);
				}
			}
		}

		static void PrintValue(XamlValue value, XElement currentXmlElement, XNode after, XNode before)
		{
			var textNode = value as TextNode;
			if (textNode != null) {
				var text = textNode.Text;
				textNode.XmlObject = new XText(text);
				AddXmlObject(textNode, currentXmlElement, after, before);

				if (text.StartsWith(" ") || text.EndsWith(" ")) {
					var xmlElement = currentXmlElement ?? (after != null ? after.Parent : null) ?? before.Parent;
					xmlElement.SetAttributeValue(XamlConstants.XmlSpaceName, "preserve");
				}
			}
			else {
				PrintObject(value as ObjectNode, currentXmlElement, after, before);
			}
		}

		static void AddXmlObject(XamlNode node, XContainer currentXmlContainer, XNode after, XNode before)
		{
			if (after != null) {
				after.AddAfterSelf(node.XmlObject);
			}
			else if (before != null) {
				before.AddBeforeSelf(node.XmlObject);
			}
			else if (node.XmlObject.Document == null) {
				currentXmlContainer.Add(node.XmlObject);
			}
		}

		static void RemoveXmlObject(XamlNode node)
		{
			if (node.XmlObject is XAttribute) {
				(node.XmlObject as XAttribute).Remove();
				node.XmlObject = null;
			}
			else if (node.XmlObject is XNode) {
				(node.XmlObject as XNode).Remove();
			}
			//node.XmlObject = null;
		}

		static XAttribute EnsureXmlAttribute(MemberNode node, XElement currentXmlElement)
		{
			var xmlAttribute = node.XmlObject as XAttribute;
			if (xmlAttribute == null) {
				if (node.XmlObject != null) {
					RemoveXmlObject(node);
				}
				else if (IsContent(node)) {
					currentXmlElement.RemoveNodes();
				}

				xmlAttribute = new XAttribute(CreateMemberName(node, true), "");
				currentXmlElement.Add(xmlAttribute);
				node.XmlObject = xmlAttribute;
			}
			return xmlAttribute;
		}

		static XElement EnsureXmlElement(MemberNode node, XElement currentXmlElement)
		{
			var xmlElement = node.XmlObject as XElement;
			if (xmlElement == null) {
				if (node.XmlObject != null) {
					RemoveXmlObject(node);
				}
				if (IsContent(node)) {
					return currentXmlElement;
				}

				xmlElement = new XElement(CreateMemberName(node, false));
				currentXmlElement.AddFirst(xmlElement);
				node.XmlObject = xmlElement;
			}
			return xmlElement;
		}

		static bool IsContent(MemberNode node)
		{
			return
				node.Member == node.ParentObject.Type.ContentProperty ||
				node.Member == IntristicMember.Items ||
				node.Member == IntristicMember.InitializationText;
		}

		public static XElement GetNamespaceProvider(ObjectNode node)
		{
			var nodeWithXmlElement = node.FindAncestor(n => n.XmlObject is XElement);
			if (nodeWithXmlElement != null) {
				return (nodeWithXmlElement.XmlObject as XElement);
			}
			if (node.Document.XmlDocument != null && node.Document.XmlDocument.Root != null) {
				return node.Document.XmlDocument.Root;
			}
			return new XElement("Empty");
		}
	}
}
