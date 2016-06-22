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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// A list of key-value pairs within a solution file.
	/// </summary>
	public class SolutionSection : IReadOnlyDictionary<string, string>
	{
		public event EventHandler Changed = delegate { };
		
		static readonly char[] forbiddenChars = { '\n', '\r', '\0', '=' };
		
		static void Validate(string key, string value)
		{
			if (key == null)
				throw new ArgumentNullException("key");
			if (value == null)
				throw new ArgumentNullException("value");
			if (key.IndexOfAny(forbiddenChars) >= 0)
				throw new ArgumentException("key contains invalid characters", "key");
			if (value.IndexOfAny(forbiddenChars) >= 0)
				throw new ArgumentException("value contains invalid characters", "value");
		}
		
		string sectionName;
		string sectionType;
		List<KeyValuePair<string, string>> entries = new List<KeyValuePair<string, string>>();
		
		public SolutionSection(string sectionName, string sectionType)
		{
			Validate(sectionName, sectionType);
			this.sectionName = sectionName;
			this.sectionType = sectionType;
		}
		
		/// <summary>
		/// Gets/Sets the section name
		/// </summary>
		public string SectionName {
			get {
				return sectionName;
			}
			set {
				if (sectionName != value) {
					Validate(value, sectionType);
					sectionName = value;
					Changed(this, EventArgs.Empty);
				}
			}
		}
		
		/// <summary>
		/// Gets/Sets the section type (e.g. 'preProject'/'postProject'/'preSolution'/'postSolution')
		/// </summary>
		public string SectionType {
			get {
				return sectionType;
			}
			set {
				if (sectionType != value) {
					Validate(sectionName, value);
					sectionType = value;
					Changed(this, EventArgs.Empty);
				}
			}
		}
		
		public int Count {
			get {
				lock (entries)
					return entries.Count;
			}
		}
		
		public void Add(string key, string value)
		{
			Validate(key, value);
			lock (entries)
				entries.Add(new KeyValuePair<string, string>(key, value));
			Changed(this, EventArgs.Empty);
		}

		public bool Remove(string key)
		{
			bool result;
			lock (entries)
				result = entries.RemoveAll(e => e.Key == key) > 0;
			if (result)
				Changed(this, EventArgs.Empty);
			return result;
		}

		public void Clear()
		{
			lock (entries)
				entries.Clear();
			Changed(this, EventArgs.Empty);
		}

		public bool ContainsKey(string key)
		{
			lock (entries) {
				for (int i = 0; i < entries.Count; i++) {
					if (entries[i].Key == key)
						return true;
				}
			}
			return false;
		}

		public bool TryGetValue(string key, out string value)
		{
			lock (entries) {
				for (int i = 0; i < entries.Count; i++) {
					if (entries[i].Key == key) {
						value = entries[i].Value;
						return true;
					}
				}
			}
			value = null;
			return false;
		}

		public string this[string key] {
			get {
				lock (entries) {
					for (int i = 0; i < entries.Count; i++) {
						if (entries[i].Key == key) {
							return entries[i].Value;
						}
					}
				}
				return null;
			}
			set {
				Validate(key, value);
				lock (entries) {
					bool found = false;
					for (int i = 0; i < entries.Count; i++) {
						if (entries[i].Key == key) {
							entries[i] = new KeyValuePair<string, string>(key, value);
							found = true;
							break;
						}
					}
					if (!found)
						entries.Add(new KeyValuePair<string, string>(key, value));
				}
				Changed(this, EventArgs.Empty);
			}
		}

		public IEnumerable<string> Keys {
			get {
				lock (entries)
					return entries.Select(e => e.Key).ToArray();
			}
		}

		public IEnumerable<string> Values {
			get {
				lock (entries)
					return entries.Select(e => e.Value).ToArray();
			}
		}
		
		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			lock (entries)
				return entries.ToList().GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
