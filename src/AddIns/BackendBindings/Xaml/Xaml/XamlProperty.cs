using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Markup;

namespace ICSharpCode.Xaml
{
	public class XamlProperty : IHasAnnotations
	{
		internal XamlProperty(ObjectNode objectNode, XamlMember member)
		{
			this.objectNode = objectNode;
			this.member = member;
			this.context = new XamlContext(this);
		}

		ObjectNode objectNode;
		XamlMember member;
		XamlContext context;

		public event EventHandler IsSetChanged;
		public event EventHandler ValueChanged;

		public ObjectNode Object
		{
			get { return objectNode; }
		}

		public XamlMember Member
		{
			get { return member; }
		}

		public XamlContext XamlContext
		{
			get { return context; }
		}

		public bool IsSet
		{
			get { return FindMemberNode() != null; }
		}

		public XamlValue Value
		{
			get
			{
				var memberNode = FindMemberNode();
				if (memberNode != null) {
					return memberNode.SingleValue;
				}
				return null;
			}
		}

		public XamlType ValueType
		{
			get
			{
				if (member == IntristicMember.InitializationText) {
					return Object.Type;
				}
				return member.ValueType;
			}
		}

		public string ValueText
		{
			get
			{
				var textValue = Value as TextNode;
				if (textValue != null) {
					return textValue.Text;
				}
				return null;
			}
		}

		public object ValueOnInstance
		{
			get
			{
				// TODO: return real value
				if (Member.IsEvent) {
					return ValueText;
				}
				return Runtime.GetValue(Object.Instance, Member);
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public NodeCollection<XamlValue> Collection
		{
			get
			{
				var items = EnsureItems();
				if (items != null) {
					return items.Values;
				}
				return null;
			}
		}

		public ObjectNode Add(object key, object value)
		{
			var newObject = AddObject(value);
			newObject.Property(Directive.Key).Set(key);
			return newObject;
		}

		public ObjectNode AddObject(object value)
		{
			return Add(value) as ObjectNode;
		}

		public XamlValue Add(object value)
		{
			var newValue = PrepareValueForCollection(value);
			EnsureItems().Values.Add(newValue);
			return newValue;
		}

		public XamlValue Insert(int index, object value)
		{
			var newValue = PrepareValueForCollection(value);
			EnsureItems().Values.Insert(index, newValue);
			return newValue;
		}

		public override string ToString()
		{
			return GetType().Name + ": " + Member.Name;
		}

		public ObjectNode SetObject(object value)
		{
			return Set(value) as ObjectNode;
		}

		public TextNode SetText(object value)
		{
			return Set(value) as TextNode;
		}

		public XamlValue Set(object value)
		{
			if (member.ValueType.IsCollection && !member.ValueType.HasTextSyntax) {
				return AddObject(value);
			}
			else {
				var newValue = PrepareValue(value);
				EnsureMemberNode().SingleValue = newValue;
				return newValue;
			}
		}

		public void Reset()
		{
			var memberNode = FindMemberNode();
			if (memberNode != null) {
				memberNode.Remove();
			}
		}

		public XamlValue PrepareValue(object value)
		{
			if (value is XamlValue) {
				return value as XamlValue;
			}
			if (value == null) {
				return new ObjectNode(objectNode.Document, new NullExtension());
			}

			string text = value as string;
			if (text == null) {
				Runtime.TryConvertToText(context, value, out text);
			}
			if (text != null) {
				var namespaceProvider = XmlTracker.GetNamespaceProvider(objectNode);
				var valueNode = objectNode.Document.Parser.CreateValueFromAttributeText(text, namespaceProvider);
				valueNode.Document = objectNode.Document;
				return valueNode;
			}

			return new ObjectNode(objectNode.Document, value);
		}

		public XamlValue PrepareValueForCollection(object value)
		{
			var valueNode = PrepareValue(value);
			var textNode = valueNode as TextNode;
			if (textNode != null) {
				var itemsNode = EnsureItems();
				if (itemsNode.ParentObject != null &&
					itemsNode.ParentObject.Type.ContentWrappers.Where(
					t => t.ContentProperty != null &&
					t.ContentProperty.ValueType == IntristicType.String).Any()) {
					return textNode;
				}

				var wrapperType = Runtime.GetWrapperTypeForInitializationText(value);
				var wrapperNode = new ObjectNode() {
					Document = objectNode.Document,
					Type = wrapperType,
					Instance = value
				};
				wrapperNode.InitializationText.Set(textNode);
				return wrapperNode;
			}
			return valueNode;
		}

		public MemberNode FindMemberNode()
		{
			return objectNode.FindMemberNode(member);
		}

		public MemberNode EnsureMemberNode()
		{
			return objectNode.EnsureMemberNode(member);
		}

		MemberNode EnsureItems()
		{
			if (member == IntristicMember.Items ||
				member == IntristicMember.ConsructorArgs ||
				member == IntristicMember.DirectiveChildren) {
				return EnsureMemberNode();
			}

			var memberNode = EnsureMemberNode();
			if (memberNode.SingleValue == null) {
				var collection = new ObjectNode() {
					Document = objectNode.Document,
					Type = member.ValueType,
					IsRetrieved = true
				};
				memberNode.SingleValue = collection;
			}
			return (memberNode.SingleValue as ObjectNode).EnsureMemberNode(IntristicMember.Items);
		}

		#region IHasAnnotations Members

		Dictionary<Type, object> annotations = new Dictionary<Type, object>();

		public void AnnotateWith<T>(T annotation) where T : class
		{
			annotations[typeof(T)] = annotation;
		}

		public T GetAnnotation<T>() where T : class
		{
			object result;
			if (annotations.TryGetValue(typeof(T), out result)) {
				return (T)result;
			}
			return default(T);
		}

		#endregion
	}
}
