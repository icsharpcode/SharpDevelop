// LineCounterBrowser is from the CodeProject article
// "Line Counter - Writing a Visual Studio 2005 Add-In" by Jon Rista
// http://www.codeproject.com/useritems/LineCounterAddin.asp

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop;

namespace LineCounterAddin
{
	/// <summary>
	/// Line Counter add-in user interface.
	/// </summary>
	public partial class LineCounterBrowser : UserControl
	{
		#region Nested Classes
		abstract class ListViewItemComparer : System.Collections.IComparer
		{
			public abstract int Compare(ListViewItem item1, ListViewItem item2);

			public ListView SortingList;
			public int Column;

			#region IComparer Members
			int System.Collections.IComparer.Compare(object x, object y)
			{
				if (x is ListViewItem && y is ListViewItem)
				{
					int diff = Compare((ListViewItem)x, (ListViewItem)y);
					if (SortingList.Sorting == SortOrder.Descending)
						diff *= -1;

					return diff;
				}
				else
				{
					throw new ArgumentException("One or both of the arguments are not ListViewItem objects.");
				}
			}
			#endregion
		}

		/// <summary>
		/// Compares items based on file name.
		/// </summary>
		class FileNameComparer : ListViewItemComparer
		{

			public override int Compare(ListViewItem item1, ListViewItem item2)
			{
				return String.Compare(item1.Text, item2.Text, false);
			}
		}

		/// <summary>
		/// Compares items based on total lines primarily, and
		/// the filename secondarily.
		/// </summary>
		class FileLinesComparer : ListViewItemComparer
		{

			public override int Compare(ListViewItem item1, ListViewItem item2)
			{
				string string1 = item1.SubItems[Column].Text;
				string string2 = item2.SubItems[Column].Text;

				if (string1 != null && string2 != null)
				{
					int total1 = int.Parse(string1);
					int total2 = int.Parse(string2);

					// Compare the totals...
					int diff = total1 - total2;

					// If totals are equal...
					if (diff == 0)
					{
						// Compare the filenames
						diff = String.Compare(item1.Text, item2.Text, false);
					}

					return diff;
				}

				return 0;
			}
		}

		/// <summary>
		/// Compares items based on file extension.
		/// </summary>
		class FileExtensionComparer : ListViewItemComparer
		{

			public override int Compare(ListViewItem item1, ListViewItem item2)
			{
				string string1 = item1.SubItems[4].Text;
				string string2 = item2.SubItems[4].Text;

				return String.Compare(string1, string2, true);
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Construct the line counter user interface and
		/// the countable file type mappings (to icons and
		/// counting algorithms).
		/// </summary>
		public LineCounterBrowser()
		{
			InitializeComponent();
			
			#if IMPR1
			projectImageListHelper = new ImageListHelper(imgProjectTypes);
			fileImageListHelper    = new ImageListHelper(imgFileTypes);
			#endif

			// Map project types to icons for use in the projects list
			m_projIconMappings = new Dictionary<string, int>();
			m_projIconMappings.Add("{00000000-0000-0000-0000-000000000000}", 0);
			m_projIconMappings.Add("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", 1); // C#
			m_projIconMappings.Add("{F184B08F-C81C-45F6-A57F-5ABD9991F28F}", 2); // VB
			m_projIconMappings.Add("{00000001-0000-0000-0000-000000000000}", 5);

			// List all the countable file types (so we don't try to count .dll's,
			// images, executables, etc.
			
			m_countableTypes = new System.Collections.Specialized.StringCollection();
			#if IMPR2
			countingAlgorithms = AddInTree.BuildItems<CountingAlgorithmDescriptor>
				("/AddIns/LineCounter/CountingAlgorithms", this);
			// Iterate through algorithms to fill list of known countable types
			foreach (CountingAlgorithmDescriptor desc in countingAlgorithms) {
				m_countableTypes.AddRange(desc.extensions);
			}
			#else
			m_countableTypes.Add("*");
			m_countableTypes.Add(".cs");
			m_countableTypes.Add(".vb");
			m_countableTypes.Add(".vj");
			m_countableTypes.Add(".cpp");
			m_countableTypes.Add(".cc");
			m_countableTypes.Add(".cxx");
			m_countableTypes.Add(".c");
			m_countableTypes.Add(".hpp");
			m_countableTypes.Add(".hh");
			m_countableTypes.Add(".hxx");
			m_countableTypes.Add(".h");
			m_countableTypes.Add(".js");
			m_countableTypes.Add(".cd");
			m_countableTypes.Add(".resx");
			m_countableTypes.Add(".res");
			m_countableTypes.Add(".css");
			m_countableTypes.Add(".htm");
			m_countableTypes.Add(".html");
			m_countableTypes.Add(".xml");
			m_countableTypes.Add(".xsl");
			m_countableTypes.Add(".xslt");
			m_countableTypes.Add(".xsd");
			m_countableTypes.Add(".config");
			m_countableTypes.Add(".asax");
			m_countableTypes.Add(".ascx");
			m_countableTypes.Add(".asmx");
			m_countableTypes.Add(".aspx");
			m_countableTypes.Add(".ashx");
			m_countableTypes.Add(".idl");
			m_countableTypes.Add(".odl");
			m_countableTypes.Add(".txt");
			m_countableTypes.Add(".sql");
			#endif

			// Map file extensions to icons for use in the file list
			m_fileIconMappings = new Dictionary<string, int>(33);
			m_fileIconMappings.Add("*", 0);
			m_fileIconMappings.Add(".cs", 1);
			m_fileIconMappings.Add(".vb", 2);
			m_fileIconMappings.Add(".vj", 3);
			m_fileIconMappings.Add(".cpp", 4);
			m_fileIconMappings.Add(".cc", 4);
			m_fileIconMappings.Add(".cxx", 4);
			m_fileIconMappings.Add(".c", 5);
			m_fileIconMappings.Add(".hpp", 6);
			m_fileIconMappings.Add(".hh", 6);
			m_fileIconMappings.Add(".hxx", 6);
			m_fileIconMappings.Add(".h", 6);
			m_fileIconMappings.Add(".js", 7);
			m_fileIconMappings.Add(".cd", 8);
			m_fileIconMappings.Add(".resx", 9);
			m_fileIconMappings.Add(".res", 9);
			m_fileIconMappings.Add(".css", 10);
			m_fileIconMappings.Add(".htm", 11);
			m_fileIconMappings.Add(".html", 11);
			m_fileIconMappings.Add(".xml", 12);
			m_fileIconMappings.Add(".xsl", 13);
			m_fileIconMappings.Add(".xslt", 13);
			m_fileIconMappings.Add(".xsd", 14);
			m_fileIconMappings.Add(".config", 15);
			m_fileIconMappings.Add(".asax", 16);
			m_fileIconMappings.Add(".ascx", 17);
			m_fileIconMappings.Add(".asmx", 18);
			m_fileIconMappings.Add(".aspx", 19);
			m_fileIconMappings.Add(".ashx", 0);
			m_fileIconMappings.Add(".idl", 0);
			m_fileIconMappings.Add(".odl", 0);
			m_fileIconMappings.Add(".txt", 0);
			m_fileIconMappings.Add(".sql", 0);

			// Prepare counting algorithm mappings
			CountLines countLinesGeneric = new CountLines(CountLinesGeneric);
			CountLines countLinesCStyle = new CountLines(CountLinesCStyle);
			CountLines countLinesVBStyle = new CountLines(CountLinesVBStyle);
			CountLines countLinesXMLStyle = new CountLines(CountLinesXMLStyle);

			m_countAlgorithms = new Dictionary<string, CountLines>(33);
			m_countAlgorithms.Add("*", countLinesGeneric);
			m_countAlgorithms.Add(".cs", countLinesCStyle);
			m_countAlgorithms.Add(".vb", countLinesVBStyle);
			m_countAlgorithms.Add(".vj", countLinesCStyle);
			m_countAlgorithms.Add(".js", countLinesCStyle);
			m_countAlgorithms.Add(".cpp", countLinesCStyle);
			m_countAlgorithms.Add(".cc", countLinesCStyle);
			m_countAlgorithms.Add(".cxx", countLinesCStyle);
			m_countAlgorithms.Add(".c", countLinesCStyle);
			m_countAlgorithms.Add(".hpp", countLinesCStyle);
			m_countAlgorithms.Add(".hh", countLinesCStyle);
			m_countAlgorithms.Add(".hxx", countLinesCStyle);
			m_countAlgorithms.Add(".h", countLinesCStyle);
			m_countAlgorithms.Add(".idl", countLinesCStyle);
			m_countAlgorithms.Add(".odl", countLinesCStyle);
			m_countAlgorithms.Add(".txt", countLinesGeneric);
			m_countAlgorithms.Add(".xml", countLinesXMLStyle);
			m_countAlgorithms.Add(".xsl", countLinesXMLStyle);
			m_countAlgorithms.Add(".xslt", countLinesXMLStyle);
			m_countAlgorithms.Add(".xsd", countLinesXMLStyle);
			m_countAlgorithms.Add(".config", countLinesXMLStyle);
			m_countAlgorithms.Add(".res", countLinesGeneric);
			m_countAlgorithms.Add(".resx", countLinesXMLStyle);
			m_countAlgorithms.Add(".aspx", countLinesXMLStyle);
			m_countAlgorithms.Add(".ascx", countLinesXMLStyle);
			m_countAlgorithms.Add(".ashx", countLinesXMLStyle);
			m_countAlgorithms.Add(".asmx", countLinesXMLStyle);
			m_countAlgorithms.Add(".asax", countLinesXMLStyle);
			m_countAlgorithms.Add(".htm", countLinesXMLStyle);
			m_countAlgorithms.Add(".html", countLinesXMLStyle);
			m_countAlgorithms.Add(".css", countLinesCStyle);
			m_countAlgorithms.Add(".sql", countLinesGeneric);
			m_countAlgorithms.Add(".cd", countLinesGeneric);
		}
		#endregion

		#region Variables
		private List<LineCountSummary> m_summaryList;
		private Dictionary<string, int> m_projIconMappings;
		private Dictionary<string, int> m_fileIconMappings;
		private Dictionary<string, CountLines> m_countAlgorithms;
		private System.Collections.Specialized.StringCollection m_countableTypes;
		
		#if IMPR1
		ImageListHelper fileImageListHelper;
		ImageListHelper projectImageListHelper;
		#endif
		#if IMPR2
		List<CountingAlgorithmDescriptor> countingAlgorithms;
		#endif
		#endregion

		#region Handlers
		private int lastSortColumn = -1;	// Track the last clicked column

		/// <summary>
		/// Sorts the ListView by the clicked column, automatically
		/// reversing the sort order on subsequent clicks of the
		/// same column.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e">Provides the index of the clicked column.</param>
		private void lvFileList_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			// Define a variable of the abstract (generic) comparer
			ListViewItemComparer comparer = null;

			// Create an instance of the specific comparer in the 'comparer'
			// variable. Since each of the explicit comparer classes is
			// derived from the abstract case class, polymorphism applies.
			switch (e.Column)
			{
					// Line count columns
				case 1:
				case 2:
				case 3:
					comparer = new FileLinesComparer();
					break;
					// The file extension column
				case 4:
					comparer = new FileExtensionComparer();
					break;
					// All other columns sort by file name
				default:
					comparer = new FileNameComparer();
					break;
			}

			// Set the sorting order
			if (lastSortColumn == e.Column)
			{
				if (lvFileList.Sorting == SortOrder.Ascending)
				{
					lvFileList.Sorting = SortOrder.Descending;
				}
				else
				{
					lvFileList.Sorting = SortOrder.Ascending;
				}
			}
			else
			{
				lvFileList.Sorting = SortOrder.Ascending;
			}
			lastSortColumn = e.Column;

			// Send the comparer the list view and column being sorted
			comparer.SortingList = lvFileList;
			comparer.Column = e.Column;

			// Attach the comparer to the list view and sort
			lvFileList.ListViewItemSorter = comparer;
			lvFileList.Sort();
		}

		/// <summary>
		/// Sets the file listing mode to No Grouping,
		/// and recounts.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>FIX THIS: Add a helper to simply repopulate
		/// the list view with existing count data if it exists!!!</remarks>
		private void tsmiNoGrouping_Click(object sender, EventArgs e)
		{
			tsmiGroupByProj.Checked = false;
			tsmiGroupByType.Checked = false;

			SumSolution();
		}

		/// <summary>
		/// Sets the file listing mode to Group by Project,
		/// and recounts.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>FIX THIS: Add a helper to simply repopulate
		/// the list view with existing count data if it exists!!!</remarks>
		private void tsmiGroupByProj_Click(object sender, EventArgs e)
		{
			tsmiGroupByType.Checked = false;
			tsmiNoGrouping.Checked = false;

			SumSolution();
		}

		/// <summary>
		/// Sets the file listing mode to Group by Type,
		/// and recounts.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>FIX THIS: Add a helper to simply repopulate
		/// the list view with existing count data if it exists!!!</remarks>
		private void tsmiGroupByType_Click(object sender, EventArgs e)
		{
			tsmiGroupByProj.Checked = false;
			tsmiNoGrouping.Checked = false;

			SumSolution();
		}

		/// <summary>
		/// Forces a recount of all files in all projects, and
		/// updates the view.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsmiRecalculate_FileList_Click(object sender, EventArgs e)
		{
			ScanSolution();
			SumSolution();
		}
		#endregion

		#region Helpers
		#region Line Counting Methods
		/// <summary>
		/// Count each line in a text file, logging
		/// blank lines.
		/// </summary>
		/// <param name="info">The file information data to use.</param>
		internal static void CountLinesGeneric(LineCountInfo info)
		{
			StreamReader sr = new StreamReader(info.FileName);

			string line;
			while ((line = sr.ReadLine()) != null)
			{
				info.LineCountInfoDetails.Add("TotalLines", 1);
				if (line.Trim() == string.Empty)
				{
					info.LineCountInfoDetails.Add("BlankLines", 1);
				}

				info.SumMode = "Generic";
			}

			sr.Close();
		}

		/// <summary>
		/// Count each line in a c-style source file, scanning
		/// for single and multi-line comments, code, and blank lines.
		/// </summary>
		/// <param name="info">The file information data to use.</param>
		/// <remarks>This algorithm was originally created by Oz Solomon,
		/// for his PLC line counter add-in for Visual Studio 2002/2003.</remarks>
		internal static void CountLinesCStyle(LineCountInfo info)
		{
			try
			{
				StreamReader reader = new StreamReader(info.FileName);

				string line;
				bool multiLineComment = false;
				bool hasCode = false;
				bool hasComments = false;
				while ((line = reader.ReadLine()) != null)
				{
					ParseCLine(line, ref multiLineComment, ref hasCode, ref hasComments);

					if (hasComments)
					{
						info.LineCountInfoDetails.Add("normcmtlines", 1);
					}

					if (hasCode)
					{
						info.LineCountInfoDetails.Add("codelines", 1);
					}

					if (!hasCode && !hasComments)
					{
						info.LineCountInfoDetails.Add("blanklines", 1);
					}

					info.LineCountInfoDetails.Add("totallines", 1);
				}

				reader.Close();

				info.SumMode = "C-Style";
			}
			catch
			{
			}
		}

		/// <summary>
		/// Count each line in a vb-style source file, scanning
		/// for comments, code, and blank lines.
		/// </summary>
		/// <param name="info">The file information data to use.</param>
		/// <remarks>This algorithm was originally created by Oz Solomon,
		/// for his PLC line counter add-in for Visual Studio 2002/2003.</remarks>
		internal static void CountLinesVBStyle(LineCountInfo info)
		{
			try
			{
				StreamReader reader = new StreamReader(info.FileName);

				string line;
				bool multiLineComment = false;
				bool hasCode = false;
				bool hasComments = false;
				while ((line = reader.ReadLine()) != null)
				{
					ParseVBLine(line, ref multiLineComment, ref hasCode, ref hasComments);

					if (hasComments)
					{
						info.LineCountInfoDetails.Add("normcmtlines", 1);
					}

					if (hasCode)
					{
						info.LineCountInfoDetails.Add("codelines", 1);
					}

					if (!hasCode && !hasComments)
					{
						info.LineCountInfoDetails.Add("blanklines", 1);
					}

					info.LineCountInfoDetails.Add("totallines", 1);
				}

				reader.Close();

				info.SumMode = "Visual Basic";
			}
			catch
			{
			}
		}

		/// <summary>
		/// Count each line in an xml source file, scanning
		/// for comments, code, and blank lines.
		/// </summary>
		/// <param name="info">The file information data to use.</param>
		/// <remarks>This algorithm is based on one created by Oz Solomon,
		/// for his PLC line counter add-in for Visual Studio 2002/2003.</remarks>
		internal static void CountLinesXMLStyle(LineCountInfo info)
		{
			try
			{
				StreamReader reader = new StreamReader(info.FileName);

				string line;
				bool multiLineComment = false;
				bool hasCode = false;
				bool hasComments = false;
				while ((line = reader.ReadLine()) != null)
				{
					ParseXMLLine(line, ref multiLineComment, ref hasCode, ref hasComments);

					if (hasComments)
					{
						info.LineCountInfoDetails.Add("normcmtlines", 1);
					}

					if (hasCode)
					{
						info.LineCountInfoDetails.Add("codelines", 1);
					}

					if (!hasCode && !hasComments)
					{
						info.LineCountInfoDetails.Add("blanklines", 1);
					}

					info.LineCountInfoDetails.Add("totallines", 1);
				}

				reader.Close();

				info.SumMode = "XML";
			}
			catch
			{
			}
		}

		/// <summary>
		/// Determines if the two input characters ch and chNext
		/// match the given pair of characters a and b.
		/// </summary>
		/// <param name="a">First char requirement.</param>
		/// <param name="b">Second char requirement.</param>
		/// <param name="ch">First char to test.</param>
		/// <param name="chNext">Second char to test.</param>
		/// <returns></returns>
		private static bool IsPair(char a, char b, char ch, char chNext)
		{
			return (ch == a && chNext == b);
		}

		/// <summary>
		/// Parses a c-style code line for comments, code, and blanks.
		/// </summary>
		/// <param name="line"></param>
		/// <param name="multiLineComment"></param>
		/// <param name="hasCode"></param>
		/// <param name="hasComments"></param>
		/// <remarks>This algorithm was originally created by Oz Solomon,
		/// for his PLC line counter add-in for Visual Studio 2002/2003.</remarks>
		private static void ParseCLine(string line, ref bool multiLineComment, ref bool hasCode, ref bool hasComments)
		{
			bool inString = false;
			bool inTwoPairSequence = false;

			hasComments = multiLineComment;
			hasCode = false;

			for (int i = 0; i < line.Length; i++)
			{
				char ch = line[i];
				char chNext = (i < line.Length - 1 ? line[i + 1] : '\0');

				// Process a single-line comment
				if (IsPair('/', '/', ch, chNext) && !multiLineComment && !inString)
				{
					hasComments = true;
					return;
				}

				// Process start of a multiline comment
				else if (IsPair('/', '*', ch, chNext) && !multiLineComment && !inString)
				{
					multiLineComment = true;
					hasComments = true;
					++i;
				}

				// Process end of a multiline comment
				else if (IsPair('*', '/', ch, chNext) && !inString)
				{
					multiLineComment = false;
					++i;
				}

				// Process escaped character
				else if (ch == '\\' && !multiLineComment)
				{
					++i;
					hasCode = true;
				}

				// Process string
				else if (ch == '"' && !multiLineComment)
				{
					inString = !inString;
					hasCode = true;
				}

				else if (!multiLineComment)
				{
					if (!Char.IsWhiteSpace(ch))
					{
						hasCode = true;
					}
				}
			}
		}

		/// <summary>
		/// Parses a vb-style code line for comments, code and blanks.
		/// </summary>
		/// <param name="line"></param>
		/// <param name="multiLineComment"></param>
		/// <param name="hasCode"></param>
		/// <param name="hasComments"></param>
		/// <remarks>This algorithm was originally created by Oz Solomon,
		/// for his PLC line counter add-in for Visual Studio 2002/2003.</remarks>
		private static void ParseVBLine(string line, ref bool multiLineComment, ref bool hasCode, ref bool hasComments)
		{
			bool inString = false;
			bool inTwoPairSequence = false;

			multiLineComment = false;

			line = line.Trim();

			if (line.Length == 0)
			{
				hasCode = false;
				hasComments = false;
				return;
			}

			if (line[0] == '\'')
			{
				hasCode = false;
				hasComments = true;
				return;
			}

			if (line.IndexOf('\'') != -1)
			{
				hasCode = true;
				hasComments = true;
				return;
			}

			hasCode = true;
			hasComments = true;
		}

		/// <summary>
		/// Parses an xml-style code line for comments, markup, and blanks.
		/// </summary>
		/// <param name="line"></param>
		/// <param name="multiLineComment"></param>
		/// <param name="hasCode"></param>
		/// <param name="hasComments"></param>
		/// <remarks>This algorithm is based on one created by Oz Solomon,
		/// for his PLC line counter add-in for Visual Studio 2002/2003.</remarks>
		private static void ParseXMLLine(string line, ref bool multiLineComment, ref bool hasCode, ref bool hasComments)
		{
			bool inString = false;
			bool inTwoPairSequence = false;

			hasComments = multiLineComment;
			hasCode = false;

			for (int i = 0; i < line.Length; i++)
			{
				char ch1 = line[i];
				char ch2 = (i < line.Length-1 ? line[i + 1] : '\0');
				char ch3 = (i+1 < line.Length-1 ? line[i + 2] : '\0');
				char ch4 = (i+2 < line.Length-1 ? line[i + 3] : '\0');

				// Process start of XML comment
				if (IsPair('<', '!', ch1, ch2) && IsPair('-', '-', ch3, ch4) && !multiLineComment && !inString)
				{
					multiLineComment = true;
					hasComments = true;
					i += 3;
				}

				// Process end of XML comment
				else if (IsPair('-', '-', ch1, ch2) && ch3 == '>' && !inString)
				{
					multiLineComment = false;
					i += 2;
				}

				// Process string
				else if (ch3 == '"' && !multiLineComment)
				{
					inString = !inString;
					hasCode = true;
				}

				else if (!multiLineComment)
				{
					if (!Char.IsWhiteSpace(ch1))
					{
						hasCode = true;
					}
				}
			}
		}
		#endregion

		#region Scanning and Summation Methods
		/// <summary>
		/// Scans the solution and creates a hierarchy of
		/// support objects for each project and file
		/// within each project.
		/// </summary>
		private void ScanSolution()
		{
			if (m_summaryList == null)
				m_summaryList = new List<LineCountSummary>();

			m_summaryList.Clear();
			
			Solution solution = ProjectService.OpenSolution;
			if (solution != null) // OpenSolution is null when no solution is opened
			{
				FileInfo fiSolution = new FileInfo(solution.FileName);
				LineCountSummary summary = new LineCountSummary("All Projects", m_projIconMappings["{00000000-0000-0000-0000-000000000000}"]);
				m_summaryList.Add(summary);

				// Configure progress bars
				tsprgTotal.Minimum = 0;
				tsprgTotal.Step = 1;
				tsprgTask.Minimum = 0;
				tsprgTask.Step = 1;

				List<IProject> projects = new List<IProject>(solution.Projects);
				tsprgTotal.Maximum = projects.Count;
				tsprgTask.Value = 0;
				foreach (IProject fiProject in projects) {
					tsprgTotal.PerformStep();
					string projName, lang;
					if (fiProject.FileName.IndexOf("://") != -1)
					{
						projName = fiProject.FileName; // this is a web project
						lang = "{00000001-0000-0000-0000-000000000000}";
					} else {
						projName = fiProject.Name;
						lang = fiProject.TypeGuid;
					}

					int iconIndex;
					#if IMPR1
					iconIndex = projectImageListHelper.GetIndex(IconService.GetImageForProjectType(fiProject.Language ?? "defaultLanguageName"));
					#else
					m_projIconMappings.TryGetValue(lang, out iconIndex); // default icon 0
					#endif
					summary = new LineCountSummary(projName, iconIndex);
					m_summaryList.Add(summary);

					tsprgTask.Maximum = 0;
					tsprgTotal.Value = 0;
					ScanProjectItems(fiProject.Items, summary);
				}

				tsprgTask.Value = tsprgTask.Maximum;
				tsprgTotal.Value = tsprgTotal.Maximum;
			}
			else
			{
				MessageBox.Show("There is no solution open in SharpDevelop.", "Line Counter");
			}
		}

		/// <summary>
		/// Scans the project items (files, usually) of
		/// a project's ProjectItems collection.
		/// </summary>
		/// <param name="projectItems">The ProjectItems collection to scan.</param>
		/// <param name="summary">The root summary data object that these
		/// files belong to.</param>
		private void ScanProjectItems(IList<ProjectItem> projectItems, LineCountSummary summary)
		{
			tsprgTask.Maximum += projectItems.Count;
			foreach (ProjectItem projectItem in projectItems)
			{
				tsprgTask.PerformStep();
				if (!(projectItem is FileProjectItem)) {
					// Skip references and other special MSBuild things
					continue;
				}
				string projectFile = projectItem.FileName;
				if (!Directory.Exists(projectFile))
				{
					int iconIndex = 0;
					#if IMPR1
					iconIndex = fileImageListHelper.GetIndex(IconService.GetImageForFile(projectFile));
					#else
					m_fileIconMappings.TryGetValue(Path.GetExtension(projectFile), out iconIndex);
					#endif
					summary.FileLineCountInfo.Add(new LineCountInfo(projectFile, iconIndex, summary));
				}
			}
		}

		/// <summary>
		/// Performs a complete counting and summation of all lines
		/// in all projects and files.
		/// </summary>
		private void SumSolution()
		{
			try
			{
				// Clean the list
				lvSummary.Items.Clear();
				lvFileList.Items.Clear();
				lvFileList.Groups.Clear();

				// Configure progress bars
				tsprgTotal.Minimum = 0;
				tsprgTotal.Step = 1;
				tsprgTask.Minimum = 0;
				tsprgTask.Step = 1;

				// Skip if there are no projects
				if (m_summaryList == null || (m_summaryList != null && m_summaryList.Count == 1))
				{
					MessageBox.Show("There are no projects loaded to summarize.", "Line Counter");
					return;
				}

				// Get all projects summary
				LineCountSummary allProjects = m_summaryList[0];
				allProjects.LineCountSummaryDetails.Reset();
				AddSummaryListItem(allProjects, lvSummary.Groups["lvgAllProj"]);

				tsprgTotal.Maximum = m_summaryList.Count;
				tsprgTotal.Value = 0;
				for (int s = 1; s < m_summaryList.Count; s++)
				{
					tsprgTotal.PerformStep();

					LineCountSummary summary = m_summaryList[s];
					summary.LineCountSummaryDetails.Reset();
					AddSummaryListItem(summary, lvSummary.Groups["lvgEachProj"]);

					tsprgTask.Maximum = summary.FileLineCountInfo.Count;
					tsprgTask.Value = 0;
					for (int i = 0; i < summary.FileLineCountInfo.Count; i++)
					{
						tsprgTask.PerformStep();

						LineCountInfo info = summary.FileLineCountInfo[i];
						if (m_countableTypes.Contains(info.FileType))
						{
							info.LineCountInfoDetails.Reset();
							#if IMPR2
							foreach (CountingAlgorithmDescriptor desc in countingAlgorithms) {
								if (desc.CanCountLines(info)) {
									desc.GetAlgorithm().CountLines(info);
									break;
								}
							}
							#else
							try
							{
								CountLines counter = m_countAlgorithms[info.FileType];
								counter(info);
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
								Console.WriteLine(ex.StackTrace);
							}
							#endif
							info.LineCountInfoDetails.Summarize();

							allProjects.LineCountSummaryDetails.Add(info.LineCountInfoDetails);
							summary.LineCountSummaryDetails.Add(info.LineCountInfoDetails);

							tstxtLinesCounted.Text = allProjects.LineCountSummaryDetails.TotalLines.ToString();

							AddFileListItem(info);
						}
					}

					summary.LineCountSummaryDetails.Summarize();
					LineCountDetails details = summary.LineCountSummaryDetails;
					summary.LinkedListViewItem.SubItems[1].Text = details.TotalLines.ToString();
					summary.LinkedListViewItem.SubItems[2].Text = details.CodeLines.ToString();
					summary.LinkedListViewItem.SubItems[3].Text = details.CommentLines.ToString();
					summary.LinkedListViewItem.SubItems[4].Text = details.BlankLines.ToString();
					summary.LinkedListViewItem.SubItems[5].Text = details.NetLines.ToString();
					details = null;
				}

				allProjects.LineCountSummaryDetails.Summarize();
				LineCountDetails totals = allProjects.LineCountSummaryDetails;
				allProjects.LinkedListViewItem.SubItems[1].Text = totals.TotalLines.ToString();
				allProjects.LinkedListViewItem.SubItems[2].Text = totals.CodeLines.ToString();
				allProjects.LinkedListViewItem.SubItems[3].Text = totals.CommentLines.ToString();
				allProjects.LinkedListViewItem.SubItems[4].Text = totals.BlankLines.ToString();
				allProjects.LinkedListViewItem.SubItems[5].Text = totals.NetLines.ToString();

			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				Debug.WriteLine(ex.StackTrace);
			}

			tsprgTotal.Value = tsprgTotal.Maximum;
		}

		/// <summary>
		/// Obsolete. Useless. Dissapeared.
		/// </summary>
		private void PrepareFileListGroups()
		{
			lvFileList.Groups.Clear();
			if (tsmiGroupByType.Checked)
			{
				
			}
			else if (tsmiGroupByProj.Checked)
			{
			}
		}

		/// <summary>
		/// Adds a summary item to the projects list view.
		/// </summary>
		/// <param name="summary">The summary data object to reference.</param>
		/// <param name="group">The summary list view group this item
		/// should be listed under.</param>
		private void AddSummaryListItem(LineCountSummary summary, ListViewGroup group)
		{
			ListViewItem lvi = new ListViewItem();
			lvi.Text = summary.ProjectName;
			lvi.SubItems.Add("0");
			lvi.SubItems.Add("0");
			lvi.SubItems.Add("0");
			lvi.SubItems.Add("0");
			lvi.SubItems.Add("0");

			lvi.Tag = summary;
			lvi.ImageIndex = summary.IconIndex;
			//lvi.StateImageIndex = summary.IconIndex;
			lvi.Group = group;

			summary.LinkedListViewItem = lvi;

			lvSummary.Items.Add(lvi);
		}

		/// <summary>
		/// Adds a file information item to the file list view.
		/// </summary>
		/// <param name="info">The file information data object.</param>
		private void AddFileListItem(LineCountInfo info)
		{
			FileInfo fileInfo = new FileInfo(info.FileName);

			ListViewItem lvi = new ListViewItem();
			lvi.Text = fileInfo.Name;
			lvi.SubItems.Add(info.LineCountInfoDetails.TotalLines.ToString());
			lvi.SubItems.Add(info.LineCountInfoDetails.CodeLines.ToString());
			lvi.SubItems.Add(info.LineCountInfoDetails.CommentLines.ToString());
			lvi.SubItems.Add(info.FileType);
			lvi.SubItems.Add(info.SumMode);

			lvi.Tag = info;

			lvi.ImageIndex = info.IconIndex;
			//lvi.StateImageIndex = iconIndex;


			if (tsmiGroupByType.Checked)
			{
				ListViewGroup group = lvFileList.Groups["groupType" + info.FileType.Substring(1)];
				if (group == null)
				{
					group = new ListViewGroup("groupType" + info.FileType.Substring(1), info.FileType.Substring(1).ToUpper() + " Files");
					lvFileList.Groups.Add(group);
				}

				lvi.Group = group;
			}
			else if (tsmiGroupByProj.Checked)
			{
				ListViewGroup group = lvFileList.Groups["groupProj" + info.ProjectSummary.ProjectName];
				if (group == null)
				{
					group = new ListViewGroup("groupProj" + info.ProjectSummary.ProjectName, info.ProjectSummary.ProjectName + " Files");
					lvFileList.Groups.Add(group);
				}

				lvi.Group = group;
			}

			lvFileList.Items.Add(lvi);
		}
		#endregion
		#endregion
	}

	#region Support Structures
	// Delegate for pluggable line counting methods
	delegate void CountLines(LineCountInfo info);

	/// <summary>
	/// Encapsulates line count sum details.
	/// </summary>
	public class LineCountDetails
	{
		public LineCountDetails()
		{
			Reset();
		}

		private int m_totalLines;
		private int m_codeLines;
		private int m_commentLines;
		private int m_doccmtLines;
		private int m_normcmtLines;
		private int m_blankLines;
		private int m_netLines;

		public int TotalLines
		{
			get { return m_totalLines; }
		}

		public int CodeLines
		{
			get { return m_codeLines; }
		}

		public int CommentLines
		{
			get { return m_commentLines; }
		}

		public int DocCommentLines
		{
			get { return m_doccmtLines; }
		}

		public int NormalCommentLines
		{
			get { return m_normcmtLines; }
		}

		public int BlankLines
		{
			get { return m_blankLines; }
		}

		public int NetLines
		{
			get { return m_netLines; }
		}

		public void Reset()
		{
			m_totalLines = 0;
			m_codeLines = 0;
			m_commentLines = 0;
			m_doccmtLines = 0;
			m_normcmtLines = 0;
			m_blankLines = 0;
			m_netLines = 0;
		}

		public void Summarize()
		{
			m_commentLines = m_doccmtLines + m_normcmtLines;
			m_netLines = m_totalLines - m_blankLines;
		}

		public void Add(string toWhat, int amount)
		{
			if (toWhat == null)
				toWhat = "totallines";

			toWhat = toWhat.ToLower();
			switch (toWhat)
			{
					case "totallines": m_totalLines += amount; break;
					case "codelines": m_codeLines += amount; break;
					case "doccmtlines": m_doccmtLines += amount; break;
					case "normcmtlines": m_normcmtLines += amount; break;
					case "blanklines": m_blankLines += amount; break;
			}
		}

		public void Add(LineCountDetails details)
		{
			m_totalLines += details.m_totalLines;
			m_codeLines += details.m_codeLines;
			m_doccmtLines += details.m_doccmtLines;
			m_normcmtLines += details.m_normcmtLines;
			m_blankLines += details.m_blankLines;
		}
	}

	/// <summary>
	/// Wraps a project and the line count total detail
	/// for that project. Enumerates all of the files
	/// within that project.
	/// </summary>
	public class LineCountSummary
	{
		public LineCountSummary(string projName, int iconIndex)
		{
			m_fileLineCountInfo = new List<LineCountInfo>();
			m_lineCountSummaryDetails = new LineCountDetails();
			m_projectName = projName;
			m_iconIndex = iconIndex;
		}

		private List<LineCountInfo> m_fileLineCountInfo;
		private LineCountDetails m_lineCountSummaryDetails;
		private string m_projectName;
		private int m_iconIndex;

		internal ListViewItem LinkedListViewItem;

		public string ProjectName
		{
			get { return m_projectName; }
		}

		public int IconIndex
		{
			get { return m_iconIndex; }
		}

		public LineCountDetails LineCountSummaryDetails
		{
			get { return m_lineCountSummaryDetails; }
		}

		public List<LineCountInfo> FileLineCountInfo
		{
			get { return m_fileLineCountInfo; }
		}
	}

	/// <summary>
	/// Wraps a project source code file and the line
	/// count info for that file. Also provides details
	/// about the file type and what icon should be shown
	/// for the file in the UI.
	/// </summary>
	public class LineCountInfo
	{
		public LineCountInfo(string fileName, int iconIndex)
		{
			m_fileName = fileName;
			m_fileType = Path.GetExtension(fileName);
			m_iconIndex = iconIndex;
			m_sumMode = "Generic";

			m_lineCountInfoDetails = new LineCountDetails();
		}

		public LineCountInfo(string fileName, int iconIndex, LineCountSummary projectSummary) : this(fileName, iconIndex)
		{
			m_projectSummary = projectSummary;
		}

		private LineCountDetails m_lineCountInfoDetails;
		private string m_fileName;
		private string m_fileType;
		private int m_iconIndex;
		private LineCountSummary m_projectSummary;
		private string m_sumMode;

		public string FileName
		{
			get { return m_fileName; }
		}

		public string FileType
		{
			get { return m_fileType; }
		}

		public int IconIndex
		{
			get { return m_iconIndex; }
		}

		public string SumMode
		{
			get { return m_sumMode; }
			set { m_sumMode = value; }
		}

		public LineCountDetails LineCountInfoDetails
		{
			get { return m_lineCountInfoDetails; }
		}

		public LineCountSummary ProjectSummary
		{
			get { return m_projectSummary; }
		}
	}
	#endregion
}
