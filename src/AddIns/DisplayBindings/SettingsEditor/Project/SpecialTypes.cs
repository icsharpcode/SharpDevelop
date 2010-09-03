// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Reflection;
using System.Configuration;

namespace ICSharpCode.SettingsEditor
{
	sealed class SpecialTypeDescriptor
	{
		internal string name;
		internal Type type;
		internal SpecialSetting specialSetting;
		
		internal SpecialTypeDescriptor(string name, Type type, SpecialSetting specialSetting)
		{
			this.name = name;
			this.type = type;
			this.specialSetting = specialSetting;
		}
		
		public string GetString(object value)
		{
			if (value == null)
				return "";
			else
				return type.InvokeMember("name",
				                         BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public,
				                         null, value, null) as string ?? "";
		}
		
		public object GetValue(string text)
		{
			return Activator.CreateInstance(type, text);
		}
		
		internal static readonly SpecialTypeDescriptor[] Descriptors = {
			new SpecialTypeDescriptor("(Web Service URL)",
			                          typeof(WebServiceUrlDummyType),
			                          SpecialSetting.WebServiceUrl),
			new SpecialTypeDescriptor("(Connection string)",
			                          typeof(ConnectionStringDummyType),
			                          SpecialSetting.ConnectionString)
		};
	}
	
	sealed class WebServiceUrlDummyType
	{
		public string name;
		
		public WebServiceUrlDummyType(string name)
		{
			this.name = name;
		}
	}
	
	sealed class ConnectionStringDummyType
	{
		public string name;
		
		public ConnectionStringDummyType(string name)
		{
			this.name = name;
		}
	}
}
