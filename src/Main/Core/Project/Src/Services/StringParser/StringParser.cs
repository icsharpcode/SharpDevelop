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

namespace ICSharpCode.Core
{
	/// <summary>
	/// This class parses internal ${xyz} tags of #Develop.
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
			properties         = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			stringTagProviders = new Dictionary<string, IStringTagProvider>(StringComparer.OrdinalIgnoreCase);
			propertyObjects    = new Dictionary<string, object>();
			
			// entryAssembly == null might happen in unit test mode
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			if (entryAssembly != null) {
				string exeName = entryAssembly.Location;
				propertyObjects["exe"] = FileVersionInfo.GetVersionInfo(exeName);
			}
			properties["USER"] = Environment.UserName;
			properties["Version"] = RevisionClass.FullVersion;
			
			// Maybe test for Mono?
			if (IntPtr.Size == 4) {
				properties["Platform"] = "Win32";
			} else if (IntPtr.Size == 8) {
				properties["Platform"] = "Win64";
			} else {
				properties["Platform"] = "unknown";
			}
		}
		
		/// <summary>
		/// Expands ${xyz} style property values.
		/// </summary>
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
			if (input == null)
				return null;
			int pos = 0;
			StringBuilder output = null; // don't use StringBuilder if input is a single property
			do {
				int oldPos = pos;
				pos = input.IndexOf("${", pos, StringComparison.Ordinal);
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
			
			// most properties start with res: in lowercase,
			// so we can save 2 string allocations here, in addition to all the jumps
			// All other prefixed properties {prefix:Key} shoulg get handled in the switch below.
			if (propertyName.StartsWith("res:", StringComparison.OrdinalIgnoreCase)) {
				try {
					return Parse(ResourceService.GetString(propertyName.Substring(4)), customTags);
				} catch (ResourceNotFoundException) {
					return null;
				}
			}
			if (propertyName.StartsWith("DATE:", StringComparison.OrdinalIgnoreCase))
			{
				try {
					return DateTime.Now.ToString(propertyName.Split(':')[1]);
				} catch (Exception ex) {
					return ex.Message;
				}
			}
			if (propertyName.Equals("DATE", StringComparison.OrdinalIgnoreCase))
				return DateTime.Today.ToShortDateString();
			if (propertyName.Equals("TIME", StringComparison.OrdinalIgnoreCase))
				return DateTime.Now.ToShortTimeString();
			if (propertyName.Equals("ProductName", StringComparison.OrdinalIgnoreCase))
				return MessageService.ProductName;
			if (propertyName.Equals("GUID", StringComparison.OrdinalIgnoreCase))
				return Guid.NewGuid().ToString().ToUpperInvariant();
			
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
			propertyName = propertyName.Substring(k + 1);
			switch (prefix.ToUpperInvariant()) {
				case "SDKTOOLPATH":
					return FileUtility.GetSdkPath(propertyName);
				case "ADDINPATH":
					foreach (AddIn addIn in AddInTree.AddIns) {
						if (addIn.Manifest.Identities.ContainsKey(propertyName)) {
							return System.IO.Path.GetDirectoryName(addIn.FileName);
						}
					}
					return null;
				case "ENV":
					return Environment.GetEnvironmentVariable(propertyName);
				case "RES":
					try {
						return Parse(ResourceService.GetString(propertyName), customTags);
					} catch (ResourceNotFoundException) {
						return null;
					}
				case "PROPERTY":
					return GetProperty(propertyName);
				default:
					if (propertyObjects.ContainsKey(prefix)) {
						return Get(propertyObjects[prefix], propertyName);
					} else {
						return null;
					}
			}
		}
		
		/// <summary>
		/// Allow special syntax to retrieve property values:
		/// ${property:PropertyName}
		/// ${property:PropertyName??DefaultValue}
		/// ${property:ContainerName/PropertyName}
		/// ${property:ContainerName/PropertyName??DefaultValue}
		/// A container is a Properties instance stored in the PropertyService. This is
		/// used by many AddIns to group all their properties into one container.
		/// </summary>
		static string GetProperty(string propertyName)
		{
			string defaultValue = "";
			int pos = propertyName.LastIndexOf("??", StringComparison.Ordinal);
			if (pos >= 0) {
				defaultValue = propertyName.Substring(pos + 2);
				propertyName = propertyName.Substring(0, pos);
			}
			pos = propertyName.IndexOf('/');
			if (pos >= 0) {
				Properties properties = PropertyService.Get(propertyName.Substring(0, pos), new Properties());
				propertyName = propertyName.Substring(pos + 1);
				pos = propertyName.IndexOf('/');
				while (pos >= 0) {
					properties = properties.Get(propertyName.Substring(0, pos), new Properties());
					propertyName = propertyName.Substring(pos + 1);
				}
				return properties.Get(propertyName, defaultValue);
			} else {
				return PropertyService.Get(propertyName, defaultValue);
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
