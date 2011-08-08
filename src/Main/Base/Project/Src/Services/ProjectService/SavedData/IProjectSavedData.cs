// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project.SavedData
{
	public enum ProjectSavedDataType
	{
		WatchVariables
	}
	
	/// <summary>
	/// Interface for storing project specific data.
	/// When implementing this, one should be carefull when and how 
	/// the SavedDataManager is used in order to not alter the other data.
	/// </summary>
	[TypeConverter(typeof(ProjectSavedDataConverter))]
	public interface IProjectSavedData
	{
		/// <summary>
		/// Saved data type.
		/// </summary>
		ProjectSavedDataType SavedDataType { get; }
		
		/// <summary>
		/// Saved data.
		/// <remarks>The format is: "ProjectName"|{0}|"ProjectSavedDataType"|{1}|(specific data splited by '|').</remarks>
		/// </summary>
		string SavedString { get; set; }
		
		/// <summary>
		/// Gets the project name.
		/// </summary>
		string ProjectName { get; }
	}
	
	/// <summary>
	/// Dummy data. Used to map the saved data and exposed to addins where project specific data can exist.
	/// </summary>
	public sealed class DummyProjectSavedData : IProjectSavedData
	{
		public ProjectSavedDataType SavedDataType { get; set; }
		
		public string SavedString { get; set; }
		
		public string ProjectName {
			get { 
				if (string.IsNullOrEmpty(SavedString))
					return string.Empty;
				
				string[] v = SavedString.Split('|');
								
				return v[1];
			}
		}
	}
}
