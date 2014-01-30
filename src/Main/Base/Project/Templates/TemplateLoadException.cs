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
using System.Runtime.Serialization;
using System.Xml;

namespace ICSharpCode.SharpDevelop.Templates
{
	/// <summary>
	/// Exception thrown when loading an invalid template.
	/// </summary>
	[Serializable()]
	public class TemplateLoadException : Exception
	{
		public TemplateLoadException() : base()
		{
		}
		
		public TemplateLoadException(string message) : base(message)
		{
		}
		
		public TemplateLoadException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected TemplateLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		
		internal static void AssertAttributeExists(XmlElement element, string attributeName)
		{
			if (string.IsNullOrEmpty(element.GetAttribute(attributeName))) {
				throw new TemplateLoadException("Error in template on node '" + element.Name + "':\n" +
				                                   "The attribute '" + attributeName + "' is required.");
			}
		}
	}
}
