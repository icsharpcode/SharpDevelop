// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	class SolutionFileItem : ISolutionFileItem
	{
		readonly Solution parentSolution;
		readonly Guid idGuid;
		
		public SolutionFileItem(Solution parentSolution)
		{
			this.parentSolution = parentSolution;
			this.idGuid = Guid.NewGuid();
		}
		
		FileName fileName;
		
		public FileName FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
				parentSolution.IsDirty = true;
			}
		}

		public ISolutionFolder ParentFolder { get; set; }

		public ISolution ParentSolution {
			get { return parentSolution; }
		}

		public Guid IdGuid {
			get { return idGuid; }
		}

		public Guid TypeGuid {
			get { return Guid.Empty; }
		}
	}
}
