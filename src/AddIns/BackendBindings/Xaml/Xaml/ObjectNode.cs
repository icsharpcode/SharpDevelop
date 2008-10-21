using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows;

namespace ICSharpCode.Xaml
{
	public class ObjectNode : XamlValue
	{
		internal ObjectNode()
		{
			MemberNodes = new NodeCollection<MemberNode>(this);
		}

		internal ObjectNode(XamlDocument doc, object instance)
			: this()
		{
			Document = doc;
			Instance = instance;
			Type = ReflectionMapper.GetXamlType(instance.GetType());
		}

		public XamlType Type;
		public bool IsRetrieved;
		public NodeCollection<MemberNode> MemberNodes;
		Dictionary<XamlMember, XamlProperty> properties = new Dictionary<XamlMember, XamlProperty>();

		public bool IsDocumentRoot
		{
			get { return Document.Root == this; }
		}

		public XamlProperty Content
		{
			get
			{
				if (Type.ContentProperty != null) {
					return Property(Type.ContentProperty);
				}
				if (Type.IsCollection) {
					return Property(IntristicMember.Items);
				}
				return null;
			}
		}

		public XamlProperty InitializationText
		{
			get { return Property(IntristicMember.InitializationText); }
		}

		public string Name
		{
			get
			{
				var nameProperty = Property(Type.NameProperty);
				if (nameProperty != null && nameProperty.IsSet) {
					return nameProperty.ValueText;
				}
				var xNameProperty = Property(Directive.Name);
				return xNameProperty.ValueText;
			}
			set
			{
				var nameProperty = Property(Type.NameProperty);
				var xNameProperty = Property(Directive.Name);

				if (nameProperty != null) {
					nameProperty.Reset();
				}

				xNameProperty.Set(value);
			}
		}

		public XamlProperty Key
		{
			get { return Property(Directive.Key); }
		}

		public override string ToString()
		{
			return GetType().Name + ": " + Type.Name;
		}

		public override IEnumerable<XamlNode> Nodes()
		{
			return MemberNodes.Cast<XamlNode>();
		}

		protected override void RemoveChild(XamlNode node)
		{
			MemberNodes.Remove(node as MemberNode);
		}

		public MemberNode FindMemberNode(XamlMember member)
		{
			foreach (var memberNode in MemberNodes) {
				if (memberNode.Member == member) {
					return memberNode;
				}
			}
			return null;
		}

		public MemberNode EnsureMemberNode(XamlMember member)
		{
			var memberNode = FindMemberNode(member);
			if (memberNode == null) {
				memberNode = new MemberNode() { Document = Document, Member = member };
				MemberNodes.Add(memberNode);

				if (member.IsReadOnly) {
					var collection = new ObjectNode() {
						Document = Document,
						Type = member.ValueType,
						IsRetrieved = true
					};
					memberNode.SingleValue = collection;
				}
			}
			return memberNode;
		}

		public XamlProperty Property(string name)
		{
			return Property(Type.Member(name));
		}

		public XamlProperty Property(DependencyProperty dp)
		{
			return Property(dp.Name) ?? Property(dp.OwnerType, dp.Name);
		}

		public XamlProperty Property(Type type, string name)
		{
			return Property(ReflectionMapper.GetXamlType(type).Member(name));
		}

		public XamlProperty Property(XamlMember member)
		{
			if (member == null) return null;
			XamlProperty property;
			if (!properties.TryGetValue(member, out property)) {
				property = new XamlProperty(this, member);
				properties[member] = property;
			}
			return property;
		}
	}
}
