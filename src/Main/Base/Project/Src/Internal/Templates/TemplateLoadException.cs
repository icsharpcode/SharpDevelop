// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;
using System.Xml;

namespace ICSharpCode.SharpDevelop.Internal.Templates
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
