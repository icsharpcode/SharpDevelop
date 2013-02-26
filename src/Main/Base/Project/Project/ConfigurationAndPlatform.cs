// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text.RegularExpressions;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Represents a configuration/platform pair.
	/// </summary>
	public struct ConfigurationAndPlatform : IEquatable<ConfigurationAndPlatform>
	{
		readonly static Regex configurationRegEx = new Regex(@"'(?<property>[^']*)'\s*==\s*'(?<value>[^']*)'", RegexOptions.Compiled);
		
		/// <summary>
		/// Gets configuration and platform from an MSBuild condition in the format "'$(Configuration)|$(Platform)' == 'configuration|platform'".
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
			return this.configuration == other.configuration && this.platform == other.platform;
		}
		
		public override int GetHashCode()
		{
			return (configuration != null ? configuration.GetHashCode() : 0) ^ (platform != null ? platform.GetHashCode() : 0);
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
		
		public override string ToString()
		{
			return configuration + "|" + platform;
		}
	}
}
