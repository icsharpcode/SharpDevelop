// 
// TypeScriptFunctionInsightItem.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2013 Matthew Ward
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
using System.ComponentModel;
using System.Text;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.TypeScriptBinding.Hosting;

namespace ICSharpCode.TypeScriptBinding
{
	public class TypeScriptFunctionInsightItem : IInsightItem
	{
		SignatureHelpItem helpItem;
		string header;
		
		public TypeScriptFunctionInsightItem(SignatureHelpItem helpItem)
		{
			this.helpItem = helpItem;
		}
		
		public object Header {
			get {
				if (header == null) {
					GenerateHeader();
				}
				return header;
			}
		}
		
		void GenerateHeader()
		{
			header = helpItem.GetInsightHeader();
		}
		
		public object Content {
			get {
				if ((helpItem.documentation == null) || (helpItem.documentation.Length == 0)) {
					return String.Empty;
				}
				
				if ((helpItem.parameters != null) && (helpItem.parameters.Length > 0)) {
					return helpItem.ToString() + GetParametersText(helpItem.parameters);
				}
				
				return helpItem.ToString();
			}
		}
		
		string GetParametersText(SignatureHelpParameter[] parameters)
		{
			var builder = new StringBuilder();
			foreach (SignatureHelpParameter parameter in parameters) {
				if (parameter.documentation != null && parameter.documentation.Length > 0) {
					builder.Append(parameter.name);
					builder.Append(": ");
					builder.Append(parameter.documentation[0].text);
				}
			}
			return Environment.NewLine + builder.ToString();
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, e);
			}
		}	}
}
