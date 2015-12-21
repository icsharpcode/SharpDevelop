// 
// FormalSignatureInfo.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2013-2015 Matthew Ward
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Linq;
using System.Text;

namespace ICSharpCode.TypeScriptBinding.Hosting
{
	public class SignatureHelpItem
	{
		public SignatureHelpItem()
		{
			prefixDisplayParts = new SymbolDisplayPart[0];
			suffixDisplayParts = new SymbolDisplayPart[0];
			separatorDisplayParts = new SymbolDisplayPart[0];
			parameters = new SignatureHelpParameter[0];
			documentation = new SymbolDisplayPart[0];
		}
		
		public bool isVariadic { get; set; }
		public SymbolDisplayPart[] prefixDisplayParts { get; set; }
		public SymbolDisplayPart[] suffixDisplayParts { get; set; }
		public SymbolDisplayPart[] separatorDisplayParts { get; set; }
		public SignatureHelpParameter[] parameters { get; set; }
		public SymbolDisplayPart[] documentation { get; set; }
		
		public override string ToString()
		{
			if ((documentation == null) || (documentation.Length == 0)) {
				return String.Empty;
			}
			
			return documentation[0].text;
		}
		
		public string GetInsightHeader()
		{
			if ((prefixDisplayParts == null) || (suffixDisplayParts == null) || (separatorDisplayParts == null)) {
				return String.Empty;
			}
			
			var header = new StringBuilder();
			AppendText(header, prefixDisplayParts);
			
			bool parameterAdded = false;
			foreach (SignatureHelpParameter parameter in parameters) {
				AppendText(header, parameter.displayParts);
				if (parameterAdded) {
					AppendText(header, separatorDisplayParts);
				}
				parameterAdded = true;
			}
			AppendText(header, suffixDisplayParts);
			
			return header.ToString();
		}
		
		void AppendText(StringBuilder textBuilder, SymbolDisplayPart[] parts)
		{
			foreach (SymbolDisplayPart part in parts) {
				textBuilder.Append(part.text);
			}
		}
	}
}
