// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
