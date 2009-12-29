// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Resources;

namespace RubyBinding.Tests.Utils
{
	public class MockResourceService : IResourceService
	{
		IResourceReader reader;
		IResourceWriter writer;
		CultureInfo readerCultureInfo;
		CultureInfo writerCultureInfo;
		
		public MockResourceService()
		{
//			reader = new MockResourceReader();
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
		public CultureInfo ResourceReaderCultureInfo {
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
		public CultureInfo ResourceWriterCultureInfo {
			get { return writerCultureInfo; }
		}		
	}
}
