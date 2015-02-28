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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xaml;
using System.Xml;
using System.Xml.XPath;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Contains template related helper methods.
	/// </summary>
	public static class TemplateHelper
	{
		/// <summary>
		/// Gets a <see cref="FrameworkTemplate"/> based on the specified parameters.
		/// </summary>
		/// <param name="xmlElement">The xml element to get template xaml from.</param>
		/// <param name="parentObject">The <see cref="XamlObject"/> to use as source for resources and contextual information.</param>
		/// <returns>A <see cref="FrameworkTemplate"/> based on the specified parameters.</returns>
		public static FrameworkTemplate GetFrameworkTemplate(XmlElement xmlElement, XamlObject parentObject)
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
			
			xmlElement = (XmlElement)xmlElement.CloneNode(true);

			foreach (var dictentry in ns.ToList())
			{
				xmlElement.SetAttribute("xmlns:" + dictentry.Key, dictentry.Value);
			}

			var keyAttrib = xmlElement.GetAttribute("Key", XamlConstants.XamlNamespace);

			if (string.IsNullOrEmpty(keyAttrib)) {
				xmlElement.SetAttribute("Key", XamlConstants.XamlNamespace, "$$temp&&§§%%__");
			}

			var xaml = xmlElement.OuterXml;
			xaml = "<ResourceDictionary xmlns=\"http://schemas.microsoft.com/netfx/2007/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">" + xaml + "</ResourceDictionary>";
			StringReader stringReader = new StringReader(xaml);
			XmlReader xmlReader = XmlReader.Create(stringReader);
			var xamlReader = new XamlXmlReader(xmlReader, parentObject.ServiceProvider.SchemaContext);

			var seti = new XamlObjectWriterSettings();

			var resourceDictionary = new ResourceDictionary();
			var obj = parentObject;
			while (obj != null)
			{
				if (obj.Instance is ResourceDictionary)
				{
					var r = obj.Instance as ResourceDictionary;
					foreach (var k in r.Keys)
					{
						if (!resourceDictionary.Contains(k))
							resourceDictionary.Add(k, r[k]);
					}
				}
				else if (obj.Instance is FrameworkElement)
				{
					var r = ((FrameworkElement)obj.Instance).Resources;
					foreach (var k in r.Keys)
					{
						if (!resourceDictionary.Contains(k))
							resourceDictionary.Add(k, r[k]);
					}
				}

				obj = obj.ParentObject;
			}

			seti.BeforePropertiesHandler = (s, e) =>
			{
				if (seti.BeforePropertiesHandler != null)
				{
					var rr = e.Instance as ResourceDictionary;
					rr.MergedDictionaries.Add(resourceDictionary);
					seti.BeforePropertiesHandler = null;
				}
			};

			var writer = new XamlObjectWriter(parentObject.ServiceProvider.SchemaContext, seti);

			XamlServices.Transform(xamlReader, writer);

			var result = (ResourceDictionary)writer.Result;

			var enr = result.Keys.GetEnumerator();
			enr.MoveNext();
			var rdKey = enr.Current;

			var template = result[rdKey] as FrameworkTemplate;
			
			result.Remove(rdKey);
			return template;
		}

		
		private static Stream GenerateStreamFromString(string s)
		{
			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);
			writer.Write(s);
			writer.Flush();
			stream.Position = 0;
			return stream;
		}
	}
}
