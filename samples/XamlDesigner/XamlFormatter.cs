using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ICSharpCode.XamlDesigner
{
	public static class XamlFormatter
	{
		public static char IndentChar = ' ';
		public static int Indenation = 2;
		public static int LengthBeforeNewLine = 60;
				
		static StringBuilder sb;
		static int currentColumn;
		static int nextColumn;

		public static string Format(string xaml)
		{
			sb = new StringBuilder();
			currentColumn = 0;
			nextColumn = 0;			

			try {
				var doc = XDocument.Parse(xaml);
				WalkContainer(doc);
				return sb.ToString();
			}
			catch {
				return xaml;
			}
		}

		static void WalkContainer(XContainer node)
		{
			foreach (var c in node.Nodes()) {
				if (c is XElement)  {
					WalkElement(c as XElement);
				} else {
					NewLine();
					Append(c.ToString().Trim());
				}
			}
		}

		static void WalkElement(XElement e)
		{
			NewLine();
			string prefix1 = e.GetPrefixOfNamespace(e.Name.Namespace);
			string name1 = prefix1 == null ? e.Name.LocalName : prefix1 + ":" + e.Name.LocalName;
			Append("<" + name1);

			List<AttributeString> list = new List<AttributeString>();
			int length = name1.Length;

			foreach (var a in e.Attributes()) {
				string prefix2 = e.GetPrefixOfNamespace(a.Name.Namespace);
				var g = new AttributeString() { Name = a.Name, Prefix = prefix2, Value = a.Value };
				list.Add(g);
				length += g.FinalString.Length;
			}

			list.Sort(AttributeComparrer.Instance);

			if (length > LengthBeforeNewLine) {
				nextColumn = currentColumn + 1;
				for (int i = 0; i < list.Count; i++) {
					if (i > 0)  {
						NewLine();
					}
					else {
						Append(" ");
					}
					Append(list[i].FinalString);
				}
				nextColumn -= name1.Length + 2;
			}
			else {
				foreach (var a in list) {
					Append(" " + a.FinalString);
				}
			}

			if (e.Nodes().Count() > 0) {
				Append(">");
				nextColumn += Indenation;

				WalkContainer(e);

				nextColumn -= Indenation;
				NewLine();
				Append("</" + name1 + ">");
			}
			else {
				Append(" />");
			}
		}

		static void NewLine()
		{
			if (sb.Length > 0) {
				sb.AppendLine();			
				sb.Append(new string(' ', nextColumn));
				currentColumn = nextColumn;
			}
		}

		static void Append(string s)
		{
			sb.Append(s);
			currentColumn += s.Length;
		}

		enum AttributeLayout
		{
			X,
			XmlnsMicrosoft,
			Xmlns,
			XmlnsWithClr,
			SpecialOrder,
			ByName,
			Attached,
			WithPrefix
		}

		class AttributeString
		{
			public XName Name;
			public string Prefix;
			public string Value;

			public string LocalName {
				get { return Name.LocalName; }
			}

			public string FinalName {
				get  {
					return Prefix == null ? Name.LocalName : Prefix + ":" + Name.LocalName;
				}
			}

			public string FinalString {
				get {
					return FinalName + "=\"" + Value + "\"";
				}
			}

			public AttributeLayout GetAttributeLayout()
			{
				if (Prefix == "xmlns" || LocalName == "xmlns")  {
					if (Value.StartsWith("http://schemas.microsoft.com")) return AttributeLayout.XmlnsMicrosoft;
					if (Value.StartsWith("clr")) return AttributeLayout.XmlnsWithClr;
					return AttributeLayout.Xmlns;
				}
				if (Prefix == "x") return AttributeLayout.X;
				if (Prefix != null) return AttributeLayout.WithPrefix;
				if (LocalName.Contains(".")) return AttributeLayout.Attached;
				if (AttributeComparrer.SpecialOrder.Contains(LocalName)) return AttributeLayout.SpecialOrder;
				return AttributeLayout.ByName;
			}
		}

		class AttributeComparrer : IComparer<AttributeString>
		{
		    public static AttributeComparrer Instance = new AttributeComparrer();

		    public int Compare(AttributeString a1, AttributeString a2)
		    {
				var y1 = a1.GetAttributeLayout();
				var y2 = a2.GetAttributeLayout();
				if (y1 == y2) {
					if (y1 == AttributeLayout.SpecialOrder) {
						return 
							Array.IndexOf(SpecialOrder, a1.LocalName).CompareTo(
							Array.IndexOf(SpecialOrder, a2.LocalName));
					}
					return a1.FinalName.CompareTo(a2.FinalName);
				}
				return y1.CompareTo(y2);
		    }

			public static string[] SpecialOrder = new string[]  {
				"Name",
				"Content",
				"Command",
				"Executed",
				"CanExecute",
				"Width",
				"Height",
				"Margin",
				"HorizontalAlignment",
				"VerticalAlignment",
				"HorizontalContentAlignment",
				"VerticalContentAlignment",
				"StartPoint",
				"EndPoint",
				"Offset",
				"Color"
			};			
		}
	}
}
