using System;

namespace DockSample
{
	/// <summary>
	/// Summary description for Options.
	/// </summary>
	public class Options
	{
		public Options()
		{
		}

		private bool m_activeDocumentChanged = false;
		public bool ActiveDocumentChanged
		{
			get	{	return m_activeDocumentChanged;	}
			set	{	m_activeDocumentChanged = value;	}
		}

		private bool m_contentAdded = false;
		public bool ContentAdded
		{
			get	{	return m_contentAdded;	}
			set	{	m_contentAdded = value;	}
		}

		private bool m_contentRemoved = false;
		public bool ContentRemoved
		{
			get	{	return m_contentRemoved;	}
			set	{	m_contentRemoved = value;	}
		}
	}
}
