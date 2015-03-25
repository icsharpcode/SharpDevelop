/*
 * Created by SharpDevelop.
 * User: jkuehner
 * Date: 30.01.2015
 * Time: 09:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// A special XamlXmlWriter wich fixes &amp; and &quot; in MarkupExtensions where not correctly handled.
	/// </summary>
	public class XamlXmlWriter : XmlWriter
	{
		/// <summary>
		/// The <see cref="XmlWriter"/> instance used internally.
		/// </summary>
		protected readonly XmlWriter xmlWriter;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="XamlXmlWriter"/> class.
		/// </summary>
		/// <param name="stringBuilder">The <see cref="System.Text.StringBuilder"/> to which to write to.</param>
		public XamlXmlWriter(System.Text.StringBuilder stringBuilder)
		{
			this.xmlWriter = XmlWriter.Create(stringBuilder);
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="XamlXmlWriter"/> class.
		/// </summary>
		/// <param name="stringBuilder">The <see cref="System.Text.StringBuilder"/> to which to write to.</param>
		/// <param name="settings">The <see cref="XmlWriterSettings"/> object used to configure the new <see cref="XamlXmlWriter"/> instance.</param>
		public XamlXmlWriter(System.Text.StringBuilder stringBuilder, XmlWriterSettings settings)
		{
			this.xmlWriter = XmlWriter.Create(stringBuilder, settings);
		}
		
		#region implemented abstract members of XmlWriter

		/// <inheritdoc/>
		public override void WriteStartDocument()
		{
			xmlWriter.WriteStartDocument();
		}

		/// <inheritdoc/>
		public override void WriteStartDocument(bool standalone)
		{
			xmlWriter.WriteStartDocument(standalone);
		}

		/// <inheritdoc/>
		public override void WriteEndDocument()
		{
			xmlWriter.WriteEndDocument();
		}

		/// <inheritdoc/>
		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			xmlWriter.WriteDocType(name, pubid, sysid, subset);
		}

		/// <inheritdoc/>
		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			xmlWriter.WriteStartElement(prefix, localName, ns);
		}

		/// <inheritdoc/>
		public override void WriteEndElement()
		{
			xmlWriter.WriteEndElement();
		}

		/// <inheritdoc/>
		public override void WriteFullEndElement()
		{
			xmlWriter.WriteFullEndElement();
		}

		/// <inheritdoc/>
		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			xmlWriter.WriteStartAttribute(prefix, localName, ns);
		}

		/// <inheritdoc/>
		public override void WriteEndAttribute()
		{
			xmlWriter.WriteEndAttribute();
		}

		/// <inheritdoc/>
		public override void WriteCData(string text)
		{
			xmlWriter.WriteCData(text);
		}

		/// <inheritdoc/>
		public override void WriteComment(string text)
		{
			xmlWriter.WriteComment(text);
		}

		/// <inheritdoc/>
		public override void WriteProcessingInstruction(string name, string text)
		{
			xmlWriter.WriteProcessingInstruction(name, text);
		}

		/// <inheritdoc/>
		public override void WriteEntityRef(string name)
		{
			xmlWriter.WriteEntityRef(name);
		}

		/// <inheritdoc/>
		public override void WriteCharEntity(char ch)
		{
			xmlWriter.WriteCharEntity(ch);
		}

		/// <inheritdoc/>
		public override void WriteWhitespace(string ws)
		{
			xmlWriter.WriteWhitespace(ws);
		}

		/// <inheritdoc/>
		public override void WriteString(string text)
		{
			xmlWriter.WriteString(text.Replace("&","&amp;").Replace("\"","&quot;"));
		}

		/// <inheritdoc/>
		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			xmlWriter.WriteSurrogateCharEntity(lowChar, highChar);
		}

		/// <inheritdoc/>
		public override void WriteChars(char[] buffer, int index, int count)
		{
			xmlWriter.WriteChars(buffer, index, count);
		}

		/// <inheritdoc/>
		public override void WriteRaw(char[] buffer, int index, int count)
		{
			xmlWriter.WriteRaw(buffer, index, count);
		}

		/// <inheritdoc/>
		public override void WriteRaw(string data)
		{
			xmlWriter.WriteRaw(data);
		}

		/// <inheritdoc/>
		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			xmlWriter.WriteBase64(buffer, index, count);
		}

		/// <inheritdoc/>
		public override void Close()
		{
			xmlWriter.Close();
		}

		/// <inheritdoc/>
		public override void Flush()
		{
			xmlWriter.Flush();
		}

		/// <inheritdoc/>
		public override string LookupPrefix(string ns)
		{
			return xmlWriter.LookupPrefix(ns);
		}

		/// <inheritdoc/>
		public override WriteState WriteState {
			get {
				return xmlWriter.WriteState;
			}
		}

		#endregion
	}
}
