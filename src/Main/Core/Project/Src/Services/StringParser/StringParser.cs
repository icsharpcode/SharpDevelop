// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
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
		static Dictionary<string, string>             properties         = new Dictionary<string, string>();
		static Dictionary<string, IStringTagProvider> stringTagProviders = new Dictionary<string, IStringTagProvider>();
		static Dictionary<string, object>             propertyObjects    = new Dictionary<string, object>();
		
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
			Assembly entryAssembly =  System.Reflection.Assembly.GetEntryAssembly();
			
			// entryAssembly == null might happen in unit test mode
			if (entryAssembly != null) {
				string exeName = entryAssembly.Location;
				propertyObjects["exe"] = FileVersionInfo.GetVersionInfo(exeName);
			}
		}
		
		public static string Parse(string input)
		{
			return Parse(input, null);
		}
		
		/// <summary>
		/// Parses an array and replaces the elements
		/// </summary>
		public static void Parse(ref string[] inputs)
		{
			for (int i = inputs.GetLowerBound(0); i <= inputs.GetUpperBound(0); ++i) {
				inputs[i] = Parse(inputs[i], null);
			}
		}
		
		public static void RegisterStringTagProvider(IStringTagProvider tagProvider)
		{
			foreach (string str in tagProvider.Tags) {
				stringTagProviders[str] = tagProvider;
			}
		}
		
		readonly static Regex pattern = new Regex(@"\$\{([^\}]*)\}");
		
		/// <summary>
		/// Expands ${xyz} style property values.
		/// </summary>
		public static string Parse(string input, string[,] customTags)
		{
			string output = input;
			if (input != null) {
				foreach (Match m in pattern.Matches(input)) {
					if (m.Length > 0) {
						string token         = m.ToString();
						string propertyName  = m.Groups[1].Captures[0].Value;
						
						string propertyNameUpper= propertyName.ToUpper();
						string propertyValue = null;
						switch (propertyName.ToUpper()) {
							case "USER": // current user
								propertyValue = Environment.UserName;
								break;
							case "DATE": // current date
								propertyValue = DateTime.Today.ToShortDateString();
								break;
							case "TIME": // current time
								propertyValue = DateTime.Now.ToShortTimeString();
								break;
							default:
								propertyValue = null;
								if (customTags != null) {
									for (int j = 0; j < customTags.GetLength(0); ++j) {
										if (propertyName.ToUpper() == customTags[j, 0].ToUpper()) {
											propertyValue = customTags[j, 1];
											break;
										}
									}
								}
								
								if (propertyValue == null && properties.ContainsKey(propertyName)) {
									propertyValue = properties[propertyName];
								}
								
								if (propertyValue == null && properties.ContainsKey(propertyNameUpper)) {
									propertyValue = properties[propertyNameUpper];
								}
								if (propertyValue == null && stringTagProviders.ContainsKey(propertyName)) {
									propertyValue = stringTagProviders[propertyName].Convert(propertyName);
								}
								
								if (propertyValue == null) {
									int k = propertyName.IndexOf(':');
									if (k > 0) {
										switch (propertyName.Substring(0, k).ToUpper()) {
												
											case "ENV":
												propertyValue = Environment.GetEnvironmentVariable(propertyName.Substring(k + 1));
												break;
												
											case "RES":
												try {
													propertyValue = Parse(ResourceService.GetString(propertyName.Substring(k + 1)), customTags);
												} catch (Exception) {
													propertyValue = null;
												}
												break;

											case "PROPERTY":
												propertyValue = PropertyService.Get(propertyName.Substring(k + 1));
												break;
												
											default:
												object obj = propertyObjects[propertyName.Substring(0, k)];
												propertyValue = Get(obj, propertyName.Substring(k + 1));
												break;
										}
									}
								}
								break;
						}
						if (propertyValue != null) {
							output = output.Replace(token, propertyValue);
						}
					}
				}
			}
			return output;
		}
		
		static string Get(object obj, string name) 
		{
			if (obj == null) {
				return null;
			}
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
