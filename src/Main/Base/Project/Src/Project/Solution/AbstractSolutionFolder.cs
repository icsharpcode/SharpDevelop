using System;

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
		
		public string IdGuid {
			get {
				return idGuid;
			}
			set {
				idGuid = value;
			}
		}
		
		public string Location {
			get {
				return location;
			}
			set {
				location = value;
			}
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public ISolutionFolderContainer Parent {
			get {
				return parent;
			}
			set {
				parent = value;
			}
		}
		
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
