using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	public class PositionXmlDocument : XmlDocument
	{
        IXmlLineInfo lineInfo;

        public override XmlElement CreateElement (string prefix, string localName, string namespaceURI)
        {
            return new PositionXmlElement(prefix, localName, namespaceURI, this, lineInfo);
        }

		public override XmlAttribute CreateAttribute(string prefix, string localName, string namespaceURI)
		{
			return new PositionXmlAttribute(prefix, localName, namespaceURI, this, lineInfo);
		}

        public override void Load (XmlReader reader)
        {
            if (reader is IXmlLineInfo) lineInfo = (IXmlLineInfo)reader;
            base.Load(reader);
            lineInfo = null;
        }
    }

	public class PositionXmlElement : XmlElement, IXmlLineInfo
	{
        public PositionXmlElement (string prefix, string localName, string namespaceURI, XmlDocument doc, IXmlLineInfo lineInfo)
            : base(prefix, localName, namespaceURI, doc)
        {
			if (lineInfo != null) {
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
        }
		
		int lineNumber;
		int linePosition;
		bool hasLineInfo;

        public bool HasLineInfo ()
        {
            return hasLineInfo;
        }

        public int LineNumber
        {
            get { return lineNumber; }
        }

        public int LinePosition
        {
            get { return linePosition; }
        }
	}

	public class PositionXmlAttribute : XmlAttribute, IXmlLineInfo
	{
		public PositionXmlAttribute (string prefix, string localName, string namespaceURI, XmlDocument doc, IXmlLineInfo lineInfo)
            : base(prefix, localName, namespaceURI, doc)
        {
            if (lineInfo != null) {
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
        }
		
		int lineNumber;
		int linePosition;
		bool hasLineInfo;

        public bool HasLineInfo ()
        {
            return hasLineInfo;
        }

        public int LineNumber
        {
            get { return lineNumber; }
        }

        public int LinePosition
        {
            get { return linePosition; }
        }
	}
}