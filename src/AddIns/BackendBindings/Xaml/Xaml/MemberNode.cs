using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ICSharpCode.Xaml
{
	public class MemberNode : XamlNode
	{
		internal MemberNode()
		{
			Values = new NodeCollection<XamlValue>(this);
		}

		public XamlMember Member;
		public NodeCollection<XamlValue> Values;

		public ObjectNode ParentObject
		{
			get { return ParentNode as ObjectNode; }
		}

		public XamlProperty Property
		{
			get
			{
				if (ParentObject != null) {
					return ParentObject.Property(Member);
				}
				return null;
			}
		}

		public XamlValue SingleValue
		{
			get
			{
				if (Values.Count == 1) {
					return Values[0];
				}
				return null;
			}
			set
			{
				if (Values.Count == 1) {
					Values[0] = value;
				}
				else {
					Values.Clear();
					Values.Add(value);
				}
			}
		}

		public override string ToString()
		{
			return GetType().Name + ": " + Member.Name;
		}

		public override IEnumerable<XamlNode> Nodes()
		{
			return Values.Cast<XamlNode>();
		}

		protected override void RemoveChild(XamlNode node)
		{
			Values.Remove(node as XamlValue);
		}
	}
}
