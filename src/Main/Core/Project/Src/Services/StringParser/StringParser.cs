// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using ICSharpCode.Core;

namespace ICSharpCode.Core
{
	/// <summary>
	/// this class parses internal ${xyz} tags of sd.
	/// All environment variables are avaible under the name env.[NAME]
	/// where [NAME] represents the string under which it is avaiable in
	/// the environment.
	/// </summary>
	public static class StringParser
	{
		readonly static Dictionary<string, string>             properties;
		readonly static Dictionary<string, IStringTagProvider> stringTagProviders;
		readonly static Dictionary<string, object>             propertyObjects;
		
		public static Dictionary<string, string> Properties {
			get {
				return properties;
			}
		}
		
		public static Dictionary<string, object> PropertyObjects {
			get {
				return propertyObjects;
			}
		}
		
		static StringParser()
		{
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			properties         = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			stringTagProviders = new Dictionary<string, IStringTagProvider>(StringComparer.InvariantCultureIgnoreCase);
			propertyObjects    = new Dictionary<string, object>();
			// entryAssembly == null might happen in unit test mode
			if (entryAssembly != null) {
				string exeName = entryAssembly.Location;
				propertyObjects["exe"] = FileVersionInfo.GetVersionInfo(exeName);
			}
			properties["USER"] = Environment.UserName;
		}
		
		public static string Parse(string input)
		{
			return Parse(input, null);
		}
		
		/// <summary>
		/// Parses an array and replaces the elements in the existing array.
		/// </summary>
		public static void Parse(string[] inputs)
		{
			for (int i = 0; i < inputs.Length; ++i) {
				inputs[i] = Parse(inputs[i], null);
			}
		}
		
		public static void RegisterStringTagProvider(IStringTagProvider tagProvider)
		{
			foreach (string str in tagProvider.Tags) {
				stringTagProviders[str] = tagProvider;
			}
		}
		
		//readonly static Regex pattern = new Regex(@"\$\{([^\}]*)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		
		/// <summary>
		/// Expands ${xyz} style property values.
		/// </summary>
		public static string Parse(string input, string[,] customTags)
		{
			// Parse is a important method and should have good performance,
			// so we don't use an expensive Regex here.
			
			/* old code using regex:
			string output = input;
			if (input != null) {
				foreach (Match m in pattern.Matches(input)) {
					if (m.Length > 0) {
						string token         = m.ToString();
						string propertyName  = m.Groups[1].Captures[0].Value;
						
						string propertyValue = GetValue(propertyName, customTags);
						
						if (propertyValue != null) {
							if (m.Length == input.Length) {
								// safe a replace operation when input is a property on its own.
								return propertyValue;
							}
							output = output.Replace(token, propertyValue);
						}
					}
				}
			}
			return output;
			 */
			if (input == null)
				return null;
			int pos = 0;
			StringBuilder output = null; // don't use StringBuilder if input is a single property
			do {
				int oldPos = pos;
				pos = input.IndexOf("${", pos);
				if (pos < 0) {
					if (output == null) {
						return input;
					} else {
						if (oldPos < input.Length) {
							// normal text after last property
							output.Append(input, oldPos, input.Length - oldPos);
						}
						return output.ToString();
					}
				}
				if (output == null) {
					if (pos == 0)
						output = new StringBuilder();
					else
						output = new StringBuilder(input, 0, pos, pos + 16);
				} else {
					if (pos > oldPos) {
						// normal text between two properties
						output.Append(input, oldPos, pos - oldPos);
					}
				}
				int end = input.IndexOf('}', pos + 1);
				if (end < 0) {
					output.Append("${");
					pos += 2;
				} else {
					string property = input.Substring(pos + 2, end - pos - 2);
					string val = GetValue(property, customTags);
					if (val == null) {
						output.Append("${");
						output.Append(property);
						output.Append('}');
					} else {
						output.Append(val);
					}
					pos = end + 1;
				}
			} while (pos < input.Length);
			return output.ToString();
		}
		
		static string GetValue(string propertyName, string[,] customTags)
		{
			if (propertyName.StartsWith("res:")) {
				// most properties start with res: in lowercase,
				// so we can safe 2 string allocations here
				try {
					return Parse(ResourceService.GetString(propertyName.Substring(4)), customTags);
				} catch (ResourceNotFoundException) {
					return null;
				}
			}
			if (propertyName.Equals("DATE", StringComparison.OrdinalIgnoreCase))
				return DateTime.Today.ToShortDateString();
			if (propertyName.Equals("TIME", StringComparison.OrdinalIgnoreCase))
				return DateTime.Now.ToShortTimeString();
			if (propertyName.Equals("ProductName", StringComparison.OrdinalIgnoreCase))
				return MessageService.ProductName;
			
			if (customTags != null) {
				for (int j = 0; j < customTags.GetLength(0); ++j) {
					if (propertyName.Equals(customTags[j, 0], StringComparison.OrdinalIgnoreCase)) {
						return customTags[j, 1];
					}
				}
			}
			
			if (properties.ContainsKey(propertyName)) {
				return properties[propertyName];
			}
			
			if (stringTagProviders.ContainsKey(propertyName)) {
				return stringTagProviders[propertyName].Convert(propertyName);
			}
			
			int k = propertyName.IndexOf(':');
			if (k <= 0)
				return null;
			string prefix = propertyName.Substring(0, k);
			switch (prefix.ToUpperInvariant()) {
				case "ENV":
					return Environment.GetEnvironmentVariable(propertyName.Substring(k + 1));
				case "RES":
					try {
						return Parse(ResourceService.GetString(propertyName.Substring(k + 1)), customTags);
					} catch (ResourceNotFoundException) {
						return null;
					}
				case "PROPERTY":
					return PropertyService.Get(propertyName.Substring(k + 1));
				default:
					if (propertyObjects.ContainsKey(prefix)) {
						return Get(propertyObjects[prefix], propertyName.Substring(k + 1));
					} else {
						return null;
					}
			}
		}
		
		static string Get(object obj, string name)
		{
			Type type = obj.GetType();
			PropertyInfo prop = type.GetProperty(name);
			if (prop != null) {
				return prop.GetValue(obj, null).ToString();
			}
			FieldInfo field = type.GetField(name);
			if (field != null) {
				return field.GetValue(obj).ToString();
			}
			return null;
		}
	}
}
