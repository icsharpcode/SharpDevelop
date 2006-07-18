namespace LineCounterAddin
{
	partial class LineCounterBrowser
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LineCounterBrowser));
			System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Grand Totals", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Project Totals", System.Windows.Forms.HorizontalAlignment.Left);
			this.tscMain = new System.Windows.Forms.ToolStripContainer();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.tsprgTotal = new System.Windows.Forms.ToolStripProgressBar();
			this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
			this.tsprgTask = new System.Windows.Forms.ToolStripProgressBar();
			this.tstxtLinesCounted = new System.Windows.Forms.ToolStripTextBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.lvFileList = new System.Windows.Forms.ListView();
			this.chFilename = new System.Windows.Forms.ColumnHeader();
			this.chFileTotalLines = new System.Windows.Forms.ColumnHeader();
			this.chFileTotalCode = new System.Windows.Forms.ColumnHeader();
			this.chFileTotalComment = new System.Windows.Forms.ColumnHeader();
			this.chFileExt = new System.Windows.Forms.ColumnHeader();
			this.chMode = new System.Windows.Forms.ColumnHeader();
			this.cmsFileList = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsmiArrange = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiGroupByType = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiGroupByProj = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiNoGrouping = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiRecalculate_FileList = new System.Windows.Forms.ToolStripMenuItem();
			this.imgFileTypes = new System.Windows.Forms.ImageList(this.components);
			this.lvSummary = new System.Windows.Forms.ListView();
			this.chProject = new System.Windows.Forms.ColumnHeader();
			this.chTotalLines = new System.Windows.Forms.ColumnHeader();
			this.chCodeLines = new System.Windows.Forms.ColumnHeader();
			this.chComments = new System.Windows.Forms.ColumnHeader();
			this.chBlankLines = new System.Windows.Forms.ColumnHeader();
			this.chNetLines = new System.Windows.Forms.ColumnHeader();
			this.imgProjectTypes = new System.Windows.Forms.ImageList(this.components);
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.tsmiLineCounter = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiRecalculate = new System.Windows.Forms.ToolStripMenuItem();
			this.tscMain.BottomToolStripPanel.SuspendLayout();
			this.tscMain.ContentPanel.SuspendLayout();
			this.tscMain.TopToolStripPanel.SuspendLayout();
			this.tscMain.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.cmsFileList.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tscMain
			// 
			// 
			// tscMain.BottomToolStripPanel
			// 
			this.tscMain.BottomToolStripPanel.Controls.Add(this.toolStrip1);
			// 
			// tscMain.ContentPanel
			// 
			this.tscMain.ContentPanel.Controls.Add(this.splitContainer1);
			this.tscMain.ContentPanel.Size = new System.Drawing.Size(600, 351);
			this.tscMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tscMain.Location = new System.Drawing.Point(0, 0);
			this.tscMain.Name = "tscMain";
			this.tscMain.Size = new System.Drawing.Size(600, 400);
			this.tscMain.TabIndex = 0;
			this.tscMain.Text = "toolStripContainer1";
			// 
			// tscMain.TopToolStripPanel
			// 
			this.tscMain.TopToolStripPanel.Controls.Add(this.menuStrip1);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.tsprgTotal,
            this.toolStripLabel2,
            this.tsprgTask,
            this.tstxtLinesCounted});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(600, 25);
			this.toolStrip1.Stretch = true;
			this.toolStrip1.TabIndex = 1;
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(80, 22);
			this.toolStripLabel1.Text = "Total Progress:";
			// 
			// tsprgTotal
			// 
			this.tsprgTotal.Name = "tsprgTotal";
			this.tsprgTotal.Size = new System.Drawing.Size(150, 22);
			// 
			// toolStripLabel2
			// 
			this.toolStripLabel2.Name = "toolStripLabel2";
			this.toolStripLabel2.Size = new System.Drawing.Size(78, 22);
			this.toolStripLabel2.Text = "Task Progress:";
			// 
			// tsprgTask
			// 
			this.tsprgTask.Name = "tsprgTask";
			this.tsprgTask.Size = new System.Drawing.Size(150, 22);
			// 
			// tstxtLinesCounted
			// 
			this.tstxtLinesCounted.Name = "tstxtLinesCounted";
			this.tstxtLinesCounted.Size = new System.Drawing.Size(100, 25);
			this.tstxtLinesCounted.ToolTipText = "Lines counted so far";
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.lvFileList);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.lvSummary);
			this.splitContainer1.Size = new System.Drawing.Size(600, 351);
			this.splitContainer1.SplitterDistance = 255;
			this.splitContainer1.TabIndex = 0;
			// 
			// lvFileList
			// 
			this.lvFileList.AllowColumnReorder = true;
			this.lvFileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFilename,
            this.chFileTotalLines,
            this.chFileTotalCode,
            this.chFileTotalComment,
            this.chFileExt,
            this.chMode});
			this.lvFileList.ContextMenuStrip = this.cmsFileList;
			this.lvFileList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvFileList.FullRowSelect = true;
			this.lvFileList.GridLines = true;
			this.lvFileList.HideSelection = false;
			this.lvFileList.Location = new System.Drawing.Point(0, 0);
			this.lvFileList.Name = "lvFileList";
			this.lvFileList.Size = new System.Drawing.Size(600, 255);
			this.lvFileList.SmallImageList = this.imgFileTypes;
			this.lvFileList.TabIndex = 0;
			this.lvFileList.UseCompatibleStateImageBehavior = false;
			this.lvFileList.View = System.Windows.Forms.View.Details;
			this.lvFileList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvFileList_ColumnClick);
			// 
			// chFilename
			// 
			this.chFilename.Text = "File";
			this.chFilename.Width = 192;
			// 
			// chFileTotalLines
			// 
			this.chFileTotalLines.Text = "Total Lines";
			this.chFileTotalLines.Width = 90;
			// 
			// chFileTotalCode
			// 
			this.chFileTotalCode.Text = "Code Lines";
			this.chFileTotalCode.Width = 90;
			// 
			// chFileTotalComment
			// 
			this.chFileTotalComment.Text = "Comments";
			this.chFileTotalComment.Width = 90;
			// 
			// chFileExt
			// 
			this.chFileExt.Text = "Extension";
			this.chFileExt.Width = 64;
			// 
			// chMode
			// 
			this.chMode.Text = "Sum-Mode";
			this.chMode.Width = 70;
			// 
			// cmsFileList
			// 
			this.cmsFileList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiArrange,
            this.tsmiRecalculate_FileList});
			this.cmsFileList.Name = "cmsFileList";
			this.cmsFileList.Size = new System.Drawing.Size(130, 48);
			// 
			// tsmiArrange
			// 
			this.tsmiArrange.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiGroupByType,
            this.tsmiGroupByProj,
            this.toolStripMenuItem1,
            this.tsmiNoGrouping});
			this.tsmiArrange.Name = "tsmiArrange";
			this.tsmiArrange.Size = new System.Drawing.Size(129, 22);
			this.tsmiArrange.Text = "Arrange";
			// 
			// tsmiGroupByType
			// 
			this.tsmiGroupByType.Checked = true;
			this.tsmiGroupByType.CheckOnClick = true;
			this.tsmiGroupByType.CheckState = System.Windows.Forms.CheckState.Checked;
			this.tsmiGroupByType.Name = "tsmiGroupByType";
			this.tsmiGroupByType.ShortcutKeyDisplayString = "CTRL-G,T";
			this.tsmiGroupByType.Size = new System.Drawing.Size(208, 22);
			this.tsmiGroupByType.Text = "Group by type";
			this.tsmiGroupByType.Click += new System.EventHandler(this.tsmiGroupByType_Click);
			// 
			// tsmiGroupByProj
			// 
			this.tsmiGroupByProj.CheckOnClick = true;
			this.tsmiGroupByProj.Name = "tsmiGroupByProj";
			this.tsmiGroupByProj.ShortcutKeyDisplayString = "CTRL-G,P";
			this.tsmiGroupByProj.Size = new System.Drawing.Size(208, 22);
			this.tsmiGroupByProj.Text = "Group by project";
			this.tsmiGroupByProj.Click += new System.EventHandler(this.tsmiGroupByProj_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(205, 6);
			// 
			// tsmiNoGrouping
			// 
			this.tsmiNoGrouping.CheckOnClick = true;
			this.tsmiNoGrouping.Name = "tsmiNoGrouping";
			this.tsmiNoGrouping.ShortcutKeyDisplayString = "CTRL-G,0";
			this.tsmiNoGrouping.Size = new System.Drawing.Size(208, 22);
			this.tsmiNoGrouping.Text = "Do not group";
			this.tsmiNoGrouping.Click += new System.EventHandler(this.tsmiNoGrouping_Click);
			// 
			// tsmiRecalculate_FileList
			// 
			this.tsmiRecalculate_FileList.Name = "tsmiRecalculate_FileList";
			this.tsmiRecalculate_FileList.Size = new System.Drawing.Size(129, 22);
			this.tsmiRecalculate_FileList.Text = "Recalculate";
			this.tsmiRecalculate_FileList.Click += new System.EventHandler(this.tsmiRecalculate_FileList_Click);
			// 
			// imgFileTypes
			// 
			this.imgFileTypes.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgFileTypes.ImageStream")));
			this.imgFileTypes.TransparentColor = System.Drawing.Color.Transparent;
			this.imgFileTypes.Images.SetKeyName(0, "Default.png");
			this.imgFileTypes.Images.SetKeyName(1, "FileCS.png");
			this.imgFileTypes.Images.SetKeyName(2, "FileVB.png");
			this.imgFileTypes.Images.SetKeyName(3, "FileVJ.png");
			this.imgFileTypes.Images.SetKeyName(4, "FileCPP.png");
			this.imgFileTypes.Images.SetKeyName(5, "FileC.png");
			this.imgFileTypes.Images.SetKeyName(6, "FileH.png");
			this.imgFileTypes.Images.SetKeyName(7, "JavaScript.png");
			this.imgFileTypes.Images.SetKeyName(8, "FileCD.png");
			this.imgFileTypes.Images.SetKeyName(9, "FileRESX.png");
			this.imgFileTypes.Images.SetKeyName(10, "FileCSS.png");
			this.imgFileTypes.Images.SetKeyName(11, "FileHTM.png");
			this.imgFileTypes.Images.SetKeyName(12, "FileXML.png");
			this.imgFileTypes.Images.SetKeyName(13, "FileXSL.png");
			this.imgFileTypes.Images.SetKeyName(14, "FileXSD.png");
			this.imgFileTypes.Images.SetKeyName(15, "FileCONFIG.png");
			this.imgFileTypes.Images.SetKeyName(16, "FileASAX.png");
			this.imgFileTypes.Images.SetKeyName(17, "FileASCX.png");
			this.imgFileTypes.Images.SetKeyName(18, "FileASMX.png");
			this.imgFileTypes.Images.SetKeyName(19, "fileASPX.png");
			// 
			// lvSummary
			// 
			this.lvSummary.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chProject,
            this.chTotalLines,
            this.chCodeLines,
            this.chComments,
            this.chBlankLines,
            this.chNetLines});
			this.lvSummary.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvSummary.FullRowSelect = true;
			listViewGroup1.Header = "Grand Totals";
			listViewGroup1.Name = "lvgAllProj";
			listViewGroup2.Header = "Project Totals";
			listViewGroup2.Name = "lvgEachProj";
			this.lvSummary.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
			this.lvSummary.HideSelection = false;
			this.lvSummary.Location = new System.Drawing.Point(0, 0);
			this.lvSummary.Name = "lvSummary";
			this.lvSummary.Size = new System.Drawing.Size(600, 92);
			this.lvSummary.SmallImageList = this.imgProjectTypes;
			this.lvSummary.TabIndex = 0;
			this.lvSummary.UseCompatibleStateImageBehavior = false;
			this.lvSummary.View = System.Windows.Forms.View.Details;
			// 
			// chProject
			// 
			this.chProject.Text = "Project";
			this.chProject.Width = 146;
			// 
			// chTotalLines
			// 
			this.chTotalLines.Text = "Total Lines";
			this.chTotalLines.Width = 90;
			// 
			// chCodeLines
			// 
			this.chCodeLines.Text = "Code Lines";
			this.chCodeLines.Width = 90;
			// 
			// chComments
			// 
			this.chComments.Text = "Comments";
			this.chComments.Width = 90;
			// 
			// chBlankLines
			// 
			this.chBlankLines.Text = "Blank Lines";
			this.chBlankLines.Width = 90;
			// 
			// chNetLines
			// 
			this.chNetLines.Text = "Net Lines";
			this.chNetLines.Width = 90;
			// 
			// imgProjectTypes
			// 
			this.imgProjectTypes.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgProjectTypes.ImageStream")));
			this.imgProjectTypes.TransparentColor = System.Drawing.Color.Transparent;
			this.imgProjectTypes.Images.SetKeyName(0, "Solution.png");
			this.imgProjectTypes.Images.SetKeyName(1, "ProjectCS.png");
			this.imgProjectTypes.Images.SetKeyName(2, "ProjectVB.png");
			this.imgProjectTypes.Images.SetKeyName(3, "ProjectVC.png");
			this.imgProjectTypes.Images.SetKeyName(4, "ProjectVJ.png");
			this.imgProjectTypes.Images.SetKeyName(5, "ProjectWeb.png");
			// 
			// menuStrip1
			// 
			this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiLineCounter});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(600, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// tsmiLineCounter
			// 
			this.tsmiLineCounter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiOptions,
            this.tsmiRecalculate});
			this.tsmiLineCounter.Name = "tsmiLineCounter";
			this.tsmiLineCounter.Size = new System.Drawing.Size(80, 20);
			this.tsmiLineCounter.Text = "Line Counter";
			// 
			// tsmiOptions
			// 
			this.tsmiOptions.Name = "tsmiOptions";
			this.tsmiOptions.Size = new System.Drawing.Size(129, 22);
			this.tsmiOptions.Text = "Options";
			// 
			// tsmiRecalculate
			// 
			this.tsmiRecalculate.Name = "tsmiRecalculate";
			this.tsmiRecalculate.Size = new System.Drawing.Size(129, 22);
			this.tsmiRecalculate.Text = "Recalculate";
			this.tsmiRecalculate.Click += new System.EventHandler(this.tsmiRecalculate_FileList_Click);
			// 
			// LineCounterBrowser
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tscMain);
			this.Name = "LineCounterBrowser";
			this.Size = new System.Drawing.Size(600, 400);
			this.tscMain.BottomToolStripPanel.ResumeLayout(false);
			this.tscMain.BottomToolStripPanel.PerformLayout();
			this.tscMain.ContentPanel.ResumeLayout(false);
			this.tscMain.TopToolStripPanel.ResumeLayout(false);
			this.tscMain.TopToolStripPanel.PerformLayout();
			this.tscMain.ResumeLayout(false);
			this.tscMain.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.cmsFileList.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStripContainer tscMain;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem tsmiLineCounter;
		private System.Windows.Forms.ToolStripMenuItem tsmiOptions;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ToolStripMenuItem tsmiRecalculate;
		private System.Windows.Forms.ListView lvSummary;
		private System.Windows.Forms.ColumnHeader chProject;
		private System.Windows.Forms.ListView lvFileList;
		private System.Windows.Forms.ColumnHeader chTotalLines;
		private System.Windows.Forms.ColumnHeader chCodeLines;
		private System.Windows.Forms.ColumnHeader chComments;
		private System.Windows.Forms.ColumnHeader chBlankLines;
		private System.Windows.Forms.ColumnHeader chNetLines;
		private System.Windows.Forms.ColumnHeader chFilename;
		private System.Windows.Forms.ColumnHeader chFileTotalLines;
		private System.Windows.Forms.ColumnHeader chFileTotalCode;
		private System.Windows.Forms.ColumnHeader chFileTotalComment;
		private System.Windows.Forms.ColumnHeader chFileExt;
		private System.Windows.Forms.ContextMenuStrip cmsFileList;
		private System.Windows.Forms.ToolStripMenuItem tsmiArrange;
		private System.Windows.Forms.ToolStripMenuItem tsmiGroupByType;
		private System.Windows.Forms.ToolStripMenuItem tsmiGroupByProj;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem tsmiNoGrouping;
		private System.Windows.Forms.ToolStripMenuItem tsmiRecalculate_FileList;
		private System.Windows.Forms.ImageList imgProjectTypes;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripProgressBar tsprgTotal;
		private System.Windows.Forms.ToolStripLabel toolStripLabel2;
		private System.Windows.Forms.ToolStripProgressBar tsprgTask;
		private System.Windows.Forms.ToolStripTextBox tstxtLinesCounted;
		private System.Windows.Forms.ImageList imgFileTypes;
		private System.Windows.Forms.ColumnHeader chMode;
	}
}
