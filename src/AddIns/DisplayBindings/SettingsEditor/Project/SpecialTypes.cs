// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Reflection;

namespace ICSharpCode.SettingsEditor
{
	sealed class SpecialTypeDescriptor
	{
		internal string name;
		internal Type type;
		
		internal SpecialTypeDescriptor(string name, Type type)
		{
			this.name = name;
			this.type = type;
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
			new SpecialTypeDescriptor("(Web Service URL)", typeof(WebServiceUrlDummyType)),
			new SpecialTypeDescriptor("(Connection string)", typeof(ConnectionStringDummyType)),
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
