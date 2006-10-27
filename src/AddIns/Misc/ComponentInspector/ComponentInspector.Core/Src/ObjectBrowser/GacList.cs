// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using NoGoop.Controls;

namespace NoGoop.ObjBrowser
{

	internal class GacList : ListView
	{
        protected const int             PADDING = 10;
        
        internal GacList() : base()
        {
            MultiSelect = false;
            FullRowSelect = true;
            HideSelection = false;
            ContextMenu = new ContextMenu();

            SmallImageList = PresentationMap._icons;
            // The icon for the GAC is recorded under the type of this class
            int iconIndex = PresentationMap.GetInfo(GetType())._iconIndex;

			MenuItem open = new MenuItem();
			open.Text = StringParser.Parse("${res:ComponentInspector.GacList.OpenMenuItem}");
			open.Click += new EventHandler(OpenClick);
            ContextMenu.MenuItems.Add(open);

            String gacDir = Environment.GetEnvironmentVariable("SystemRoot")
                + @"\assembly\GAC";

            DirectoryInfo gacDI = new DirectoryInfo(gacDir);
            DirectoryInfo[] assemblies = gacDI.GetDirectories();

            Graphics g = CreateGraphics();
            int maxNameWidth = 0;
            int maxVersionWidth = 0;
            int maxCultureWidth = 0;
            int maxKeyWidth = 0;

            foreach (DirectoryInfo di in assemblies) {
                int width;

                ListViewItem li = new ListViewItem(di.Name);
                li.ImageIndex = iconIndex;
                Items.Add(li);

                // The one subdirectory is the string that contains
                // version_culture_publickey (separated by underscores)
                DirectoryInfo[] subDirDis = di.GetDirectories();
                String subDirName = subDirDis[0].Name;

                width = (int)g.MeasureString(subDirName, Font).Width;
                if (width > maxNameWidth)   
                    maxNameWidth = width;   

                int culturePos = subDirName.IndexOf('_');
                int keyPos = subDirName.IndexOf('_', culturePos + 1);
                String version = subDirName.Substring(0, culturePos);
                width = (int)g.MeasureString(version, Font).Width;
                if (width > maxVersionWidth)   
                    maxVersionWidth = width;   

                String culture = subDirName.Substring(culturePos + 1, 
                                                      keyPos - culturePos - 1);
                width = (int)g.MeasureString(culture, Font).Width;
                if (width > maxCultureWidth)   
                    maxCultureWidth = width;   

                String key = subDirName.Substring(keyPos + 1);
                width = (int)g.MeasureString(key, Font).Width;
                if (width > maxKeyWidth)   
                    maxKeyWidth = width;   

                li.SubItems.Add(version);
                li.SubItems.Add(culture);
                li.SubItems.Add(key);

                // Find the .dll file in the subdirectory
                FileInfo[] dlls = subDirDis[0].GetFiles("*.dll");
                if (dlls.Length == 1)
                    li.Tag = dlls[0];
            }

            g.Dispose();

            ColumnHeader ch;
            ch = new ColumnHeader();
            ch.Text = StringParser.Parse("${res:ComponentInspector.GacList.GlobalAssemblyNameColumn}");
            ch.TextAlign = HorizontalAlignment.Left;
            ch.Width = maxNameWidth + PADDING;
            Columns.Add(ch);

            ch = new ColumnHeader();
            ch.Text = StringParser.Parse("${res:ComponentInspector.GacList.VersionColumn}");
            ch.TextAlign = HorizontalAlignment.Left;
            ch.Width = maxVersionWidth + PADDING;
            Columns.Add(ch);

            ch = new ColumnHeader();
            ch.Text = StringParser.Parse("${res:ComponentInspector.GacList.CultureColumn}");
            ch.TextAlign = HorizontalAlignment.Left;
            ch.Width = maxCultureWidth + PADDING;
            Columns.Add(ch);

            ch = new ColumnHeader();
            ch.Text = StringParser.Parse("${res:ComponentInspector.GacList.PublicKeyTokenColumn}");
            ch.TextAlign = HorizontalAlignment.Left;
            ch.Width = maxKeyWidth + PADDING;
            Columns.Add(ch);

            View = View.Details;
        }

        protected void OpenClick(object sender, EventArgs e)
        {
            ListViewItem li = SelectedItems[0];
            if (li.Tag != null)
            {
                FileInfo fi = (FileInfo)li.Tag;
                String fileName = fi.DirectoryName + "\\" + fi.Name;
                // The open mechanism will select the assembly tree
                // node whether or not it was actually opened (that is,
                // it might have been already opened)
                // Open can sometimes take a while
                Cursor save = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                AssemblySupport.OpenFile(fileName);
                Cursor.Current = save;
            }
            else
            {
                ErrorDialog.Show("Invalid GAC entry; cannot be opened",
                                "Invalid GAC Entry",
                                 MessageBoxIcon.Error);
            }

        }


	}
}
