// *****************************************************************************
// 
//  Copyright 2004, Weifen Luo
//  All rights reserved. The software and associated documentation 
//  supplied hereunder are the proprietary information of Weifen Luo
//  and are supplied subject to licence terms.
// 
//  WinFormsUI Library Version 1.0
// *****************************************************************************

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using WeifenLuo.WinFormsUI;
using System.IO;
using System.Text;
using System.Xml;
using System.Globalization;

namespace WeifenLuo.WinFormsUI
{
	internal class DockPanelPersist
	{
		private const string ConfigFileVersion = "0.9.3";

		private class DummyContent : DockContent
		{
		}

		private struct DockPanelStruct
		{
			private double m_dockLeftPortion;
			public double DockLeftPortion
			{
				get	{	return m_dockLeftPortion;	}
				set	{	m_dockLeftPortion = value;	}
			}

			private double m_dockRightPortion;
			public double DockRightPortion
			{
				get	{	return m_dockRightPortion;	}
				set	{	m_dockRightPortion = value;	}
			}

			private double m_dockTopPortion;
			public double DockTopPortion
			{
				get	{	return m_dockTopPortion;	}
				set	{	m_dockTopPortion = value;	}
			}

			private double m_dockBottomPortion;
			public double DockBottomPortion
			{
				get	{	return m_dockBottomPortion;	}
				set	{	m_dockBottomPortion = value;	}
			}

			private int m_indexActiveDocumentPane;
			public int IndexActiveDocumentPane
			{
				get	{	return m_indexActiveDocumentPane;	}
				set	{	m_indexActiveDocumentPane = value;	}
			}

			private int m_indexActivePane;
			public int IndexActivePane
			{
				get	{	return m_indexActivePane;	}
				set	{	m_indexActivePane = value;	}
			}
		}

		private struct ContentStruct
		{
			private string m_persistString;
			public string PersistString
			{
				get	{	return m_persistString;	}
				set	{	m_persistString = value;	}
			}

			private double m_autoHidePortion;
			public double AutoHidePortion
			{
				get	{	return m_autoHidePortion;	}
				set	{	m_autoHidePortion = value;	}
			}

			private bool m_isHidden;
			public bool IsHidden
			{
				get	{	return m_isHidden;	}
				set	{	m_isHidden = value;	}
			}

			private bool m_isFloat;
			public bool IsFloat
			{
				get	{	return m_isFloat;	}
				set	{	m_isFloat = value;	}
			}
		}

		private struct PaneStruct
		{
			private DockState m_dockState;
			public DockState DockState
			{
				get	{	return m_dockState;	}
				set	{	m_dockState = value;	}
			}

			private int m_indexActiveContent;
			public int IndexActiveContent
			{
				get	{	return m_indexActiveContent;	}
				set	{	m_indexActiveContent = value;	}
			}

			private int[] m_indexContents;
			public int[] IndexContents
			{
				get	{	return m_indexContents;	}
				set	{	m_indexContents = value;	}
			}

			private int m_zOrderIndex;
			public int ZOrderIndex
			{
				get	{	return m_zOrderIndex;	}
				set	{	m_zOrderIndex = value;	}
			}
		}

		private struct DockListItem
		{
			private int m_indexPane;
			public int IndexPane
			{
				get	{	return m_indexPane;	}
				set	{	m_indexPane = value;	}
			}

			private int m_indexPrevPane;
			public int IndexPrevPane
			{
				get	{	return m_indexPrevPane;	}
				set	{	m_indexPrevPane = value;	}
			}

			private DockAlignment m_alignment;
			public DockAlignment Alignment
			{
				get	{	return m_alignment;	}
				set	{	m_alignment = value;	}
			}

			private double m_proportion;
			public double Proportion
			{
				get	{	return m_proportion;	}
				set	{	m_proportion = value;	}
			}
		}

		private struct DockWindowStruct
		{
			private DockState m_dockState;
			public DockState DockState
			{
				get	{	return m_dockState;	}
				set	{	m_dockState = value;	}
			}

			private int m_zOrderIndex;
			public int ZOrderIndex
			{
				get	{	return m_zOrderIndex;	}
				set	{	m_zOrderIndex = value;	}
			}

			private DockListItem[] m_dockList;
			public DockListItem[] DockList
			{
				get	{	return m_dockList;	}
				set	{	m_dockList = value;	}
			}
		}

		private struct FloatWindowStruct
		{
			private Rectangle m_bounds;
			public Rectangle Bounds
			{
				get	{	return m_bounds;	}
				set	{	m_bounds = value;	}
			}
			
			private bool m_allowRedocking;
			public bool AllowRedocking
			{
				get	{	return m_allowRedocking;	}
				set	{	m_allowRedocking = value;	}
			}

			private int m_zOrderIndex;
			public int ZOrderIndex
			{
				get	{	return m_zOrderIndex;	}
				set	{	m_zOrderIndex = value;	}
			}

			private DockListItem[] m_dockList;
			public DockListItem[] DockList
			{
				get	{	return m_dockList;	}
				set	{	m_dockList = value;	}
			}
		}

		public DockPanelPersist()
		{
		}

		public static void SaveAsXml(DockPanel dockPanel, string filename)
		{
			SaveAsXml(dockPanel, filename, Encoding.Unicode);
		}

		public static void SaveAsXml(DockPanel dockPanel, string filename, Encoding encoding)
		{
			FileStream fs = new FileStream(filename, FileMode.Create);
			try
			{
				SaveAsXml(dockPanel, fs, encoding);
			}
			finally
			{
				fs.Close();
			}
		}

		public static void SaveAsXml(DockPanel dockPanel, Stream stream, Encoding encoding)
		{
			XmlTextWriter xmlOut = new XmlTextWriter(stream, encoding); 

			// Use indenting for readability
			xmlOut.Formatting = Formatting.Indented;
			
			// Always begin file with identification and warning
			xmlOut.WriteStartDocument();
			xmlOut.WriteComment(" DockPanel configuration file. Author: Weifen Luo, all rights reserved. ");
			xmlOut.WriteComment(" !!! AUTOMATICALLY GENERATED FILE. DO NOT MODIFY !!! ");

			// Associate a version number with the root element so that future version of the code
			// will be able to be backwards compatible or at least recognise out of date versions
			xmlOut.WriteStartElement("DockPanel");
			xmlOut.WriteAttributeString("FormatVersion", ConfigFileVersion);
			xmlOut.WriteAttributeString("DockLeftPortion", dockPanel.DockLeftPortion.ToString(CultureInfo.InvariantCulture));
			xmlOut.WriteAttributeString("DockRightPortion", dockPanel.DockRightPortion.ToString(CultureInfo.InvariantCulture));
			xmlOut.WriteAttributeString("DockTopPortion", dockPanel.DockTopPortion.ToString(CultureInfo.InvariantCulture));
			xmlOut.WriteAttributeString("DockBottomPortion", dockPanel.DockBottomPortion.ToString(CultureInfo.InvariantCulture));
			xmlOut.WriteAttributeString("ActiveDocumentPane", dockPanel.Panes.IndexOf(dockPanel.ActiveDocumentPane).ToString());
			xmlOut.WriteAttributeString("ActivePane", dockPanel.Panes.IndexOf(dockPanel.ActivePane).ToString());

			// Contents
			xmlOut.WriteStartElement("Contents");
			xmlOut.WriteAttributeString("Count", dockPanel.Contents.Count.ToString());
			foreach (DockContent content in dockPanel.Contents)
			{
				xmlOut.WriteStartElement("Content");
				xmlOut.WriteAttributeString("ID", dockPanel.Contents.IndexOf(content).ToString());
				xmlOut.WriteAttributeString("PersistString", content.PersistString);
				xmlOut.WriteAttributeString("AutoHidePortion", content.AutoHidePortion.ToString(CultureInfo.InvariantCulture));
				xmlOut.WriteAttributeString("IsHidden", content.IsHidden.ToString());
				xmlOut.WriteAttributeString("IsFloat", content.IsFloat.ToString());
				xmlOut.WriteEndElement();
			}
			xmlOut.WriteEndElement();

			// Panes
			xmlOut.WriteStartElement("Panes");
			xmlOut.WriteAttributeString("Count", dockPanel.Panes.Count.ToString());
			foreach (DockPane pane in dockPanel.Panes)
			{
				xmlOut.WriteStartElement("Pane");
				xmlOut.WriteAttributeString("ID", dockPanel.Panes.IndexOf(pane).ToString());
				xmlOut.WriteAttributeString("DockState", pane.DockState.ToString());
				xmlOut.WriteAttributeString("ActiveContent", dockPanel.Contents.IndexOf(pane.ActiveContent).ToString());
				xmlOut.WriteStartElement("Contents");
				xmlOut.WriteAttributeString("Count", pane.Contents.Count.ToString());
				foreach (DockContent content in pane.Contents)
				{
					xmlOut.WriteStartElement("Content");
					xmlOut.WriteAttributeString("ID", pane.Contents.IndexOf(content).ToString());
					xmlOut.WriteAttributeString("RefID", dockPanel.Contents.IndexOf(content).ToString());
					xmlOut.WriteEndElement();
				}
				xmlOut.WriteEndElement();
				xmlOut.WriteEndElement();
			}
			xmlOut.WriteEndElement();

			// DockWindows
			xmlOut.WriteStartElement("DockWindows");
			int count = 0;
			foreach (DockWindow dw in dockPanel.DockWindows)
				if (dw.DockList.Count > 0)
					count++;
			xmlOut.WriteAttributeString("Count", count.ToString());
			int i = 0;
			foreach (DockWindow dw in dockPanel.DockWindows)
			{
				if (dw.DockList.Count == 0)
					continue;
				xmlOut.WriteStartElement("DockWindow");
				xmlOut.WriteAttributeString("ID", i.ToString());
				xmlOut.WriteAttributeString("DockState", dw.DockState.ToString());
				xmlOut.WriteAttributeString("ZOrderIndex", dockPanel.Controls.IndexOf(dw).ToString());
				xmlOut.WriteStartElement("DockList");
				xmlOut.WriteAttributeString("Count", dw.DockList.Count.ToString());
				foreach (DockPane pane in dw.DockList)
				{
					xmlOut.WriteStartElement("Pane");
					xmlOut.WriteAttributeString("ID", dw.DockList.IndexOf(pane).ToString());
					xmlOut.WriteAttributeString("RefID", dockPanel.Panes.IndexOf(pane).ToString());
					NestedDockingStatus status = pane.NestedDockingStatus;
					xmlOut.WriteAttributeString("PrevPane", dockPanel.Panes.IndexOf(status.PrevPane).ToString());
					xmlOut.WriteAttributeString("Alignment", status.Alignment.ToString());
					xmlOut.WriteAttributeString("Proportion", status.Proportion.ToString(CultureInfo.InvariantCulture));
					xmlOut.WriteEndElement();
				}
				xmlOut.WriteEndElement();
				xmlOut.WriteEndElement();
				i++;
			}
			xmlOut.WriteEndElement();

			// FloatWindows
			RectangleConverter rectConverter = new RectangleConverter();
			xmlOut.WriteStartElement("FloatWindows");
			xmlOut.WriteAttributeString("Count", dockPanel.FloatWindows.Count.ToString());
			foreach (FloatWindow fw in dockPanel.FloatWindows)
			{
				xmlOut.WriteStartElement("FloatWindow");
				xmlOut.WriteAttributeString("ID", dockPanel.FloatWindows.IndexOf(fw).ToString());
				xmlOut.WriteAttributeString("Bounds", rectConverter.ConvertToInvariantString(fw.Bounds));
				xmlOut.WriteAttributeString("AllowRedocking", fw.AllowRedocking.ToString());
				xmlOut.WriteAttributeString("ZOrderIndex", fw.DockPanel.FloatWindows.IndexOf(fw).ToString());
				xmlOut.WriteStartElement("DockList");
				xmlOut.WriteAttributeString("Count", fw.DockList.Count.ToString());
				foreach (DockPane pane in fw.DockList)
				{
					xmlOut.WriteStartElement("Pane");
					xmlOut.WriteAttributeString("ID", fw.DockList.IndexOf(pane).ToString());
					xmlOut.WriteAttributeString("RefID", dockPanel.Panes.IndexOf(pane).ToString());
					NestedDockingStatus status = pane.NestedDockingStatus;
					xmlOut.WriteAttributeString("PrevPane", dockPanel.Panes.IndexOf(status.PrevPane).ToString());
					xmlOut.WriteAttributeString("Alignment", status.Alignment.ToString());
					xmlOut.WriteAttributeString("Proportion", status.Proportion.ToString(CultureInfo.InvariantCulture));
					xmlOut.WriteEndElement();
				}
				xmlOut.WriteEndElement();
				xmlOut.WriteEndElement();
			}
			xmlOut.WriteEndElement();	//	</FloatWindows>
			
			xmlOut.WriteEndElement();
			xmlOut.WriteEndDocument();

			// This should flush all actions and close the file
			xmlOut.Close();
		}

		public static void LoadFromXml(DockPanel dockPanel, string filename, DeserializeDockContent deserializeContent)
		{
			FileStream fs = new FileStream(filename, FileMode.Open);
			try
			{
				LoadFromXml(dockPanel, fs, deserializeContent);
			}
			finally
			{
				fs.Close();
			}
		}

		public static void LoadFromXml(DockPanel dockPanel, Stream stream, DeserializeDockContent deserializeContent)
		{

			if (dockPanel.Contents.Count != 0)
				throw new InvalidOperationException(ResourceHelper.GetString("DockPanel.LoadFromXml.AlreadyInitialized"));

			EnumConverter dockStateConverter = new EnumConverter(typeof(DockState));
			EnumConverter dockAlignmentConverter = new EnumConverter(typeof(DockAlignment));
			RectangleConverter rectConverter = new RectangleConverter();

			XmlTextReader xmlIn = new XmlTextReader(stream);
			xmlIn.WhitespaceHandling = WhitespaceHandling.None;
			xmlIn.MoveToContent();

			if (xmlIn.Name != "DockPanel")
				throw new ArgumentException(ResourceHelper.GetString("DockPanel.LoadFromXml.InvalidXmlFormat"));

			string formatVersion = xmlIn.GetAttribute("FormatVersion");
			if (formatVersion != ConfigFileVersion)
				throw new ArgumentException(ResourceHelper.GetString("DockPanel.LoadFromXml.InvalidFormatVersion"));
			DockPanelStruct dockPanelStruct = new DockPanelStruct();
			dockPanelStruct.DockLeftPortion = Convert.ToDouble(xmlIn.GetAttribute("DockLeftPortion"), CultureInfo.InvariantCulture);
			dockPanelStruct.DockRightPortion = Convert.ToDouble(xmlIn.GetAttribute("DockRightPortion"), CultureInfo.InvariantCulture);
			dockPanelStruct.DockTopPortion = Convert.ToDouble(xmlIn.GetAttribute("DockTopPortion"), CultureInfo.InvariantCulture);
			dockPanelStruct.DockBottomPortion = Convert.ToDouble(xmlIn.GetAttribute("DockBottomPortion"), CultureInfo.InvariantCulture);
			dockPanelStruct.IndexActiveDocumentPane = Convert.ToInt32(xmlIn.GetAttribute("ActiveDocumentPane"));
			dockPanelStruct.IndexActivePane = Convert.ToInt32(xmlIn.GetAttribute("ActivePane"));

			// Load Contents
			MoveToNextElement(xmlIn);
			if (xmlIn.Name != "Contents")
				throw new ArgumentException(ResourceHelper.GetString("DockPanel.LoadFromXml.InvalidXmlFormat"));
			int countOfContents = Convert.ToInt32(xmlIn.GetAttribute("Count"));
			ContentStruct[] contents = new ContentStruct[countOfContents];
			MoveToNextElement(xmlIn);
			for (int i=0; i<countOfContents; i++)
			{
				int id = Convert.ToInt32(xmlIn.GetAttribute("ID"));
				if (xmlIn.Name != "Content" || id != i)
					throw new ArgumentException(ResourceHelper.GetString("DockPanel.LoadFromXml.InvalidXmlFormat"));

				contents[i].PersistString = xmlIn.GetAttribute("PersistString");
				contents[i].AutoHidePortion = Convert.ToDouble(xmlIn.GetAttribute("AutoHidePortion"), CultureInfo.InvariantCulture);
				contents[i].IsHidden = Convert.ToBoolean(xmlIn.GetAttribute("IsHidden"));
				contents[i].IsFloat = Convert.ToBoolean(xmlIn.GetAttribute("IsFloat"));
				MoveToNextElement(xmlIn);
			}

			// Load Panes
			if (xmlIn.Name != "Panes")
				throw new ArgumentException(ResourceHelper.GetString("DockPanel.LoadFromXml.InvalidXmlFormat"));
			int countOfPanes = Convert.ToInt32(xmlIn.GetAttribute("Count"));
			PaneStruct[] panes = new PaneStruct[countOfPanes];
			MoveToNextElement(xmlIn);
			for (int i=0; i<countOfPanes; i++)
			{
				int id = Convert.ToInt32(xmlIn.GetAttribute("ID"));
				if (xmlIn.Name != "Pane" || id != i)
					throw new ArgumentException(ResourceHelper.GetString("DockPanel.LoadFromXml.InvalidXmlFormat"));

				panes[i].DockState = (DockState)dockStateConverter.ConvertFrom(xmlIn.GetAttribute("DockState"));
				panes[i].IndexActiveContent = Convert.ToInt32(xmlIn.GetAttribute("ActiveContent"));
				panes[i].ZOrderIndex = -1;

				MoveToNextElement(xmlIn);
				if (xmlIn.Name != "Contents")
					throw new ArgumentException(ResourceHelper.GetString("DockPanel.LoadFromXml.InvalidXmlFormat"));
				int countOfPaneContents = Convert.ToInt32(xmlIn.GetAttribute("Count"));
				panes[i].IndexContents = new int[countOfPaneContents];
				MoveToNextElement(xmlIn);
				for (int j=0; j<countOfPaneContents; j++)
				{
					int id2 = Convert.ToInt32(xmlIn.GetAttribute("ID"));
					if (xmlIn.Name != "Content" || id2 != j)
						throw new ArgumentException(ResourceHelper.GetString("DockPanel.LoadFromXml.InvalidXmlFormat"));

					panes[i].IndexContents[j] = Convert.ToInt32(xmlIn.GetAttribute("RefID"));
					MoveToNextElement(xmlIn);
				}
			}

			// Load DockWindows
			if (xmlIn.Name != "DockWindows")
				throw new ArgumentException(ResourceHelper.GetString("DockPanel.LoadFromXml.InvalidXmlFormat"));
			int countOfDockWindows = Convert.ToInt32(xmlIn.GetAttribute("Count"));
			DockWindowStruct[] dockWindows = new DockWindowStruct[countOfDockWindows];
			MoveToNextElement(xmlIn);
			for (int i=0; i<countOfDockWindows; i++)
			{
				int id = Convert.ToInt32(xmlIn.GetAttribute("ID"));
				if (xmlIn.Name != "DockWindow" || id != i)
					throw new ArgumentException(ResourceHelper.GetString("DockPanel.LoadFromXml.InvalidXmlFormat"));

				dockWindows[i].DockState = (DockState)dockStateConverter.ConvertFrom(xmlIn.GetAttribute("DockState"));
				dockWindows[i].ZOrderIndex = Convert.ToInt32(xmlIn.GetAttribute("ZOrderIndex"));
				MoveToNextElement(xmlIn);
				if (xmlIn.Name != "DockList")
					throw new ArgumentException(ResourceHelper.GetString("DockPanel.LoadFromXml.InvalidXmlFormat"));
				int countOfDockList = Convert.ToInt32(xmlIn.GetAttribute("Count"));
				dockWindows[i].DockList = new DockListItem[countOfDockList];
				MoveToNextElement(xmlIn);
				for (int j=0; j<countOfDockList; j++)
				{
					int id2 = Convert.ToInt32(xmlIn.GetAttribute("ID"));
					if (xmlIn.Name != "Pane" || id2 != j)
						throw new ArgumentException(ResourceHelper.GetString("DockPanel.LoadFromXml.InvalidXmlFormat"));
					dockWindows[i].DockList[j].IndexPane = Convert.ToInt32(xmlIn.GetAttribute("RefID"));
					dockWindows[i].DockList[j].IndexPrevPane = Convert.ToInt32(xmlIn.GetAttribute("PrevPane"));
					dockWindows[i].DockList[j].Alignment = (DockAlignment)dockAlignmentConverter.ConvertFrom(xmlIn.GetAttribute("Alignment"));
					dockWindows[i].DockList[j].Proportion = Convert.ToDouble(xmlIn.GetAttribute("Proportion"), CultureInfo.InvariantCulture);
					MoveToNextElement(xmlIn);
				}
			}

			// Load FloatWindows
			if (xmlIn.Name != "FloatWindows")
				throw new ArgumentException(ResourceHelper.GetString("DockPanel.LoadFromXml.InvalidXmlFormat"));
			int countOfFloatWindows = Convert.ToInt32(xmlIn.GetAttribute("Count"));
			FloatWindowStruct[] floatWindows = new FloatWindowStruct[countOfFloatWindows];
			MoveToNextElement(xmlIn);
			for (int i=0; i<countOfFloatWindows; i++)
			{
				int id = Convert.ToInt32(xmlIn.GetAttribute("ID"));
				if (xmlIn.Name != "FloatWindow" || id != i)
					throw new ArgumentException(ResourceHelper.GetString("DockPanel.LoadFromXml.InvalidXmlFormat"));

				floatWindows[i].Bounds = (Rectangle)rectConverter.ConvertFromInvariantString(xmlIn.GetAttribute("Bounds"));
				floatWindows[i].AllowRedocking = Convert.ToBoolean(xmlIn.GetAttribute("AllowRedocking"));
				floatWindows[i].ZOrderIndex = Convert.ToInt32(xmlIn.GetAttribute("ZOrderIndex"));
				MoveToNextElement(xmlIn);
				if (xmlIn.Name != "DockList")
					throw new ArgumentException(ResourceHelper.GetString("DockPanel.LoadFromXml.InvalidXmlFormat"));
				int countOfDockList = Convert.ToInt32(xmlIn.GetAttribute("Count"));
				floatWindows[i].DockList = new DockListItem[countOfDockList];
				MoveToNextElement(xmlIn);
				for (int j=0; j<countOfDockList; j++)
				{
					int id2 = Convert.ToInt32(xmlIn.GetAttribute("ID"));
					if (xmlIn.Name != "Pane" || id2 != j)
						throw new ArgumentException(ResourceHelper.GetString("DockPanel.LoadFromXml.InvalidXmlFormat"));
					floatWindows[i].DockList[j].IndexPane = Convert.ToInt32(xmlIn.GetAttribute("RefID"));
					floatWindows[i].DockList[j].IndexPrevPane = Convert.ToInt32(xmlIn.GetAttribute("PrevPane"));
					floatWindows[i].DockList[j].Alignment = (DockAlignment)dockAlignmentConverter.ConvertFrom(xmlIn.GetAttribute("Alignment"));
					floatWindows[i].DockList[j].Proportion = Convert.ToDouble(xmlIn.GetAttribute("Proportion"), CultureInfo.InvariantCulture);
					MoveToNextElement(xmlIn);
				}
			}
			xmlIn.Close();
					
			dockPanel.DockLeftPortion = dockPanelStruct.DockLeftPortion;
			dockPanel.DockRightPortion = dockPanelStruct.DockRightPortion;
			dockPanel.DockTopPortion = dockPanelStruct.DockTopPortion;
			dockPanel.DockBottomPortion = dockPanelStruct.DockBottomPortion;

			// Create Contents
			for (int i=0; i<contents.Length; i++)
			{
				DockContent content = deserializeContent(contents[i].PersistString);
				if (content == null)
					content = new DummyContent();
				content.DockPanel = dockPanel;
				content.AutoHidePortion = contents[i].AutoHidePortion;
				content.IsHidden = true;
				content.IsFloat = contents[i].IsFloat;
			}

			// Create panes
			for (int i=0; i<panes.Length; i++)
			{
				DockPane pane = null;
				for (int j=0; j<panes[i].IndexContents.Length; j++)
				{
					DockContent content = dockPanel.Contents[panes[i].IndexContents[j]];
					if (j==0)
						pane = dockPanel.DockPaneFactory.CreateDockPane(content, panes[i].DockState, false);
					else if (panes[i].DockState == DockState.Float)
						content.FloatPane = pane;
					else
						content.PanelPane = pane;
				}
			}

			// Assign Panes to DockWindows
			for (int i=0; i<dockWindows.Length; i++)
			{
				for (int j=0; j<dockWindows[i].DockList.Length; j++)
				{
					DockWindow dw = dockPanel.DockWindows[dockWindows[i].DockState];
					int indexPane = dockWindows[i].DockList[j].IndexPane;
					DockPane pane = dockPanel.Panes[indexPane];
					int indexPrevPane = dockWindows[i].DockList[j].IndexPrevPane;
					DockPane prevPane = (indexPrevPane == -1) ? dw.DockList.GetDefaultPrevPane(pane) : dockPanel.Panes[indexPrevPane];
					DockAlignment alignment = dockWindows[i].DockList[j].Alignment;
					double proportion = dockWindows[i].DockList[j].Proportion;
					pane.AddToDockList(dw, prevPane, alignment, proportion);
					if (panes[indexPane].DockState == dw.DockState)
						panes[indexPane].ZOrderIndex = dockWindows[i].ZOrderIndex;
				}
			}

			// Create float windows
			for (int i=0; i<floatWindows.Length; i++)
			{
				FloatWindow fw = null;
				for (int j=0; j<floatWindows[i].DockList.Length; j++)
				{
					int indexPane = floatWindows[i].DockList[j].IndexPane;
					DockPane pane = dockPanel.Panes[indexPane];
					if (j == 0)
						fw = dockPanel.FloatWindowFactory.CreateFloatWindow(dockPanel, pane, floatWindows[i].Bounds);
					else
					{
						int indexPrevPane = floatWindows[i].DockList[j].IndexPrevPane;
						DockPane prevPane = indexPrevPane == -1 ? null : dockPanel.Panes[indexPrevPane];
						DockAlignment alignment = floatWindows[i].DockList[j].Alignment;
						double proportion = floatWindows[i].DockList[j].Proportion;
						pane.AddToDockList(fw, prevPane, alignment, proportion);
						if (panes[indexPane].DockState == fw.DockState)
							panes[indexPane].ZOrderIndex = floatWindows[i].ZOrderIndex;
					}
				}
			}

			for (int i=0; i<contents.Length; i++)
				dockPanel.Contents[i].IsHidden = contents[i].IsHidden;

			if (panes.Length > 0)
			{
				int[] sortedPanes = new int[panes.Length];
				for (int i=0; i<panes.Length; i++)
					sortedPanes[i] = i;

				for (int i=0; i<panes.Length-1; i++)
					for (int j=i+1; j<panes.Length; j++)
						if (panes[sortedPanes[j]].ZOrderIndex  < panes[sortedPanes[i]].ZOrderIndex)
						{
							int temp = sortedPanes[i];
							sortedPanes[i] = sortedPanes[j];
							sortedPanes[j] = temp;
						}
				
				for (int i=0; i<sortedPanes.Length; i++)
				{
					int index = sortedPanes[i];
					dockPanel.Panes[index].DockState = panes[index].DockState;
					dockPanel.Panes[index].ActiveContent = panes[index].IndexActiveContent == -1 ? null : dockPanel.Contents[panes[index].IndexActiveContent];
				}
			}

			if (dockPanelStruct.IndexActiveDocumentPane != -1)
				dockPanel.Panes[dockPanelStruct.IndexActiveDocumentPane].Activate();

			if (dockPanelStruct.IndexActivePane != -1)
				dockPanel.Panes[dockPanelStruct.IndexActivePane].Activate();

			for (int i=dockPanel.Contents.Count-1; i>=0; i--)
				if (dockPanel.Contents[i] is DummyContent)
					dockPanel.Contents[i].Close();
		}

		private static void MoveToNextElement(XmlTextReader xmlIn)
		{
			xmlIn.Read();
			while (xmlIn.NodeType == XmlNodeType.EndElement)
				xmlIn.Read();
		}
	}
}
