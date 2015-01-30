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
	/// Description of XamlXmlWriter.
	/// </summary>
	public class XamlXmlWriter : XmlWriter
	{
		protected XmlWriter xmlWriter;
		
		public XamlXmlWriter(System.Text.StringBuilder stringBuilder)
		{
			this.xmlWriter = XmlWriter.Create(stringBuilder);
		}
		
		#region implemented abstract members of XmlWriter

		public override void WriteStartDocument()
		{
			xmlWriter.WriteStartDocument();
		}

		public override void WriteStartDocument(bool standalone)
		{
			xmlWriter.WriteStartDocument(standalone);
		}

		public override void WriteEndDocument()
		{
			xmlWriter.WriteEndDocument();
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			xmlWriter.WriteDocType(name, pubid, sysid, subset);
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			xmlWriter.WriteStartElement(prefix, localName, ns);
		}

		public override void WriteEndElement()
		{
			xmlWriter.WriteEndElement();
		}

		public override void WriteFullEndElement()
		{
			xmlWriter.WriteFullEndElement();
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			xmlWriter.WriteStartAttribute(prefix, localName, ns);
		}

		public override void WriteEndAttribute()
		{
			xmlWriter.WriteEndAttribute();
		}

		public override void WriteCData(string text)
		{
			xmlWriter.WriteCData(text);
		}

		public override void WriteComment(string text)
		{
			xmlWriter.WriteComment(text);
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			xmlWriter.WriteProcessingInstruction(name, text);
		}

		public override void WriteEntityRef(string name)
		{
			xmlWriter.WriteEntityRef(name);
		}

		public override void WriteCharEntity(char ch)
		{
			xmlWriter.WriteCharEntity(ch);
		}

		public override void WriteWhitespace(string ws)
		{
			xmlWriter.WriteWhitespace(ws);
		}

		public override void WriteString(string text)
		{
			xmlWriter.WriteString(text.Replace("&","&amp;").Replace("\"","&quot;"));
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			xmlWriter.WriteSurrogateCharEntity(lowChar, highChar);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			xmlWriter.WriteChars(buffer, index, count);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			xmlWriter.WriteRaw(buffer, index, count);
		}

		public override void WriteRaw(string data)
		{
			xmlWriter.WriteRaw(data);
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			xmlWriter.WriteBase64(buffer, index, count);
		}

		public override void Close()
		{
			xmlWriter.Close();
		}

		public override void Flush()
		{
			xmlWriter.Flush();
		}

		public override string LookupPrefix(string ns)
		{
			return xmlWriter.LookupPrefix(ns);
		}

		public override WriteState WriteState {
			get {
				return xmlWriter.WriteState;
			}
		}

		#endregion
	}
}
