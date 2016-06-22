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

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Parses a string of Wix property values (e.g. "$(var.DATAPATH)") it also
	/// handles MSBuild property values (e.g. "$(SharpDevelopBinPath)").
	/// </summary>
	/// <remarks>
	/// Wix property names are case sensitive.
	/// </remarks>
	public sealed class WixPropertyParser
	{
		const string PropertyPrefix = "var.";
		const string PropertyStart = "$(";
		const string PropertyEnd = ")";
		
		WixPropertyParser()
		{
		}
		
		public static string Parse(string input, IWixPropertyValueProvider valueProvider)
		{
			StringBuilder output = new StringBuilder();
			
			int currentPos = 0;
			do {
				// Find property.
				int endIndex = -1;
				int startIndex = input.IndexOf(PropertyStart, currentPos);
				if (startIndex != -1) {
					endIndex = input.IndexOf(PropertyEnd, startIndex);
				}
				
				// Did we find a property?
				if (endIndex == -1) {
					if (currentPos == 0) {
						return input;
					} else {
						output.Append(input.Substring(currentPos));
						return output.ToString();
					}
				}
									
				// Get the property name
				int propertyNameStart = startIndex + PropertyStart.Length;
				int propertyNameLength = endIndex - propertyNameStart;
				string propertyName = input.Substring(propertyNameStart, propertyNameLength);
				if (propertyName.StartsWith(PropertyPrefix)) {
					propertyName = propertyName.Substring(PropertyPrefix.Length);
				}
				
				// Get the property value.
				string propertyValue = valueProvider.GetValue(propertyName);
				if (propertyValue != null) {
					output.Append(String.Concat(input.Substring(currentPos, startIndex - currentPos), propertyValue));
				} else {
					output.Append(input.Substring(currentPos, endIndex - currentPos));
				}
				currentPos = endIndex + 1;
			} while (currentPos < input.Length);
			
			return output.ToString();
		}
	}
}
