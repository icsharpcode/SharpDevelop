// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.Documentation
{
	/// <summary>
	/// Represents a documentation comment.
	/// </summary>
	[Serializable]
	public class DocumentationComment
	{
		string xmlText;
		
		/// <summary>
		/// Gets the XML code for this documentation comment.
		/// </summary>
		public string XmlText {
			get { return xmlText; }
		}
		
		public DocumentationComment(string xmlText)
		{
			if (xmlText == null)
				throw new ArgumentNullException("xmlText");
			this.xmlText = xmlText;
		}
		
		/// <summary>
		/// Resolves the given cref value to an entity.
		/// Returns null if the entity is not found, or if the cref attribute is syntactically invalid.
		/// </summary>
		public virtual IEntity ResolveCRef(string cref)
		{
			return null;
		}
	}
}
