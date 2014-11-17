// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using System.Xml.XPath;

namespace ICSharpCode.WpfDesign.XamlDom
{
	public static class TemplateHelper
	{
		public static FrameworkTemplate GetFrameworkTemplate(XmlElement xmlElement)
		{
			var nav = xmlElement.CreateNavigator();

			var ns = new Dictionary<string, string>();
			while (true)
			{
				var nsInScope = nav.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml);
				foreach (var ak in nsInScope)
				{
					if (!ns.ContainsKey(ak.Key) && ak.Key != "")
						ns.Add(ak.Key, ak.Value);
				}
				if (!nav.MoveToParent())
					break;
			}

			foreach (var dictentry in ns)
			{
				xmlElement.SetAttribute("xmlns:" + dictentry.Key, dictentry.Value);
			}
			
			var xaml = xmlElement.OuterXml;
			StringReader stringReader = new StringReader(xaml);
			XmlReader xmlReader = XmlReader.Create(stringReader);
			return (FrameworkTemplate)XamlReader.Load(xmlReader);
		}
	}
}
