//
// -*- C# -*-
//
// Author:         Roman Taranchenko
// Copyright:      (c) 2004 Roman Taranchenko
// Copying Policy: GNU General Public License
//

using System;
using System.Collections;
using System.IO;

namespace CPPBinding
{

	/// <summary>
	/// Description of IncludeParser.	
	/// </summary>
	public class IncludeParser
	{
		private IDependence _owner;
		private IList _includeDirectories;
		private bool _onlyProjectInclude;
	
		public IncludeParser(IDependence owner, IList includeDirectories, bool onlyProjectInclude)
		{
			_includeDirectories = includeDirectories;
			_owner = owner;
			_onlyProjectInclude = onlyProjectInclude;
		}

		public IncludeParser(String fileName, IList includeDirectories, bool onlyProjectInclude)
		{
			_includeDirectories = includeDirectories;
			_owner = new SourceFile(fileName);
			_onlyProjectInclude = onlyProjectInclude;
		}
		
		public IDependence Parse() 
		{
			ParseFile(_owner);
			return _owner;
		}
	
		private void ParseFile(IDependence parent)
		{
			if (!parent.Exists)
			{
				return;
			}
			ArrayList includes = new ArrayList();
			StreamReader reader = File.OpenText(parent.Name);
			using (reader)
			{
				string line = null;
				while ((line = reader.ReadLine()) != null)
				{
					line = line.TrimStart(' ', '\t');
					if (line.StartsWith("#include"))
					{
						IDependence	include = GetInclude(line);
						if (include != null)
						{
							includes.Add(include);
						}
					}
				}
			}
			foreach (IDependence include in includes)
			{
				parent.Add(include);
				ParseFile(include);
			}
		}
	
		private IDependence GetInclude(string line)
		{
			IDependence result = null;
			int start = -1, end = -1;
			bool quotes = false;
		
			// find first index
			for (int i = 8; i < line.Length; ++i)
			{
				if (!_onlyProjectInclude && line[i] == '<')
				{
					start = i+1;
					break;
				}
				if (line[i] == '"')
				{
					start = i+1;
					quotes = true;
					break;
				}
			}
		
			// find second index
			if (start > 0 && start + 1 < line.Length)
			{
				end = line.IndexOf(quotes ? '"' : '>', start + 1);
			}
		
			// create include
			if (start > 0 && end > start)
			{
				string includeName = line.Substring(start, end-start);
				foreach (string dir in _includeDirectories)
				{
					string fullName = Path.Combine(dir, includeName);
					if (!_owner.Contains(fullName) && File.Exists(fullName))
					{
						result = new SourceFile(fullName);
					}
				}
			}
		
			return result;
		}

	}

}
