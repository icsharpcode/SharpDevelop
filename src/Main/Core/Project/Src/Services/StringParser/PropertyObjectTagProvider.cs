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
using System.Reflection;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Provides properties by using Reflection on an object.
	/// </summary>
	public sealed class PropertyObjectTagProvider : IStringTagProvider
	{
		readonly object obj;
		
		public PropertyObjectTagProvider(object obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");
			this.obj = obj;
		}
		
		public string ProvideString(string tag, StringTagPair[] customTags)
		{
			Type type = obj.GetType();
			PropertyInfo prop = type.GetProperty(tag);
			if (prop != null) {
				return prop.GetValue(obj, null).ToString();
			}
			FieldInfo field = type.GetField(tag);
			if (field != null) {
				return field.GetValue(obj).ToString();
			}
			return null;
		}
	}
}
