// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.AddInManager2.Model
{
	/// <summary>
	/// Describes an AddIn package entry in AddInManager.
	/// </summary>
	public interface IAddInPackage
	{	
		string Name
		{
			get;
		}
		
		bool HasLicenseUrl
		{
			get;
		}

		Uri LicenseUrl
		{
			get;
		}

		bool HasProjectUrl
		{
			get;
		}

		Uri ProjectUrl
		{
			get;
		}

		bool HasReportAbuseUrl
		{
			get;
		}

		Uri ReportAbuseUrl
		{
			get;
		}

		bool IsAdded
		{
			get;
		}

		IEnumerable<AddInDependency> Dependencies
		{
			get;
		}

		bool HasDependencies
		{
			get;
		}

		bool HasNoDependencies
		{
			get;
		}

		IEnumerable<string> Authors
		{
			get;
		}

		bool HasDownloadCount
		{
			get;
		}

		string Id
		{
			get;
		}

		Uri IconUrl
		{
			get;
		}

		string Summary
		{
			get;
		}

		Version Version
		{
			get;
		}

		int DownloadCount
		{
			get;
		}

		string Description
		{
			get;
		}

		DateTime? LastUpdated
		{
			get;
		}

		bool HasLastUpdated
		{
			get;
		}
		
		bool HasVersion
		{
			get;
		}

		bool IsManaged
		{
			get;
		}
	}
}
