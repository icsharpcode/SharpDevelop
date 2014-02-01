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

#region Usings

using System;
using System.Linq;
using System.Xml.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using System.IO;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.IO
{
    public class IO
    {
        #region Namespace declarations

        protected static XNamespace edmxNamespace = "http://schemas.microsoft.com/ado/2008/10/edmx";
        protected static XNamespace ssdlNamespace = "http://schemas.microsoft.com/ado/2009/02/edm/ssdl";
        protected static XNamespace storeNamespace = "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator";

        protected static XNamespace csdlNamespace = "http://schemas.microsoft.com/ado/2008/09/edm";
        protected static XNamespace csdlCodeGenerationNamespace = "http://schemas.microsoft.com/ado/2006/04/codegeneration";
        protected static XNamespace csdlAnnotationNamespace = "http://schemas.microsoft.com/ado/2009/02/edm/annotation";

        protected static XNamespace mslNamespace = "http://schemas.microsoft.com/ado/2008/09/mapping/cs";
        
        #endregion

        #region Helper functions

        protected static string GetName(string fullName)
        {
            return fullName.Substring(fullName.LastIndexOf(".") + 1);
        }

        protected static void SetStringValueFromAttribute(XElement element, string attributeName, Action<string> setAction)
        {
            SetStringValueFromAttribute(element, attributeName, string.Empty, setAction);
        }

        protected static void SetStringValueFromAttribute(XElement element, string attributeName, string ns, Action<string> setAction)
        {
            var attribute = element.Attribute(XName.Get(attributeName, ns));
            if (attribute != null)
                setAction(attribute.Value);
        }

        protected static void SetBoolValueFromAttribute(XElement element, string attributeName, Action<bool> setAction)
        {
            SetStringValueFromAttribute(element, attributeName,
                value =>
                {
                    switch (value)
                    {
                        case "0":
                        case "false":
                        case "False":
                            setAction(false);
                            break;
                        case "1":
                        case "true":
                        case "True":
                            setAction(true);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                });
        }

        protected static void SetIntValueFromAttribute(XElement element, string attributeName, Action<int> setAction)
        {
            SetStringValueFromAttribute(element, attributeName,
                value =>
                {
                    if (value != "Max")
                        setAction(int.Parse(value));
                });
        }

        protected static void SetEnumValueFromAttribute<T>(XElement element, string attribute, Action<T> setAction)
        {
            SetEnumValueFromAttribute<T>(element, attribute, string.Empty, setAction);
        }

        protected static void SetEnumValueFromAttribute<T>(XElement element, string attribute, string ns, Action<T> setAction)
        {
            SetStringValueFromAttribute(element, attribute, ns,
                enumName =>
                {
                    T[] values = Enum.GetValues(typeof(T)).Cast<object>().Where(v => v.ToString() == enumName).Select(v => (T)v).Take(1).ToArray();
                    if (values.Length == 0)
                        throw new NotImplementedException();
                    setAction(values[0]);
                });
        }

        protected static void SetCardinalityValueFromAttribute(XElement element, Action<Cardinality> setAction)
        {
            SetStringValueFromAttribute(element, "Multiplicity", multiplicity => setAction(CardinalityStringConverter.CardinalityFromString(multiplicity)));
        }

        protected static void SetStringValueFromElement(XElement element, string elementName, string ns, Action<string> setAction)
        {
            var subElement = element.Element(XName.Get(elementName, ns));
            if (subElement != null)
                setAction(subElement.Value);
        }

        public static string GetTempFilename()
        {
            return Path.GetTempFileName();
        }

        public static string GetTempFilenameWithExtension(string filenamePrefix, string extension)
        {
            string tempFileAuto = GetTempFilename();

            string tempFile =
                Path.GetTempPath() +
                filenamePrefix +
                Path.GetFileNameWithoutExtension(tempFileAuto)
                + "." + extension;

            return tempFile;
        }

        public static string GetTempFilenameWithExtension(string extension)
        {
            return GetTempFilenameWithExtension(string.Empty, extension);
        }

        #endregion
    }
}
