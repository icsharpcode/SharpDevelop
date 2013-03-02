// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.AddInManager2.Model
{
	public class PackageRepository : Model<PackageRepository>
	{
		private int _highlightCount;
		
		public PackageRepository()
			: base()
		{
		}
		
		public PackageRepository(IAddInManagerServices services)
			: base(services)
		{
		}
		
		public PackageRepository(PackageSource packageSource)
			: base()
		{
			Name = packageSource.Name;
			SourceUrl = packageSource.Source;
		}
		
		public PackageRepository(IAddInManagerServices services, PackageSource packageSource)
			: base(services)
		{
			Name = packageSource.Name;
			SourceUrl = packageSource.Source;
		}
		
		public string Name
		{
			get;
			set;
		}
		
		public string SourceUrl
		{
			get;
			set;
		}
		
		public int HighlightCount
		{
			get
			{
				return _highlightCount;
			}
			set
			{
				_highlightCount = value;
				OnPropertyChanged(vm => vm.HighlightCount);
				OnPropertyChanged(vm => vm.HasHighlightCount);
				OnPropertyChanged(vm => vm.NameWithHighlight);
			}
		}
		
		public bool HasHighlightCount
		{
			get
			{
				return (_highlightCount > 0);
			}
		}
		
		public string NameWithHighlight
		{
			get
			{
				if (_highlightCount > 0)
				{
					return String.Format("{0} ({1})", Name, _highlightCount);
				}
				else
				{
					return Name;
				}
			}
		}
		
		public PackageSource ToPackageSource()
		{
			return new PackageSource(SourceUrl, Name);
		}
	}
}
