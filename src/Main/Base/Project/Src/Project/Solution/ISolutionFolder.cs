using System;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of ISolutionFolderContainer.
	/// </summary>
	public interface ISolutionFolder
	{
		ISolutionFolderContainer Parent {
			get;
			set;
		}
		
		string TypeGuid {
			get;
			set;
		}
		
		string IdGuid {
			get;
			set;
		}
		
		string Location {
			get;
			set;
		}
		
		string Name {
			get;
			set;
		}
	}
}
