// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		// TODO: expose some change event so that the solution can be marked as dirty when a section is changed
		
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
		
		/// <summary>
		/// Gets/Sets the section name
		/// </summary>
		public string SectionName { get; set; }
		
		/// <summary>
		/// Gets/Sets the section type (e.g. 'preProject'/'postProject'/'preSolution'/'postSolution')
		/// </summary>
		public string SectionType { get; set; }
		
		List<KeyValuePair<string, string>> entries = new List<KeyValuePair<string, string>>();
		
		public SolutionSection(string sectionName, string sectionType)
		{
			Validate(sectionName, sectionType);
			this.SectionName = sectionName;
			this.SectionType = sectionType;
		}
		
		public int Count {
			get { return entries.Count; }
		}
		
		public void Add(string key, string value)
		{
			Validate(key, value);
			entries.Add(new KeyValuePair<string, string>(key, value));
		}

		public bool Remove(string key)
		{
			return entries.RemoveAll(e => e.Key == key) > 0;
		}

		public void Clear()
		{
			entries.Clear();
		}

		public bool ContainsKey(string key)
		{
			for (int i = 0; i < entries.Count; i++) {
				if (entries[i].Key == key)
					return true;
			}
			return false;
		}

		public bool TryGetValue(string key, out string value)
		{
			for (int i = 0; i < entries.Count; i++) {
				if (entries[i].Key == key) {
					value = entries[i].Value;
					return true;
				}
			}
			value = null;
			return false;
		}

		public string this[string key] {
			get {
				for (int i = 0; i < entries.Count; i++) {
					if (entries[i].Key == key) {
						return entries[i].Value;
					}
				}
				return null;
			}
			set {
				Validate(key, value);
				for (int i = 0; i < entries.Count; i++) {
					if (entries[i].Key == key) {
						entries[i] = new KeyValuePair<string, string>(key, value);
						return;
					}
				}
				Add(key, value);
			}
		}

		public IEnumerable<string> Keys {
			get {
				return entries.Select(e => e.Key);
			}
		}

		public IEnumerable<string> Values {
			get {
				return entries.Select(e => e.Value);
			}
		}
		
		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return entries.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return entries.GetEnumerator();
		}
	}
}
