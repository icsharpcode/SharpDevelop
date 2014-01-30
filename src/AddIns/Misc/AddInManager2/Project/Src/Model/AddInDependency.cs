// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Text;
using ICSharpCode.Core;
using NuGet;

namespace ICSharpCode.AddInManager2.Model
{
	/// <summary>
	/// Object describing a dependency constraint of an AddIn.
	/// </summary>
	public class AddInDependency
	{
		public AddInDependency()
		{
		}
		
		public AddInDependency(PackageDependency packageDependency)
		{
			if (packageDependency != null)
			{
				IVersionSpec versionSpec = packageDependency.VersionSpec;
				
				Id = packageDependency.Id;
				if (versionSpec.MinVersion != null)
				{
					MinimumVersion = versionSpec.MinVersion.Version;
				}
				if (versionSpec.MaxVersion != null)
				{
					MaximumVersion = versionSpec.MaxVersion.Version;
				}
				IncludeMinimumVersion = packageDependency.VersionSpec.IsMinInclusive;
				IncludeMaximumVersion = packageDependency.VersionSpec.IsMaxInclusive;
			}
		}
		
		public AddInDependency(AddInReference reference)
		{
			if (reference != null)
			{
				Id = reference.Name;
				
				// Hint: An absolutely minimal or maximal version means no version constraint at all!
				Version absMinimumVersion = new Version(0, 0, 0, 0);
				Version absMaximumVersion = new Version(Int32.MaxValue, Int32.MaxValue);
				
				if ((reference.MinimumVersion != null) && (reference.MinimumVersion != absMinimumVersion))
				{
					MinimumVersion = reference.MinimumVersion;
				}
				if ((reference.MaximumVersion != null) && (reference.MaximumVersion != absMaximumVersion))
				{
					MaximumVersion = reference.MaximumVersion;
				}
				
				IncludeMinimumVersion = true;
				IncludeMaximumVersion = true;
			}
		}
		
		public string Id
		{
			get;
			set;
		}
		
		public Version MinimumVersion
		{
			get;
			set;
		}
		
		public bool IncludeMinimumVersion
		{
			get;
			set;
		}
		
		public Version MaximumVersion
		{
			get;
			set;
		}
		
		public bool IncludeMaximumVersion
		{
			get;
			set;
		}
		
		public override string ToString()
		{
			string printedMinimalDependency = FormatVersionConstraint(MinimumVersion, IncludeMinimumVersion, true);
			string printedMaximalDependency = FormatVersionConstraint(MaximumVersion, IncludeMaximumVersion, false);
			
			StringBuilder formattedDependency = new StringBuilder();
			if (Id != null)
			{
				formattedDependency.Append(Id);
			}
			if ((printedMinimalDependency != null) || (printedMaximalDependency != null))
			{
				formattedDependency.Append(" (");
			}
			if (printedMinimalDependency != null)
			{
				formattedDependency.Append(printedMinimalDependency);
			}
			if ((printedMinimalDependency != null) && (printedMaximalDependency != null))
			{
				formattedDependency.Append(" && ");
			}
			if (printedMaximalDependency != null)
			{
				formattedDependency.Append(printedMaximalDependency);
			}
			if ((printedMinimalDependency != null) || (printedMaximalDependency != null))
			{
				formattedDependency.Append(")");
			}
			
			return formattedDependency.ToString();
		}

		
		private string FormatVersionConstraint(Version version, bool includeVersion, bool isMinimum)
		{
			if (version == null)
			{
				return null;
			}
			else
			{
				string constraintOperator = isMinimum ? ">" : "<";
				string equalityOperator = includeVersion ? "=" : "";
				return String.Format("{0}{1} {2}", constraintOperator, equalityOperator, version.ToString());
			}
		}
	}
}
