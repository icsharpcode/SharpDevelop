// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Contains values for localized properties.
	/// </summary>
	public sealed class StringParserPropertyContainer : IStringTagProvider
	{
		public static readonly StringParserPropertyContainer LocalizedProperty = new StringParserPropertyContainer();
		public static readonly StringParserPropertyContainer FileCreation = new StringParserPropertyContainer();
		
		private StringParserPropertyContainer()
		{
			StringParser.RegisterStringTagProvider(this);
		}
		
		ConcurrentDictionary<string, string> values = new ConcurrentDictionary<string, string>();
		
		public string this[string key] {
			get {
				string val;
				if (values.TryGetValue(key, out val))
					return val;
				else
					return null;
			}
			set {
				WorkbenchSingleton.AssertMainThread();
				if (value != null) {
					values[key] = value;
				} else {
					string tmp;
					values.TryRemove(key, out tmp);
				}
			}
		}
		
		public string ProvideString(string tag, StringTagPair[] customTags)
		{
			return this[tag];
		}
	}
}
