// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Represents a .xaml document.
	/// </summary>
	public sealed class XamlDocument
	{
		XmlDocument _xmlDoc;
		XamlObject _rootElement;
		
		/// <summary>
		/// Gets the root xaml object.
		/// </summary>
		public XamlObject RootElement {
			get { return _rootElement; }
		}
		
		/// <summary>
		/// Gets the object instance created by the root xaml object.
		/// </summary>
		public object RootInstance {
			get { return (_rootElement != null) ? _rootElement.Instance : null; }
		}
		
		/// <summary>
		/// Saves the xaml document into the <paramref name="writer"/>.
		/// </summary>
		public void Save(XmlWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			_xmlDoc.Save(writer);
		}
		
		/// <summary>
		/// Internal constructor, used by XamlParser.
		/// </summary>
		internal XamlDocument(XmlDocument xmlDoc)
		{
			this._xmlDoc = xmlDoc;
		}
		
		/// <summary>
		/// Called by XamlParser to finish initializing the document.
		/// </summary>
		internal void ParseComplete(XamlObject rootElement)
		{
			this._rootElement = rootElement;
		}
	}
}
