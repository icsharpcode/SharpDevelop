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
using System.Text.RegularExpressions;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Represents a configuration/platform pair.
	/// </summary>
	public struct ConfigurationAndPlatform : IEquatable<ConfigurationAndPlatform>
	{
		public static readonly StringComparer ConfigurationNameComparer = StringComparer.OrdinalIgnoreCase;
		
		public static bool IsValidName(string name)
		{
			return !string.IsNullOrEmpty(name)
				&& MSBuildInternals.Escape(name) == name
				&& name.Trim() == name
				&& FileUtility.IsValidDirectoryEntryName(name)
				&& name.IndexOf('\'') < 0
				&& name.IndexOf('.') < 0;
		}
		
		readonly static Regex configurationRegEx = new Regex(@"'(?<property>[^']*)'\s*==\s*'(?<value>[^']*)'", RegexOptions.Compiled);
		
		/// <summary>
		/// Gets configuration and platform from an MSBuild condition in the format "'$(Configuration)|$(Platform)' == 'configuration|platform'".
		/// If the condition only tests the configuration, the platform property will be null (and vice versa).
		/// If the condition is not recognized, both condition and platform of the result will be null.
		/// </summary>
		public static ConfigurationAndPlatform FromCondition(string condition)
		{
			Match match = configurationRegEx.Match(condition);
			if (match.Success) {
				string conditionProperty = match.Result("${property}");
				string conditionValue = match.Result("${value}");
				if (conditionProperty == "$(Configuration)|$(Platform)") {
					// configuration is ok
					return FromKey(conditionValue);
				} else if (conditionProperty == "$(Configuration)") {
					return new ConfigurationAndPlatform(conditionValue, null);
				} else if (conditionProperty == "$(Platform)") {
					return new ConfigurationAndPlatform(null, conditionValue);
				} else {
					return default(ConfigurationAndPlatform);
				}
			} else {
				return default(ConfigurationAndPlatform);
			}
		}
		
		/// <summary>
		/// Gets configuration and platform from a key string in the format 'configuration|platform'.
		/// If the key string has an invalid format, both condition and platform of the result will be null.
		/// </summary>
		public static ConfigurationAndPlatform FromKey(string key)
		{
			int pos = key.IndexOf('|');
			if (pos < 0)
				return default(ConfigurationAndPlatform);
			else
				return new ConfigurationAndPlatform(key.Substring(0, pos), key.Substring(pos + 1));
		}
		
		readonly string configuration;
		readonly string platform;
		
		public ConfigurationAndPlatform(string configuration, string platform)
		{
			this.configuration = configuration;
			this.platform = platform;
		}
		
		public string Platform {
			get { return platform; }
		}

		public string Configuration {
			get { return configuration; }
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			if (obj is ConfigurationAndPlatform)
				return Equals((ConfigurationAndPlatform)obj); // use Equals method below
			else
				return false;
		}
		
		public bool Equals(ConfigurationAndPlatform other)
		{
			return ConfigurationNameComparer.Equals(this.configuration, other.configuration) && ConfigurationNameComparer.Equals(this.platform, other.platform);
		}
		
		public override int GetHashCode()
		{
			return (configuration != null ? ConfigurationNameComparer.GetHashCode(configuration) : 0) ^ (platform != null ? ConfigurationNameComparer.GetHashCode(platform) : 0);
		}
		
		public static bool operator ==(ConfigurationAndPlatform left, ConfigurationAndPlatform right)
		{
			return left.Equals(right);
		}
		
		public static bool operator !=(ConfigurationAndPlatform left, ConfigurationAndPlatform right)
		{
			return !left.Equals(right);
		}
		#endregion
		
		/// <summary>
		/// Converts configuration and platform to a string in the 'configuration|platform' format.
		/// </summary>
		public override string ToString()
		{
			return configuration + "|" + platform;
		}
		
		/// <summary>
		/// Creates an MSBuild condition string.
		/// At most one of configuration and platform can be null.
		/// </summary>
		public string ToCondition()
		{
			if (configuration == null)
				return CreateCondition(configuration, platform, PropertyStorageLocations.PlatformSpecific);
			else if (platform == null)
				return CreateCondition(configuration, platform, PropertyStorageLocations.ConfigurationSpecific);
			else
				return CreateCondition(configuration, platform, PropertyStorageLocations.ConfigurationAndPlatformSpecific);
		}
		
		/// <summary>
		/// Creates an MSBuild condition string.
		/// configuration and platform may be only <c>null</c> if they are not required (as specified by the
		/// storage location), otherwise an ArgumentNullException is thrown.
		/// </summary>
		internal static string CreateCondition(string configuration, string platform, PropertyStorageLocations location)
		{
			switch (location & PropertyStorageLocations.ConfigurationAndPlatformSpecific) {
				case PropertyStorageLocations.ConfigurationSpecific:
					if (configuration == null)
						throw new ArgumentNullException("configuration");
					return " '$(Configuration)' == '" + configuration + "' ";
				case PropertyStorageLocations.PlatformSpecific:
					if (platform == null)
						throw new ArgumentNullException("platform");
					return " '$(Platform)' == '" + platform + "' ";
				case PropertyStorageLocations.ConfigurationAndPlatformSpecific:
					if (platform == null)
						throw new ArgumentNullException("platform");
					if (configuration == null)
						throw new ArgumentNullException("configuration");
					return " '$(Configuration)|$(Platform)' == '" + configuration + "|" + platform + "' ";
				default:
					return null;
			}
		}
	}
}
