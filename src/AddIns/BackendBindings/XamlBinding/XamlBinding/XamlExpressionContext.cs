// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Represents the context of a location in a XAML document.
	/// </summary>
	public sealed class XamlExpressionContext : ExpressionContext
	{
		public static readonly XamlExpressionContext Empty = new XamlExpressionContext(new XmlElementPath(), null, false);

		public XmlElementPath ElementPath { get; private set; }
		public string AttributeName { get; private set; }
		public bool InAttributeValue { get; private set; }

		public XamlExpressionContext(XmlElementPath elementPath, string attributeName, bool inAttributeValue)
		{
			if (elementPath == null)
				throw new ArgumentNullException("elementPath");
			this.ElementPath = elementPath;
			this.AttributeName = attributeName;
			this.InAttributeValue = inAttributeValue;
		}

		public override bool ShowEntry(ICompletionEntry o)
		{
			return true;
		}

		public override string ToString()
		{
			StringBuilder b = new StringBuilder();
			b.Append("[XamlExpressionContext ");
			for (int i = 0; i < ElementPath.Elements.Count; i++) {
				if (i > 0) b.Append(">");
				if (!string.IsNullOrEmpty(ElementPath.Elements[i].Prefix)) {
					b.Append(ElementPath.Elements[i].Prefix);
					b.Append(':');
				}
				b.Append(ElementPath.Elements[i].Name);
			}
			if (AttributeName != null) {
				b.Append(" AttributeName=");
				b.Append(AttributeName);
				if (InAttributeValue) {
					b.Append(" InAttributeValue");
				}
			}
			b.Append("]");
			return b.ToString();
		}
	}
}
