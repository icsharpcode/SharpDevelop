// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
