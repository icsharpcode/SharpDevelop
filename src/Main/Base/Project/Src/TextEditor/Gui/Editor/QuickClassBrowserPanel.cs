// created on 07.03.2004 at 19:12
using System;
using System.Threading;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class QuickClassBrowserPanel : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.ComboBox classComboBox;
		private System.Windows.Forms.ComboBox membersComboBox;
		
		ICompilationUnit            currentCompilationUnit;
		SharpDevelopTextAreaControl textAreaControl;
		bool                        autoselect = true;
		
		class ComboBoxItem : System.IComparable
		{
			object item;
			string text;
			int    iconIndex;
			
			public int IconIndex {
				get {
					return iconIndex;
				}
			}
			
			public object Item {
				get {
					return item;
				}
			}
			
			public IRegion ItemRegion {
				get {
					IClass classItem = item as IClass;
					if (item is IClass)
						return ((IClass)item).Region;
					else if (item is IMember)
						return ((IMember)item).Region;
					else
						return null;
				}
			}
			
			public int Line {
				get {
					IRegion r = this.ItemRegion;
					if (r == null)
						return 0;
					else
						return r.BeginLine - 1;
				}
			}
			
			public int Column {
				get {
					IRegion r = this.ItemRegion;
					if (r == null)
						return 0;
					else
						return r.BeginColumn - 1;
				}
			}
			
			public int EndLine {
				get {
					IRegion r = this.ItemRegion;
					if (r == null)
						return 0;
					else
						return r.EndLine - 1;
				}
			}
			
			public ComboBoxItem(object item, string text, int iconIndex)
			{
				this.item = item;
				this.text = text;
				this.iconIndex = iconIndex;
			}
			
			public bool IsInside(int lineNumber)
			{
				IClass classItem = item as IClass;
				if (classItem != null) {
					if (classItem.Region == null)
						return false;
					return classItem.Region.BeginLine - 1 <= lineNumber &&
						classItem.Region.EndLine - 1 >= lineNumber;
				}
				
				IMember member = item as IMember;
				if (member == null || member.Region == null) {
					return false;
				}
				bool isInside = member.Region.BeginLine - 1 <= lineNumber;
				
				if (member is IMethod) {
					if (((IMethod)member).BodyRegion.EndLine >= 0) {
						isInside &= lineNumber <= ((IMethod)member).BodyRegion.EndLine - 1;
					} else {
						return member.Region.BeginLine - 1 == lineNumber;
					}
				} else if (member is IProperty) {
					isInside &= lineNumber <= ((IProperty)member).BodyRegion.EndLine - 1;
				} else if (member is IIndexer) {
					isInside &= lineNumber <= ((IIndexer)member).BodyRegion.EndLine - 1;
				} else {
					isInside &= lineNumber <= member.Region.EndLine - 1;
				}
				return isInside;
			}
			
			public int CompareItemTo(object obj)
			{
				ComboBoxItem boxItem = (ComboBoxItem)obj;
				
				if (boxItem.Item is IComparable) {
					return ((IComparable)boxItem.Item).CompareTo(item);
				}
				if (boxItem.text != text || boxItem.Line != Line || boxItem.EndLine != EndLine || boxItem.iconIndex != iconIndex) {
					return 1;
				}
				return 0;
			}
			
			public override string ToString()
			{
				IAmbience ambience = AmbienceService.CurrentAmbience;
				ambience.ConversionFlags = ConversionFlags.ShowParameterNames;
				if (item is IMethod) {
					return ambience.Convert((IMethod)item);
				}
				if (item is IProperty) {
					return ambience.Convert((IProperty)item);
				}
				if (item is IIndexer) {
					return ambience.Convert((IIndexer)item);
				}
				if (item is IField) {
					return ambience.Convert((IField)item);
				}
				if (item is IProperty) {
					return ambience.Convert((IProperty)item);
				}
				if (item is IEvent) {
					return ambience.Convert((IEvent)item);
				}
				return text;
			}
			
			#region System.IComparable interface implementation
			public int CompareTo(object obj)
			{
				return ToString().CompareTo(obj.ToString());
			}
			#endregion
			
		}
		
		public QuickClassBrowserPanel(SharpDevelopTextAreaControl textAreaControl)
		{
			InitializeComponent();
			this.membersComboBox.MaxDropDownItems = 20;
			
			base.Dock = DockStyle.Top;
			this.textAreaControl = textAreaControl;
			this.textAreaControl.ActiveTextAreaControl.Caret.PositionChanged += new EventHandler(CaretPositionChanged);
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				this.textAreaControl.ActiveTextAreaControl.Caret.PositionChanged -= new EventHandler(CaretPositionChanged);
			}
			base.Dispose(disposing);
		}
		
		void CaretPositionChanged(object sender, EventArgs e)
		{
			// ignore simple movements
			if (e != EventArgs.Empty) {
				return;
			}
			try {
				
				ParseInformation parseInfo = ParserService.GetParseInformation(textAreaControl.FileName);
				if (parseInfo != null) {
					if (currentCompilationUnit != (ICompilationUnit)parseInfo.MostRecentCompilationUnit) {
						currentCompilationUnit = (ICompilationUnit)parseInfo.MostRecentCompilationUnit;
						if (currentCompilationUnit != null) {
							
							FillClassComboBox(true);
							FillMembersComboBox();
						}
					}
					UpdateClassComboBox();
					UpdateMembersComboBox();
				}
			} catch (Exception) {}
		}
		
		bool membersComboBoxSelectedMember = false;
		void UpdateMembersComboBox()
		{
			autoselect = false;
			try {
				if (currentCompilationUnit != null) {
					for (int i = 0; i < membersComboBox.Items.Count; ++i) {
						if (((ComboBoxItem)membersComboBox.Items[i]).IsInside(textAreaControl.ActiveTextAreaControl.Caret.Line)) {
							if (membersComboBox.SelectedIndex != i) {
								membersComboBox.SelectedIndex = i;
								
							}
							if (!membersComboBoxSelectedMember) {
								membersComboBox.Refresh();
							}
							membersComboBoxSelectedMember = true;
							return;
						}
					}
				}
				if (membersComboBoxSelectedMember) {
					membersComboBox.Refresh();
					membersComboBoxSelectedMember = false;
				}
			} finally {
				autoselect = true;
			}
		}
		
		bool classComboBoxSelectedMember = false;
		void UpdateClassComboBox()
		{
			// Still needed ?
			if (currentCompilationUnit == null) {
				currentCompilationUnit = (ICompilationUnit)ParserService.GetParseInformation(Path.GetFullPath(textAreaControl.FileName)).MostRecentCompilationUnit;
			}
			
			autoselect = false;
			try {
				if (currentCompilationUnit != null) {
					//// Alex: when changing between files in different compilation units whole process must be restarted
					//// happens usually when files are opened from different project(s)
					if (classComboBox.Items == null || classComboBox.Items.Count == 0) {
						FillClassComboBox(false);
					}
					for (int i = 0; i < classComboBox.Items.Count; ++i) {
						if (((ComboBoxItem)classComboBox.Items[i]).IsInside(textAreaControl.ActiveTextAreaControl.Caret.Line)) {
							bool innerClassContainsCaret = false;
							for (int j = i + 1; j < classComboBox.Items.Count; ++j) {
								if (((ComboBoxItem)classComboBox.Items[j]).IsInside(textAreaControl.ActiveTextAreaControl.Caret.Line)) {
									innerClassContainsCaret = true;
									break;
								}
							}
							if (!innerClassContainsCaret) {
								if (classComboBox.SelectedIndex != i) {
									classComboBox.SelectedIndex = i;
									FillMembersComboBox();
								}
								if (!classComboBoxSelectedMember) {
									classComboBox.Refresh();
								}
								classComboBoxSelectedMember = true;
								return;
							}
						}
					}
				}
				if (classComboBoxSelectedMember) {
					classComboBox.Refresh();
					classComboBoxSelectedMember = false;
				}
			} finally {
				autoselect = true;
			}
//				classComboBox.SelectedIndex = -1;
		}
		
		bool NeedtoUpdate(ArrayList items, ComboBox comboBox)
		{
			if (items.Count != comboBox.Items.Count) {
				return true;
			}
			for (int i = 0; i < items.Count; ++i) {
				ComboBoxItem oldItem = (ComboBoxItem)comboBox.Items[i];
				ComboBoxItem newItem = (ComboBoxItem)items[i];
				if (oldItem.GetType() != newItem.GetType()) {
					return true;
				}
				if (newItem.CompareItemTo(oldItem) != 0) {
					return true;
				}
			}
			return false;
		}
		
		void FillMembersComboBox()
		{
			IClass c = GetCurrentSelectedClass();
			if (c != null) {
				ArrayList items = new ArrayList();
				
				int lastIndex = 0;
				IComparer comparer = new Comparer(System.Globalization.CultureInfo.InvariantCulture);
				foreach (IMethod m in c.Methods) {
					items.Add(new ComboBoxItem(m, m.Name, ClassBrowserIconService.GetIcon(m)));
				}
				items.Sort(lastIndex, c.Methods.Count, comparer);
				lastIndex = items.Count;
				
				foreach (IProperty p in c.Properties) {
					items.Add(new ComboBoxItem(p, p.Name, ClassBrowserIconService.GetIcon(p)));
				}
				items.Sort(lastIndex, c.Properties.Count, comparer);
				lastIndex = items.Count;
				
				foreach (IIndexer indexer in c.Indexer) {
					items.Add(new ComboBoxItem(indexer, indexer.Name, ClassBrowserIconService.GetIcon(indexer)));
				}
				items.Sort(lastIndex, c.Indexer.Count, comparer);
				lastIndex = items.Count;
				
				foreach (IField f in c.Fields) {
					items.Add(new ComboBoxItem(f, f.Name, ClassBrowserIconService.GetIcon(f)));
				}
				items.Sort(lastIndex, c.Fields.Count, comparer);
				lastIndex = items.Count;
				
				foreach (IEvent evt in c.Events) {
					items.Add(new ComboBoxItem(evt, evt.Name, ClassBrowserIconService.GetIcon(evt)));
				}
				items.Sort(lastIndex, c.Events.Count, comparer);
				lastIndex = items.Count;
				
				if (NeedtoUpdate(items, membersComboBox)) {
					membersComboBox.BeginUpdate();
					membersComboBox.Items.Clear();
					membersComboBox.Items.AddRange(items.ToArray());
					membersComboBox.EndUpdate();
					UpdateMembersComboBox();
				}
			} else {
				if (membersComboBox.Items.Count > 0) {
					membersComboBox.Items.Clear();
				}
			}
		}
		
		void AddClasses(ArrayList items, ICollection classes)
		{
			
			foreach (IClass c in classes) {
				items.Add(new ComboBoxItem(c, c.FullyQualifiedName, ClassBrowserIconService.GetIcon(c)));
				AddClasses(items, c.InnerClasses);
			}
		}
		
		void FillClassComboBox(bool isUpdateRequired)
		{
			ArrayList items = new ArrayList();
			AddClasses(items, currentCompilationUnit.Classes);
			if (NeedtoUpdate(items, classComboBox)) {
				if (isUpdateRequired) {
					classComboBox.BeginUpdate();
				}
				classComboBox.Items.Clear();
				classComboBox.Items.AddRange(items.ToArray());
				if (isUpdateRequired) {
					classComboBox.EndUpdate();
				}
				UpdateClassComboBox();
			}
		}
		
		
		// THIS METHOD IS MAINTAINED BY THE FORM DESIGNER
		// DO NOT EDIT IT MANUALLY! YOUR CHANGES ARE LIKELY TO BE LOST
		void InitializeComponent() {
			this.membersComboBox = new System.Windows.Forms.ComboBox();
			this.classComboBox = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// membersComboBox
			// 
			this.membersComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			                                                                    | System.Windows.Forms.AnchorStyles.Right)));
			this.membersComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.membersComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.membersComboBox.Location = new System.Drawing.Point(200, 4);
			this.membersComboBox.Name = "membersComboBox";
			this.membersComboBox.Size = new System.Drawing.Size(161, 21);
			this.membersComboBox.TabIndex = 1;
			this.membersComboBox.SelectedIndexChanged += new System.EventHandler(this.ComboBoxSelectedIndexChanged);
			this.membersComboBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.MeasureComboBoxItem);
			this.membersComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ComboBoxDrawItem);
			
			//
			// classComboBox
			// 
			this.classComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.classComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.classComboBox.Location = new System.Drawing.Point(4, 4);
			this.classComboBox.Name = "classComboBox";
			this.classComboBox.Size = new System.Drawing.Size(189, 21);
			this.classComboBox.TabIndex = 0;
			this.classComboBox.SelectedIndexChanged += new System.EventHandler(this.ComboBoxSelectedIndexChanged);
			this.classComboBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.MeasureComboBoxItem);
			this.classComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ComboBoxDrawItem);
			this.classComboBox.Sorted = true;
			
			//
			// QuickClassBrowserPanel
			// 
			this.Controls.Add(this.membersComboBox);
			this.Controls.Add(this.classComboBox);
			this.Name = "QuickClassBrowserPanel";
			this.Size = new System.Drawing.Size(368, 28);
			this.Resize += new System.EventHandler(this.QuickClassBrowserPanelResize);
			this.ResumeLayout(false);
		}
		
		public IClass GetCurrentSelectedClass()
		{
			if (classComboBox.SelectedIndex >= 0) {
				return (IClass)((ComboBoxItem)classComboBox.Items[classComboBox.SelectedIndex]).Item;
			}
			return null;
		}
		
		void ComboBoxSelectedIndexChanged(object sender, System.EventArgs e)
		{
			ComboBox comboBox = (ComboBox)sender;
			if (comboBox.SelectedIndex < 0) {
				membersComboBox.Items.Clear();
			} else if (autoselect) {
				textAreaControl.ActiveTextAreaControl.Caret.Position = new Point(((ComboBoxItem)comboBox.Items[comboBox.SelectedIndex]).Column,
				                                                                 ((ComboBoxItem)comboBox.Items[comboBox.SelectedIndex]).Line);
				textAreaControl.ActiveTextAreaControl.TextArea.Focus();
			}
		}
		
		// font - has to be static - don't create on each draw
		static Font font = font = new Font("Arial", 8.25f);
		static StringFormat drawStringFormat = new StringFormat(StringFormatFlags.NoWrap);
//		static int drawingCount = 0;
		
		void ComboBoxDrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
		{
//			Console.WriteLine("Draw " + (drawingCount++));
			ComboBox comboBox = (ComboBox)sender;
			e.DrawBackground();
			
			if (e.Index >= 0) {
				ComboBoxItem item = (ComboBoxItem)comboBox.Items[e.Index];
				
				e.Graphics.DrawImageUnscaled(ClassBrowserIconService.ImageList.Images[item.IconIndex],
				                             new Point(e.Bounds.X, e.Bounds.Y + (e.Bounds.Height - ClassBrowserIconService.ImageList.ImageSize.Height) / 2));
				Rectangle drawingRect = new Rectangle(e.Bounds.X + ClassBrowserIconService.ImageList.ImageSize.Width,
				                                      e.Bounds.Y,
				                                      e.Bounds.Width - ClassBrowserIconService.ImageList.ImageSize.Width,
				                                      e.Bounds.Height);
				
				Brush drawItemBrush = SystemBrushes.WindowText;
				if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) {
					drawItemBrush = SystemBrushes.HighlightText;
				}
				if (e.State == DrawItemState.ComboBoxEdit && !item.IsInside(textAreaControl.ActiveTextAreaControl.Caret.Line)) {
					drawItemBrush = SystemBrushes.ControlDark;
				}
				e.Graphics.DrawString(item.ToString(),
				                      font,
				                      drawItemBrush,
				                      drawingRect,
				                      drawStringFormat);
			}
			e.DrawFocusRectangle();
		}
		
		void QuickClassBrowserPanelResize(object sender, System.EventArgs e)
		{
			Size comboBoxSize = new Size(Width / 2 - 4 * 3, 21);
			classComboBox.Size = comboBoxSize;
			membersComboBox.Location = new Point(classComboBox.Bounds.Right + 8, classComboBox.Bounds.Top);
			membersComboBox.Size = comboBoxSize;
		}
		
		void MeasureComboBoxItem(object sender, System.Windows.Forms.MeasureItemEventArgs e)
		{
			ComboBox comboBox = (ComboBox)sender;
			if (e.Index >= 0) {
				ComboBoxItem item = (ComboBoxItem)comboBox.Items[e.Index];
				SizeF size = e.Graphics.MeasureString(item.ToString(), font);
				e.ItemWidth  = (int)size.Width;
				
				e.ItemHeight = (int)Math.Max(size.Height, ClassBrowserIconService.ImageList.ImageSize.Height);
			}
		}
	}
}
