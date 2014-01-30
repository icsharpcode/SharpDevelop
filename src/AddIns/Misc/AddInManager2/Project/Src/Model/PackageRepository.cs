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
