// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// The backend storage of a project item.
	/// </summary>
	public interface IProjectItemBackendStore
	{
		/// Gets the owning project.
		IProject Project { get; }
		
		string UnevaluatedInclude { get; set; }
		string EvaluatedInclude { get; set; }
		ItemType ItemType { get; set; }
		
		string GetEvaluatedMetadata(string name);
		string GetMetadata(string name);
		bool HasMetadata(string name);
		void RemoveMetadata(string name);
		void SetEvaluatedMetadata(string name, string value);
		void SetMetadata(string name, string value);
		IEnumerable<string> MetadataNames { get; }
	}
}
