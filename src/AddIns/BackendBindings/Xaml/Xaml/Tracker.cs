using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.Xaml
{
	public class Tracker
	{
		public virtual void Process(DocumentChangedEventArgs e)
		{
			Remove(e.OldNode, e.OldParent);
			Add(e.NewNode);
		}

		public virtual void Add(XamlNode node)
		{
			if (node is MemberNode) {
				Add(node as MemberNode);
			}
			else if (node is ObjectNode) {
				Add(node as ObjectNode);
			}
			else if (node is TextNode) {
				Add(node as TextNode);
			}
		}

		public virtual void Add(MemberNode node)
		{
		}

		public virtual void Add(ObjectNode node)
		{
		}

		public virtual void Add(TextNode node)
		{
		}

		public virtual void Remove(XamlNode node, XamlNode parent)
		{
			if (node is MemberNode) {
				Remove(node as MemberNode, parent as ObjectNode);
			}
			else if (node is ObjectNode) {
				Remove(node as ObjectNode, parent as MemberNode);
			}
			else if (node is TextNode) {
				Remove(node as TextNode, parent as MemberNode);
			}
		}

		public virtual void Remove(MemberNode node, ObjectNode parent)
		{
		}

		public virtual void Remove(ObjectNode node, MemberNode parent)
		{
		}

		public virtual void Remove(TextNode node, MemberNode parent)
		{
		}
	}
}
