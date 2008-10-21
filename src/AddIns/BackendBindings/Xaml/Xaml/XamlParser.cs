using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Collections;

namespace ICSharpCode.Xaml
{
	public class XamlParser
	{
		public XamlParser(XamlDocument doc)
		{
			Document = doc;
		}

		public XamlDocument Document { get; private set; }

		public XamlTypeFinder TypeFinder
		{
			get { return Document.Project.TypeFinder; }
		}

		public ObjectNode CreateObjectNodeFromXmlElement(XElement xmlObjectElement,
			bool parentPreservesXmlSpace)
		{
			//type
			XamlType objectType = TypeFinder.FindType(xmlObjectElement.Name);
			if (objectType == null) {
				objectType = TypeFinder.FindExtensionType(xmlObjectElement.Name);
			}
			if (objectType == null) {
				throw new XamlException("Unknown element type");
			}

			//xml:space
			bool preserveChildXmlSpace = parentPreservesXmlSpace;
			var spaceAttribute = xmlObjectElement.Attribute(XamlConstants.XmlSpaceName);
			if (spaceAttribute != null) {
				preserveChildXmlSpace = spaceAttribute.Value == "preserve";
			}

			//x:Code, x:XData
			if (objectType == IntristicType.Code || objectType == IntristicType.XData) {
				ObjectNode literalResult = new ObjectNode();
				literalResult.Type = objectType;
				var textNode = new TextNode() { Text = xmlObjectElement.Value };
				var memberNode = new MemberNode();
				memberNode.Values.Add(textNode);
				literalResult.MemberNodes.Add(memberNode);
				literalResult.XmlObject = xmlObjectElement;
				return literalResult;
			}

			List<XamlNode> convertedChildNodes = new List<XamlNode>();
			List<ObjectNode> codeItems = new List<ObjectNode>();

			//conversion

			foreach (var node in xmlObjectElement.Nodes()) {
				var childElement = node as XElement;
				if (childElement != null) {
					if (IsXamlName(childElement.Name)) {
						var convertedObject = CreateObjectNodeFromXmlElement(childElement, preserveChildXmlSpace);
						if (convertedObject.Type == IntristicType.Code) {
							codeItems.Add(convertedObject);
						}
						else {
							convertedChildNodes.Add(convertedObject);
						}
					}
					else if (GetDottedXamlName(childElement.Name) != null) {
						var convertedMember = CreateMemberNodeFromXmlElement(childElement, objectType, preserveChildXmlSpace);
						foreach (var objectNode in convertedMember.Values.OfType<ObjectNode>().ToArray()) {
							if (objectNode.Type == IntristicType.Code) {
								codeItems.Add(objectNode);
								convertedMember.Values.Remove(objectNode);
							}
						}
						convertedChildNodes.Add(convertedMember);
					}
					else {
						throw new XamlException("Invalid element name syntax");
					}
				}
				else {
					AddText(convertedChildNodes, node);
				}
			}

			if (codeItems.Count > 0) {
				MemberNode directiveChildren = new MemberNode();
				directiveChildren.Member = IntristicMember.DirectiveChildren;
				foreach (var item in codeItems) {
					directiveChildren.Values.Add(item);
					//item.ParentMember = directiveChildren;
				}

			}

			//whitespace removal

			List<XamlNode> strippedChildNodes = new List<XamlNode>();
			int n = convertedChildNodes.Count;
			bool hasTextNodes = false;
			bool hasContent = false;

			for (int i = 0; i < n; i++) {
				var node = convertedChildNodes[i];
				var textNode = node as TextNode;
				if (textNode != null && IsCollapsibleString(textNode.Text)) {
					bool prevMember = i - 1 > 0 && convertedChildNodes[i - 1] is MemberNode;
					bool nextMember = i + 1 < n && convertedChildNodes[i + 1] is MemberNode;
					if (i == 0 && nextMember) continue;
					if (prevMember && nextMember) continue;
					if (i == n - 1 && prevMember && (hasTextNodes || hasContent)) continue;
				}

				strippedChildNodes.Add(convertedChildNodes[i]);

				if (textNode != null) {
					hasTextNodes = true;
				}
				var memberNode = node as MemberNode;
				if (memberNode != null && memberNode.Member == objectType.ContentProperty) {
					hasContent = true;
				}
			}

			//content wrapping

			XamlMember contentMember = null;
			XamlType contentMemberType = null;
			if (objectType.ContentProperty != null) {
				contentMember = objectType.ContentProperty;
				contentMemberType = contentMember.ValueType;
			}
			else {
				contentMember = IntristicMember.Items;
				contentMemberType = objectType;
			}

			List<MemberNode> attributeMembers = new List<MemberNode>();
			foreach (var xmlAttribute in xmlObjectElement.Attributes()) {
				if (xmlAttribute.IsNamespaceDeclaration) continue;
				var memberNode = CreateMemberNodeFromXmlAttribute(xmlAttribute, objectType, xmlObjectElement);
				attributeMembers.Add(memberNode);
			}

			List<MemberNode> members = new List<MemberNode>();

			bool initFromText =
				strippedChildNodes.Count(node => node is TextNode) == 1 &&
				strippedChildNodes.All(node => node is TextNode ||
				node is MemberNode && (node as MemberNode).Member == IntristicMember.DirectiveChildren) &&
				attributeMembers.All(node => node.Member == Directive.Key ||
				node.Member == Directive.Uid) &&

				// differ from spec (spec bug?)
				//(contentMember.HasTextSyntax || objectType.HasTextSyntax);
				objectType.HasTextSyntax;

			if (initFromText) {
				var initTextMember = new MemberNode();
				initTextMember.Member = IntristicMember.InitializationText;
				initTextMember.Values.Add(strippedChildNodes.OfType<TextNode>().First());
				members.Add(initTextMember);
			}
			else {
				members.AddRange(attributeMembers);
				List<XamlValue> npChildren = new List<XamlValue>();

				foreach (var node in strippedChildNodes) {
					if (node is MemberNode) {
						if (npChildren.Count > 0) {
							var memberNode = CreateMemberNodeFromContent(contentMember, contentMemberType, npChildren, preserveChildXmlSpace);
							members.Add(memberNode);
							npChildren.Clear();
						}
						members.Add(node as MemberNode);
					}
					else {
						npChildren.Add(node as XamlValue);
					}
				}

				if (npChildren.Count > 0) {
					var memberNode = CreateMemberNodeFromContent(contentMember, contentMemberType, npChildren, preserveChildXmlSpace);
					members.Add(memberNode);
				}
			}

			ObjectNode result = new ObjectNode();
			result.Type = objectType;
			foreach (var memberNode in members) {
				result.MemberNodes.Add(memberNode);
				//memberNode.ParentObject = result;
			}
			result.XmlObject = xmlObjectElement;
			return result;
		}

		public MemberNode CreateMemberNodeFromXmlAttribute(XAttribute xmlAttribute,
			XamlType objectType, XElement namespaceProvider)
		{
			XamlMember member;
			if (IsXamlName(xmlAttribute.Name)) {
				member = objectType.Member(xmlAttribute.Name.LocalName);
				if (member == null) {
					member = Directive.GetDirective(xmlAttribute.Name);
				}
				if (member == null) {
					throw new XamlException("Unknown member");
				}
			}
			else {
				var dottedName = GetDottedXamlName(xmlAttribute.Name);
				if (dottedName != null) {
					var attributeNamespace =
						xmlAttribute.Name.Namespace != XNamespace.None ?
						xmlAttribute.Name.Namespace :
						namespaceProvider.Name.Namespace;

					var definingType = TypeFinder.FindType(attributeNamespace + dottedName.TypeName);
					if (definingType == null) {
						throw new XamlException("Unknown type");
					}
					member = definingType.Member(dottedName.MemberName);
					if (member == null) {
						throw new XamlException("Unknown member");
					}
				}
				else {
					throw new XamlException("Invalid attribute syntax");
				}
			}

			XamlValue attributeValue = CreateValueFromAttributeText(xmlAttribute.Value, namespaceProvider);

			MemberNode result = new MemberNode();
			result.Member = member;
			result.Values.Add(attributeValue);
			//attributeValue.ParentMember = result;
			result.XmlObject = xmlAttribute;
			return result;
		}

		public XamlValue CreateValueFromAttributeText(string valueText, XElement namespaceProvider)
		{
			if (valueText.StartsWith("{}")) {
				return new TextNode() { Text = valueText.Substring(2) };
			}
			else if (valueText.StartsWith("{")) {
				return CreateObjectNodeFromMarkupExtensionInAttribute(valueText, namespaceProvider);
			}
			return new TextNode() { Text = valueText };
		}

		public MemberNode CreateMemberNodeFromXmlElement(XElement xmlMemberElement,
			XamlType containingType, bool parentPreservesXmlSpace)
		{
			if (xmlMemberElement.HasAttributes) {
				if (xmlMemberElement.Attributes().Count() > 1 ||
					Directive.GetDirective(xmlMemberElement.FirstAttribute.Name) !=
					Directive.Uid) {
					throw new XamlException("Member elements cannot contain attributes");
				}
			}

			var dottedName = GetDottedXamlName(xmlMemberElement.Name);
			XamlType ownerType = TypeFinder.FindType(xmlMemberElement.Name.Namespace + dottedName.TypeName);
			if (ownerType == null) {
				throw new XamlException("Unknown element type");
			}

			XamlMember resolvedMember = ownerType.Member(dottedName.MemberName);
			if (resolvedMember == null || resolvedMember.AllowedLocation != AllowedLocation.Any) {
				throw new XamlException("Member not found");
			}

			List<XamlValue> convertedChildNodes = new List<XamlValue>();

			foreach (var node in xmlMemberElement.Nodes()) {
				var childElement = node as XElement;
				if (childElement != null) {
					if (IsXamlName(childElement.Name)) {
						var convertedObject = CreateObjectNodeFromXmlElement(childElement, parentPreservesXmlSpace);
						convertedChildNodes.Add(convertedObject);
					}
					else if (GetDottedXamlName(childElement.Name) != null) {
						throw new XamlException("Member elements may not be nested directly inside of another member element");
					}
					else {
						throw new XamlException("Invalid element name syntax");
					}
				}
				else {
					AddText(convertedChildNodes, node);
				}
			}

			var result = CreateMemberNodeFromContent(resolvedMember, resolvedMember.ValueType, convertedChildNodes, parentPreservesXmlSpace);
			result.XmlObject = xmlMemberElement;
			return result;
		}

		public MemberNode CreateMemberNodeFromContent(XamlMember containingMember,
			XamlType memberType, List<XamlValue> childNodes, bool preserveXmlSpace)
		{
			if (!preserveXmlSpace) {
				if (memberType.IsWhitespaceSignificantCollection) {
					TextNode prevTextNode = null;
					for (int i = 0; i < childNodes.Count; i++) {
						var node = childNodes[i];
						var textNode = node as TextNode;
						if (textNode != null) {
							textNode.Text = CollapseWhitespace(textNode.Text);
							if (prevTextNode == null) {
								textNode.Text = textNode.Text.TrimStart();
								prevTextNode = textNode;
							}
						}
						else {
							var objectNode = node as ObjectNode;
							if (objectNode != null && objectNode.Type.TrimSurroundingWhitespace) {
								var afterTextNode = i + 1 < childNodes.Count ? childNodes[i + 1] as TextNode : null;
								if (prevTextNode != null) {
									prevTextNode.Text = prevTextNode.Text.TrimEnd();
								}
								if (afterTextNode != null) {
									afterTextNode.Text = afterTextNode.Text.TrimStart();
								}
							}
						}
					}
					if (prevTextNode != null) {
						prevTextNode.Text = prevTextNode.Text.TrimEnd();
					}
				}
				// differ from spec (spec bug?)
				else {
					foreach (var textNode in childNodes.OfType<TextNode>()) {
						textNode.Text = CollapseWhitespace(textNode.Text).Trim();
					}
				}
			}

			childNodes = childNodes.Where(node => node is ObjectNode || (node as TextNode).Text.Length > 0).ToList();

			List<XamlValue> outputValues = new List<XamlValue>();
			var singleObjectNode = childNodes.FirstOrDefault() as ObjectNode;
			var useSingleObjectNode = singleObjectNode != null && memberType.IsAssignableFrom(singleObjectNode.Type);

			if (memberType.IsCollection && !useSingleObjectNode) {
				var retrievedContentMember = new MemberNode();
				retrievedContentMember.Member = IntristicMember.Items;
				foreach (var value in childNodes) {
					retrievedContentMember.Values.Add(value);
				}

				var retrievedValue = new ObjectNode();
				retrievedValue.Type = memberType;
				retrievedValue.MemberNodes.Add(retrievedContentMember);
				retrievedValue.IsRetrieved = true;

				outputValues.Add(retrievedValue);
			}
			else {
				outputValues.AddRange(childNodes);
			}

			MemberNode result = new MemberNode();
			result.Member = containingMember;
			foreach (var value in outputValues) {
				result.Values.Add(value);
				//value.ParentMember = result;
			}
			return result;
		}

		public static XName GetTypeName(string typeNameWithPrefix, XElement namespaceProvider)
		{
			var prefixedName = GetPrefixedName(typeNameWithPrefix);
			if (prefixedName == null) {
				throw new XamlException("Bad type extension name");
			}

			XNamespace typeNamespace = null;
			if (prefixedName.Prefix == null) {
				typeNamespace = namespaceProvider.Name.Namespace;
			}
			else {
				typeNamespace = namespaceProvider.GetNamespaceOfPrefix(prefixedName.Prefix);
				if (typeNamespace == null) {
					throw new XamlException("Unrecognized namespace prefix");
				}
			}

			return typeNamespace + prefixedName.LocalName;
		}

		public ObjectNode CreateObjectNodeFromMarkupExtensionInAttribute(string attributeText,
			XElement namespaceProvider)
		{
			var ast = MarkupExtensionParser.Parse(attributeText);

			var prefixedName = GetPrefixedName(ast.TypeName);
			if (prefixedName == null) {
				throw new XamlException("Bad type extension name");
			}

			XNamespace typeNamespace = null;
			if (prefixedName.Prefix == null) {
				typeNamespace = namespaceProvider.Name.Namespace;
			}
			else {
				typeNamespace = namespaceProvider.GetNamespaceOfPrefix(prefixedName.Prefix);
				if (typeNamespace == null) {
					throw new XamlException("Unrecognized namespace prefix");
				}
			}

			XName typeName = typeNamespace + prefixedName.LocalName;
			XamlType extensionType = TypeFinder.FindExtensionType(typeName);
			if (extensionType == null || !IntristicType.MarkupExtension.IsAssignableFrom(extensionType)) {
				extensionType = TypeFinder.FindType(typeName);
			}
			if (extensionType == null || !IntristicType.MarkupExtension.IsAssignableFrom(extensionType)) {
				throw new XamlException("Unknown markup extension");
			}

			List<MemberNode> namedMembers = new List<MemberNode>();
			foreach (var namedArg in ast.NamedArgs) {
				var memberName = GetPrefixedName(namedArg.Key);
				if (memberName == null) {
					throw new XamlException("Bad member name");
				}

				XNamespace memberNamespace;
				if (prefixedName.Prefix == null) {
					memberNamespace = namespaceProvider.Name.Namespace;
				}
				else {
					memberNamespace = namespaceProvider.GetNamespaceOfPrefix(prefixedName.Prefix);
					if (typeNamespace == null) {
						throw new XamlException("Unrecognized namespace prefix");
					}
				}

				XamlMember member = null;
				if (IsXamlName(memberName.LocalName)) {
					if (memberNamespace != typeNamespace) {
						throw new XamlException("Unknown member");
					}
					member = extensionType.Member(memberName.LocalName);
				}
				else {
					var dottedName = GetDottedXamlName(memberNamespace + memberName.LocalName);
					if (dottedName != null) {
						var ownerType = TypeFinder.FindType(memberNamespace + dottedName.TypeName);
						if (ownerType == null) {
							throw new XamlException("Unknown type");
						}
						member = ownerType.Member(dottedName.MemberName);
					}
				}

				if (member == null) {
					throw new XamlException("Unknown member");
				}

				var memberValue = CreateValueFromAttributeText(namedArg.Value, namespaceProvider);

				var namedMember = new MemberNode();
				namedMember.Member = member;
				namedMember.Values.Add(memberValue);
				namedMembers.Add(namedMember);
			}

			List<string> positionalArgs = ast.PositionalArgs;
			MemberNode positionalArgsMember = null;

			if (positionalArgs.Count > 0) {
				Constructor constructorInfo = null;
				foreach (var extensionConstructor in extensionType.Constructors) {
					if (extensionConstructor.Arguments.Length == positionalArgs.Count) {
						constructorInfo = extensionConstructor;
						break;
					}
				}

				if (constructorInfo == null) {
					throw new XamlException(
						string.Format("No constructor for type '{0}' has {1} parameters", extensionType.Name, positionalArgs.Count));
				}

				List<XamlValue> positionalArgValues = new List<XamlValue>();

				for (int i = 0; i < positionalArgs.Count; i++) {
					var positionalArg = positionalArgs[i];
					var argumentType = constructorInfo.Arguments.ElementAt(i);
					var argValue = CreateValueFromAttributeText(positionalArg, namespaceProvider);
					positionalArgValues.Add(argValue);
				}

				if (!MapPositionalArgsToNamedMembers(constructorInfo, positionalArgValues, namedMembers)) {
					positionalArgsMember = new MemberNode();
					positionalArgsMember.Member = IntristicMember.ConsructorArgs;
					foreach (var value in positionalArgValues) {
						positionalArgsMember.Values.Add(value);
						//value.ParentMember = positionalArgsMember;
					}
				}
			}

			List<MemberNode> allArgs = new List<MemberNode>();
			allArgs.AddRange(namedMembers);
			if (positionalArgsMember != null) {
				allArgs.Add(positionalArgsMember);
			}

			ObjectNode result = new ObjectNode();
			result.Type = extensionType;
			foreach (var arg in allArgs) {
				result.MemberNodes.Add(arg);
				//arg.ParentObject = result;
			}
			return result;
		}

		static bool MapPositionalArgsToNamedMembers(Constructor ctor, List<XamlValue> positionalArgValues, List<MemberNode> namedMembers)
		{
			if (ctor.CorrespondingMembers != null) {
				for (int i = 0; i < positionalArgValues.Count; i++) {
					var memberNode = new MemberNode();
					memberNode.Member = ctor.CorrespondingMembers[i];
					memberNode.Values.Add(positionalArgValues[i]);
					namedMembers.Add(memberNode);
				}
				return true;
			}
			return false;
		}

		static Regex XamlNameRegex = new Regex(@"^[\w]*$");
		static Regex DottedXamlNameRegex = new Regex(@"^([\w]*)\.([\w]*)$");

		static bool IsXamlName(XName name)
		{
			return XamlNameRegex.IsMatch(name.LocalName);
		}

		static DottedXamlName GetDottedXamlName(XName name)
		{
			var m = DottedXamlNameRegex.Match(name.LocalName);
			if (m.Success) {
				return new DottedXamlName() {
					TypeName = m.Groups[1].Value,
					MemberName = m.Groups[2].Value
				};
			}
			return null;
		}

		static PrefixedName GetPrefixedName(string s)
		{
			var result = new PrefixedName();
			var parts = s.Split(':');
			if (parts.Length == 1) {
				result.LocalName = parts[0];
			}
			else {
				result.Prefix = parts[0];
				result.LocalName = parts[1];
			}
			return result;
		}

		public static bool IsCollapsibleChar(char c)
		{
			return char.IsWhiteSpace(c);
		}

		public static bool IsCollapsibleString(string s)
		{
			return s == null || s.Trim().Length == 0;
		}

		public static string CollapseWhitespace(string s)
		{
			StringBuilder b = new StringBuilder(s.Length);
			bool firstSpace = true;
			foreach (var c in s) {
				if (char.IsWhiteSpace(c)) {
					if (firstSpace) b.Append(" ");
					firstSpace = false;
					continue;
				}
				b.Append(c);
				firstSpace = true;
			}
			return b.ToString();
		}

		static void AddText(IList convertedChildNodes, XNode node)
		{
			var xmlTextNode = node as XText;
			if (xmlTextNode != null) {
				TextNode textNode = null;
				if (convertedChildNodes.Count > 0) {
					textNode = convertedChildNodes[convertedChildNodes.Count - 1] as TextNode;
				}
				if (textNode != null) {
					textNode.Text += xmlTextNode.Value;
				}
				else {
					textNode = new TextNode() { Text = xmlTextNode.Value };
					convertedChildNodes.Add(textNode);
				}
			}
		}

		class DottedXamlName
		{
			public string TypeName;
			public string MemberName;
		}

		class PrefixedName
		{
			public string Prefix;
			public string LocalName;
		}
	}
}
