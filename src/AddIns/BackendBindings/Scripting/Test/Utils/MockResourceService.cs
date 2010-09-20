// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Resources;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockResourceService : IResourceService
	{
		IResourceReader reader;
		IResourceWriter writer;
		CultureInfo readerCultureInfo;
		CultureInfo writerCultureInfo;
		
		public MockResourceService()
		{
			reader = new MockResourceReader();
		}
		
		public void SetResourceReader(IResourceReader reader)
		{
			this.reader = reader;
		}

		public void SetResourceWriter(IResourceWriter writer)
		{
			this.writer = writer;
		}
		
		public IResourceReader GetResourceReader(CultureInfo info)
		{
			readerCultureInfo = info;
			return reader;
		}
		
		/// <summary>
		/// Gets the culture passed to GetResourceReader.
		/// </summary>
		public CultureInfo CultureInfoPassedToGetResourceReader {
			get { return readerCultureInfo; }
		}
		
		public IResourceWriter GetResourceWriter(CultureInfo info)
		{
			writerCultureInfo = info;
			return writer;
		}
		
		/// <summary>
		/// Gets the culture passed to GetResourceWriter.
		/// </summary>
		public CultureInfo CultureInfoPassedToGetResourceWriter {
			get { return writerCultureInfo; }
		}		
	}
}
