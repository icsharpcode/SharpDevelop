using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;
using System.Xml;
using System.IO;

namespace ICSharpCode.Xaml
{
	public abstract class XamlProject
	{
		public XamlProject()
		{
			TypeFinder = new XamlTypeFinder(this);
		}

		public XamlTypeFinder TypeFinder;
		public XamlAssembly ProjectAssembly;
		public XamlDocument ApplicationDefinition;
		public List<XamlAssembly> References = new List<XamlAssembly>();
		public List<XamlDocument> Themes = new List<XamlDocument>();
		public Dictionary<Uri, XamlDocument> Documents = new Dictionary<Uri, XamlDocument>();

		XmlResolver xmlResolver = new XmlUrlResolver();

		public IEnumerable<XamlAssembly> AllAssemblies
		{
			get
			{
				if (ProjectAssembly != null) {
					yield return ProjectAssembly;
				}
				foreach (var a in References) {
					yield return a;
				}
			}
		}

		public XamlDocument LoadDocument(string uri)
		{
			var absoluteUri = xmlResolver.ResolveUri(null, uri);
			XamlDocument doc;
			if (!Documents.TryGetValue(absoluteUri, out doc)) {
				doc = new XamlDocument(this);
				var stream = (Stream)xmlResolver.GetEntity(absoluteUri, null, typeof(Stream));
				var text = new StreamReader(stream).ReadToEnd();
				doc.Parse(text);
				doc.BaseUri = absoluteUri;
			}
			return doc;
		}

		public XamlDocument ParseDocument(string text)
		{
			var doc = new XamlDocument(this);
			doc.Parse(text);
			return doc;
		}

		public XamlDocument CreateDocument()
		{
			return new XamlDocument(this);
		}

		public XamlDocument CreateDocument(object root)
		{
			var doc = new XamlDocument(this);
			doc.Root = doc.CreateObject(root);
			return doc;
		}

		public void AddReference(Assembly assembly)
		{
			AddReference(ReflectionMapper.GetXamlAssembly(assembly));
		}

		public void AddReference(XamlAssembly xamlAssembly)
		{
			References.Add(xamlAssembly);
			TypeFinder.RegisterAssembly(xamlAssembly);
		}

		public void RemoveReference(Assembly assembly)
		{
			var xamlAssembly = ReflectionMapper.GetXamlAssembly(assembly);
			References.Remove(xamlAssembly);
		}
	}
}
