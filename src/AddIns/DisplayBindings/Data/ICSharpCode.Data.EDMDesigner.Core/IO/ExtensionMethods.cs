// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.IO
{
    public static class XElements
    {
        private static XAttribute GetAttribute(XNamespace xmlns, string attributeName, object value)
        {
            if (value == null)
                return null;

            if (xmlns == null)
                return new XAttribute(attributeName, value);
            else
                return new XAttribute(xmlns + attributeName, value);
        }

        public static XElement AddAttribute(this XElement element, XAttribute attribute)
        {
            if (attribute != null)
                element.Add(attribute);
            
            return element;
        }

        public static XElement AddAttribute(this XElement element, XNamespace xmlns, string attributeName, object value)
        {
            return AddAttribute(element, GetAttribute(xmlns, attributeName, value));
        }

        public static XElement AddAttribute(this XElement element, string attributeName, object value)
        {
            return AddAttribute(element, GetAttribute(null, attributeName, value));
        }

        public static XElement AddElement(this XElement parentElement, XElement childElement)
        {
            if (childElement != null)
                parentElement.Add(childElement);

            return parentElement;
        }
    }
}
