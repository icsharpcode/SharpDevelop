using System;
using System.ComponentModel;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of ISolutionFolderContainer.
	/// </summary>
	public abstract class AbstractSolutionFolder : ISolutionFolder
	{
		ISolutionFolderContainer parent   = null;
		string                   typeGuid = null;
		string                   idGuid   = null;
		string                   location = null;
		string                   name     = null;
		
		[Browsable(false)]
		public string IdGuid {
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
				parent = value;
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
