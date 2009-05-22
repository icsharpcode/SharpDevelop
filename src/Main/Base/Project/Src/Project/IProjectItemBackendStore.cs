/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 22.05.2009
 * Time: 22:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
		
		string Include { get; set; }
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
