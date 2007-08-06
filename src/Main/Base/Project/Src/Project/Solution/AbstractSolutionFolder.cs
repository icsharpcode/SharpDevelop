// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Default implementation for ISolutionFolderContainer. Thread-safe.
	/// </summary>
	public abstract class AbstractSolutionFolder : LocalizedObject, ISolutionFolder
	{
		readonly object syncRoot = new object();
		
		ISolutionFolderContainer parent   = null;
		string                   typeGuid = null;
		string                   idGuid   = null;
		string                   location = null;
		string                   name     = null;
		
		/// <summary>
		/// Gets the object used for thread-safe synchronization.
		/// Thread-safe members lock on this object, but if you manipulate underlying structures
		/// (such as the MSBuild project for MSBuildBasedProjects) directly, you will have to lock on this object.
		/// </summary>
		[Browsable(false)]
		public object SyncRoot {
			get { return syncRoot; }
		}
		
		/// <summary>
		/// Gets the solution this project belongs to. Returns null for projects that are not (yet) added to
		/// any solution; or are added to a solution folder that is not added to any solution.
		/// </summary>
		[Browsable(false)]
		public virtual Solution ParentSolution {
			get {
				lock (syncRoot) {
					if (parent != null)
						return parent.ParentSolution;
					else
						return null;
				}
			}
		}
		
		[Browsable(false)]
		public virtual string IdGuid {
			get {
				return idGuid;
			}
			set {
				idGuid = value;
			}
		}
		
		[Browsable(false)]
		public string Location {
			get {
				return location;
			}
			set {
				location = value;
			}
		}
		
		[Browsable(false)]
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		[Browsable(false)]
		public ISolutionFolderContainer Parent {
			get {
				return parent;
			}
			set {
				lock (syncRoot) {
					parent = value;
				}
			}
		}
		
		[Browsable(false)]
		public virtual string TypeGuid {
			get {
				return typeGuid;
			}
			set {
				typeGuid = value;
			}
		}
	}
}
