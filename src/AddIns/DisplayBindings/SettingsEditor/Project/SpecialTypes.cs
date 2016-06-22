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
