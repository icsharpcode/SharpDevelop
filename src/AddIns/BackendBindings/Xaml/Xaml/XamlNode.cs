using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ICSharpCode.Xaml
{
	public abstract class XamlNode : IHasAnnotations
	{
		public XamlNode ParentNode;
		public XamlDocument Document;
		public XObject XmlObject;

		public abstract IEnumerable<XamlNode> Nodes();
		protected abstract void RemoveChild(XamlNode node);

		public bool InDocument
		{
			get { return FindAncestor(n => Document.Root == n) != null; }
		}

		public IEnumerable<XamlNode> Descendants()
		{
			foreach (var node in Nodes()) {
				foreach (var node2 in node.DescendantsAndSelf()) {
					yield return node2;
				}
			}
		}

		public IEnumerable<XamlNode> DescendantsAndSelf()
		{
			foreach (var node in Descendants()) {
				yield return node;
			}
			yield return this;
		}

		public XamlNode FindAncestor(Predicate<XamlNode> predicate)
		{
			var node = this;
			while (node != null) {
				if (predicate(node)) {
					return node;
				}
				node = node.ParentNode;
			}
			return null;
		}

		public void Remove()
		{
			ParentNode.RemoveChild(this);
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
