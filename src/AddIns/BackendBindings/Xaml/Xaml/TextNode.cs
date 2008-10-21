using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ICSharpCode.Xaml
{
	public class TextNode : XamlValue
	{
		internal TextNode()
		{
		}

		public string Text;

		public override string ToString()
		{
			return GetType().Name + ": " + Text;
		}

		public override IEnumerable<XamlNode> Nodes()
		{
			yield break;
		}

		protected override void RemoveChild(XamlNode node)
		{
		}
	}
}
