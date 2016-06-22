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
using System.Text;
using System.Xml;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Utility class that will encode special XML characters.
	/// </summary>
	public sealed class XmlEncoder
	{
		XmlEncoder()
		{
		}
		
		/// <summary>
		/// Encodes any special characters in the xml string.
		/// </summary>
		public static string Encode(string xml, char quoteCharacter)
		{
			XmlEncoderTextWriter encoderWriter = new XmlEncoderTextWriter();
			using (XmlTextWriter writer = new XmlTextWriter(encoderWriter)) {
				writer.WriteStartElement("root");
				writer.WriteStartAttribute("attribute");
				writer.QuoteChar = quoteCharacter;
				
				encoderWriter.BeginMarkup();
				writer.WriteString(xml);
				return encoderWriter.Markup;
			}
		}
		
		/// <summary>
		/// Special XmlTextWriter that will return the last item written to
		/// it from a certain point. This is used by the XmlEncoder to
		/// get the encoded attribute string so the XmlEncoder does not
		/// have to do the special character encoding itself, but can
		/// use the .NET framework to do the work.
		/// </summary>
		class XmlEncoderTextWriter : EncodedStringWriter
		{
			StringBuilder markup = new StringBuilder();
			
			public XmlEncoderTextWriter() : base(Encoding.UTF8)
			{
			}
			
			/// <summary>
			/// Sets the point from which we are interested in 
			/// saving the string written to the text writer.
			/// </summary>
			public void BeginMarkup()
			{
				markup = new StringBuilder();
			}
			
			/// <summary>
			/// Returns the string written to this text writer after the
			/// BeginMarkup method was called.
			/// </summary>
			public string Markup {
				get {
					return markup.ToString();
				}
			}
			
			public override void Write(string text)
			{
				base.Write(text);
				markup.Append(text);
			}
			
			public override void Write(char value)
			{
				base.Write(value);
				markup.Append(value);
			}
		}
	}
}
