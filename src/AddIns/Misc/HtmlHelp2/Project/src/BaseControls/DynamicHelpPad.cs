// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mathias Simmack" email="mathias@simmack.de"/>
//     <version>$Revision$</version>
// </file>

namespace HtmlHelp2
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Drawing;
	using System.Globalization;
	using System.Reflection;
	using System.Security.Permissions;
	using System.Windows.Forms;
	
	using HtmlHelp2.Environment;
	using ICSharpCode.Core;
	using ICSharpCode.SharpDevelop;
	using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
	using ICSharpCode.SharpDevelop.Dom;
	using ICSharpCode.SharpDevelop.Gui;
	using ICSharpCode.SharpDevelop.Project;
	using ICSharpCode.TextEditor;
	using MSHelpServices;

	public class ShowDynamicHelpMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PadDescriptor dynamicHelp = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2DynamicHelpPad));
			if (dynamicHelp != null) dynamicHelp.BringPadToFront();
		}
	}

	public class HtmlHelp2DynamicHelpPad : AbstractPadContent
	{
		HtmlHelp2DynamicHelpBrowserControl dynamicHelpBrowser;
		private List<string> dynamicHelpTerms       = new List<string>();
		private TextLocation lastPoint              = TextLocation.Empty;
		private string debugPreElement              = String.Empty;
		private bool enableDebugInfo                = HtmlHelp2Environment.Config.DynamicHelpDebugInfos;

		public override Control Control
		{
			get { return dynamicHelpBrowser; }
		}

		public override void RedrawContent()
		{
			dynamicHelpBrowser.RedrawContent();
		}

		[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
		public HtmlHelp2DynamicHelpPad()
		{
			dynamicHelpBrowser                      = new HtmlHelp2DynamicHelpBrowserControl();
			dynamicHelpBrowser.LoadDynamicHelpPage();
			
			ParserService.ParserUpdateStepFinished += UpdateTick;
			PropertyPad.SelectedObjectChanged      += new EventHandler(this.FormsDesignerSelectedObjectChanged);
			PropertyPad.SelectedGridItemChanged    += new SelectedGridItemChangedEventHandler(this.FormsDesignerSelectedGridItemChanged);
			ProjectService.SolutionClosed          += new EventHandler(this.SolutionClosed);
			
			HtmlHelp2Environment.NamespaceReloaded += new EventHandler(this.NamespaceReloaded);
		}

		#region Dynamic Help Calls
		private void BuildDynamicHelpList()
		{
			try
			{
				dynamicHelpBrowser.RemoveAllChildren();
				this.debugPreElement = string.Empty;
				bool helpResults = false;
				Cursor.Current = Cursors.WaitCursor;

				foreach (string currentHelpTerm in this.dynamicHelpTerms)
				{
					if (!currentHelpTerm.StartsWith("!"))
					{
						helpResults = (this.CallDynamicHelp(currentHelpTerm, false) || helpResults);
					}
				}
				foreach (string currentHelpTerm in this.dynamicHelpTerms)
				{
					if (currentHelpTerm.StartsWith("!"))
					{
						helpResults = (this.CallDynamicHelp(currentHelpTerm.Substring(1)) || helpResults);
					}
				}

				Cursor.Current = Cursors.Default;
				
				// debug info
				if (this.enableDebugInfo)
				{
					this.debugPreElement +=
						string.Format(CultureInfo.InvariantCulture, "<br>Current project language: {0}", SharpDevLanguage.GetPatchedLanguage());
					dynamicHelpBrowser.CreateDebugPre(this.debugPreElement);
				}
			}
			catch (System.Runtime.InteropServices.COMException ex)
			{
				LoggingService.Error("Help 2.0: Dynamic Help Call Exception; " + ex.ToString());
			}
		}

		private bool CallDynamicHelp(string searchTerm)
		{
			return this.CallDynamicHelp(searchTerm, true);
		}
		
		private bool CallDynamicHelp(string searchTerm, bool keywordSearch)
		{
			if (!HtmlHelp2Environment.SessionIsInitialized || HtmlHelp2Environment.DynamicHelpIsBusy)
			{
				return false;
			}

			IHxTopicList topics = HtmlHelp2Environment.GetMatchingTopicsForDynamicHelp(searchTerm);
			bool result = (topics != null && topics.Count > 0);

			if (result)
			{
				// debug info
				this.debugPreElement +=
					string.Format(CultureInfo.InvariantCulture,
					              "{0} ({1}): {2} {3}<br>", searchTerm, (keywordSearch)?"Kwd":"DH",
					              topics.Count, (topics.Count==1)?"topic":"topics");
				
				List<IHxTopic> newTopics = SortTopics(topics);
				foreach (IHxTopic topic in newTopics)
				{
					if ((keywordSearch)?SharpDevLanguage.CheckUniqueTopicLanguage(topic):SharpDevLanguage.CheckTopicLanguage(topic))
					{
						this.BuildNewChild(topic.Location,
						                   topic.get_Title(HxTopicGetTitleType.HxTopicGetRLTitle,
						                                   HxTopicGetTitleDefVal.HxTopicGetTitleFileName),
						                   topic.URL);
					}
				}
			}
			return result;
		}

		private void BuildNewChild(string sectionName, string topicName, string topicUrl)
		{
			try {
				dynamicHelpBrowser.BuildNewChild(sectionName, topicName, topicUrl);
			} catch (NullReferenceException ex) {
				// HACK: the code doesn't properly check for nulls, so we just ignore errors.
				// There were bug reports with BuildNewChild crashing simply on a layout change.
				// e.g. http://community.sharpdevelop.net/forums/t/9180.aspx
				LoggingService.Warn(ex);
			}
		}
		#endregion

		#region Taken from DefinitionView.cs
		private void UpdateTick(object sender, ParserUpdateStepEventArgs e)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(UpdateTick, e);
		}
		
		void UpdateTick(ParserUpdateStepEventArgs e)
		{
			this.dynamicHelpTerms.Clear();
			ResolveResult res = ResolveAtCaret(e);
			if (res == null) return;

			if (res != null && res.ResolvedType != null)
			{
				this.AddToStringCollection(res.ResolvedType.FullyQualifiedName);
			}

			MemberResolveResult member      = res as MemberResolveResult;
			NamespaceResolveResult nspace   = res as NamespaceResolveResult;
			MethodGroupResolveResult method = res as MethodGroupResolveResult;
			TypeResolveResult types         = res as TypeResolveResult;

			if (member != null && member.ResolvedMember != null)
			{
				this.AddToStringCollection(0, member.ResolvedMember.FullyQualifiedName);
			}
			if (nspace != null)
			{
				this.AddToStringCollection(0, nspace.Name);
			}
			if (method != null && method.ContainingType != null)
			{
				this.AddToStringCollection(0, method.ContainingType.FullyQualifiedName);
			}
			if (types != null && types.ResolvedClass != null)
			{
				this.AddToStringCollection(0, types.ResolvedClass.FullyQualifiedName);
			}

			BuildDynamicHelpList();
		}

		private ResolveResult ResolveAtCaret(ParserUpdateStepEventArgs e)
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null) return null;
			ITextEditorControlProvider provider = window.ActiveViewContent as ITextEditorControlProvider;
			if (provider == null) return null;
			TextEditorControl ctl = provider.TextEditorControl;

			// e might be null when this is a manually triggered update
			string fileName = (e == null) ? ctl.FileName : e.FileName;
			if (ctl.FileName != fileName) return null;
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(fileName);
			if (expressionFinder == null) return null;
			Caret caret = ctl.ActiveTextAreaControl.Caret;
			string content = (e == null) ? ctl.Text : e.Content;
			ExpressionResult expr = expressionFinder.FindFullExpression(content, caret.Offset);
			if (expr.Expression == null) return null;

			// save the current position
			if(this.lastPoint != null && this.lastPoint == caret.Position) return null;
			this.lastPoint = caret.Position;
			this.AddToStringCollection(string.Format(CultureInfo.InvariantCulture, "!{0}", expr.Expression));

			return ParserService.Resolve(expr, caret.Line + 1, caret.Column + 1, fileName, content);
		}
		#endregion

		#region Dynamic Help for Forms Designer
		private void FormsDesignerSelectedObjectChanged(object sender, EventArgs e)
		{
			this.CallDynamicHelpForFormsDesigner(PropertyPad.Grid.SelectedObject,
			                                     PropertyPad.Grid.SelectedGridItem);
		}

		private void FormsDesignerSelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
		{
			this.CallDynamicHelpForFormsDesigner(PropertyPad.Grid.SelectedObject,
			                                     e.NewSelection);
		}

		private void CallDynamicHelpForFormsDesigner(object selectedObject, GridItem selectedItem)
		{
			if (selectedObject == null) return;
			this.dynamicHelpTerms.Clear();

			Type myObject = selectedObject.GetType();
			if (selectedItem != null && selectedItem.Label != null) {
				foreach (Type type in TypeHandling.FindDeclaringType(myObject, selectedItem.Label)) {
					this.AddToStringCollection(string.Format(CultureInfo.InvariantCulture,
					                                         "{0}.{1}", type.FullName, selectedItem.Label));
				}
			}
			this.AddToStringCollection(myObject.FullName);

			WorkbenchSingleton.SafeThreadAsyncCall(BuildDynamicHelpList);
		}
		#endregion

		private void SolutionClosed(object sender, EventArgs e)
		{
			dynamicHelpBrowser.RemoveAllChildren();
		}

		#region StringCollection & Sorting
		private void AddToStringCollection(string searchTerm)
		{
			this.AddToStringCollection(-1, searchTerm);
		}

		private void AddToStringCollection(int insertWhere, string searchTerm)
		{
			if (this.dynamicHelpTerms.IndexOf(searchTerm) == -1)
			{
				if (insertWhere == -1)
					this.dynamicHelpTerms.Add(searchTerm);
				else
					this.dynamicHelpTerms.Insert(insertWhere, searchTerm);
			}
		}

		private static List<IHxTopic> SortTopics(IHxTopicList topics)
		{
			if (topics == null || topics.Count == 0)
			{
				return null;
			}

			List<IHxTopic> result = new List<IHxTopic>();
			foreach (IHxTopic topic in topics)
			{
				if (!result.Contains(topic)) result.Add(topic);
			}
			TopicComparer topicComparer = new TopicComparer();
			result.Sort(topicComparer);

			return result;
		}

		class TopicComparer : IComparer<IHxTopic>
		{
			public int Compare(IHxTopic x, IHxTopic y)
			{
				int result             = CompareType("kbSyntax", x, y);
				if(result == 0) result = CompareType("kbHowTo", x, y);
				if(result == 0) result = CompareType("kbOrient", x, y);
				if(result == 0) result = CompareType("kbArticle", x, y);

				return result;
			}

			private static int CompareType(string topicType, IHxTopic x, IHxTopic y)
			{
				if(x.HasAttribute("TopicType", topicType) && !y.HasAttribute("TopicType", topicType))
					return -1;
				else if(y.HasAttribute("TopicType", topicType) && !x.HasAttribute("TopicType", topicType))
					return 1;
				else
					return 0;
			}
		}
		#endregion

		private void NamespaceReloaded(object sender, EventArgs e)
		{
			this.enableDebugInfo = HtmlHelp2Environment.Config.DynamicHelpDebugInfos;
		}
	}

	public class HtmlHelp2DynamicHelpBrowserControl : UserControl
	{
		WebBrowser axWebBrowser      = new WebBrowser();
		ToolStrip dynamicHelpToolbar = new ToolStrip();
		int internalIndex;
		string[] toolbarButtons      = new string[] {
			"${res:AddIns.HtmlHelp2.Contents}",
			"${res:AddIns.HtmlHelp2.Index}",
			"${res:AddIns.HtmlHelp2.Search}"
		};

		public void RedrawContent()
		{
			for (int i = 0; i < toolbarButtons.Length; i++)
			{
				dynamicHelpToolbar.Items[i].Text = StringParser.Parse(toolbarButtons[i]);
			}
		}

		[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
		public HtmlHelp2DynamicHelpBrowserControl()
		{
			this.InitializeComponents();
		}

		[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
		private void InitializeComponents()
		{
			Dock = DockStyle.Fill;
			Size = new Size(500, 500);

			Controls.Add(axWebBrowser);
			axWebBrowser.Dock                            = DockStyle.Fill;
			axWebBrowser.WebBrowserShortcutsEnabled      = false;
			axWebBrowser.IsWebBrowserContextMenuEnabled  = false;
			axWebBrowser.AllowWebBrowserDrop             = false;
			axWebBrowser.DocumentCompleted              +=
				new WebBrowserDocumentCompletedEventHandler(this.OnDocumentCompleted);

			Controls.Add(dynamicHelpToolbar);
			dynamicHelpToolbar.Dock                      = DockStyle.Top;
			dynamicHelpToolbar.AllowItemReorder          = false;
			dynamicHelpToolbar.ShowItemToolTips          = false;
			dynamicHelpToolbar.GripStyle                 = ToolStripGripStyle.Hidden;
			for (int i = 0; i < toolbarButtons.Length; i++)
			{
				ToolStripButton button = new ToolStripButton();
				button.Font            = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				button.ImageIndex      = i;
				button.Click          += new EventHandler(this.ToolStripButtonClicked);

				dynamicHelpToolbar.Items.Add(button);
			}
			
			this.RedrawContent();

			dynamicHelpToolbar.ImageList            = new ImageList();
			dynamicHelpToolbar.ImageList.ColorDepth = ColorDepth.Depth32Bit;
			dynamicHelpToolbar.ImageList.Images.Add(ResourcesHelper.GetBitmap("HtmlHelp2.16x16.Toc.png"));
			dynamicHelpToolbar.ImageList.Images.Add(ResourcesHelper.GetBitmap("HtmlHelp2.16x16.Index.png"));
			dynamicHelpToolbar.ImageList.Images.Add(ResourcesHelper.GetBitmap("HtmlHelp2.16x16.Search.png"));

			if (HtmlHelp2Environment.SessionIsInitialized)
			{
				HtmlHelp2Environment.NamespaceReloaded   += new EventHandler(this.NamespaceReloaded);
			}
		}

		[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
		public void LoadDynamicHelpPage()
		{
			string url = string.Format(CultureInfo.InvariantCulture, "res://{0}/context", Assembly.GetExecutingAssembly().Location);
			axWebBrowser.Navigate(url);
		}

		private void OnDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			this.RemoveAllChildren();
		}

		private void ToolStripButtonClicked(object sender, EventArgs e)
		{
			ToolStripItem item = (ToolStripItem)sender;
			PadDescriptor pad  = null;

			switch (item.ImageIndex)
			{
				case 0:
					pad = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2TocPad));
					break;
				case 1:
					pad = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2IndexPad));
					break;
				case 2:
					pad = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2SearchPad));
					break;
			}

			if(pad != null) pad.BringPadToFront();
		}

		#region Help 2.0 Environment Events
		private void NamespaceReloaded(object sender, EventArgs e)
		{
			this.LoadDynamicHelpPage();
		}
		#endregion

		#region WebBrowser Scripting
		[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
		public void BuildNewChild(string sectionName, string topicName, string topicLink)
		{
			HtmlElementCollection children =
				axWebBrowser.Document.Body.GetElementsByTagName("span");
			foreach (HtmlElement child in children)
			{
				if (child.GetAttribute("className") == "section")
				{
					HtmlElement sectionBlock = child.FirstChild.NextSibling;
					HtmlElement contentSpan = sectionBlock.NextSibling.NextSibling;

					if (sectionBlock.TagName == "B" &&
					    sectionBlock.InnerText == sectionName &&
					    contentSpan.TagName == "SPAN" &&
					    contentSpan.GetAttribute("className") == "content")
					{
						if (!DoesLinkExist(contentSpan, topicName, topicLink))
						{
							HtmlElement newLink = this.CreateNewLink(topicLink, topicName);
							if (newLink != null)
							{
								contentSpan.AppendChild(newLink);
								contentSpan.AppendChild(this.CreateABreak());
							}
						}

						return;
					}
				}
			}

			if (children.Count > 0)
			{
				axWebBrowser.Document.Body.InsertAdjacentElement
					(HtmlElementInsertionOrientation.BeforeEnd, this.CreateABreak());
			}

			HtmlElement linkContent = null;
			HtmlElement htmlSection = this.CreateNewSection(sectionName, out linkContent);
			if (htmlSection != null)
			{
				axWebBrowser.Document.Body.InsertAdjacentElement
					(HtmlElementInsertionOrientation.BeforeEnd, htmlSection);

				HtmlElement newLink = this.CreateNewLink(topicLink, topicName);
				if (newLink != null)
				{
					linkContent.AppendChild(newLink);
					linkContent.AppendChild(this.CreateABreak());
				}

				this.internalIndex++;
			}
		}

		[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
		private HtmlElement CreateNewSection(string sectionName, out HtmlElement linkNode)
		{
			HtmlElement span = axWebBrowser.Document.CreateElement("span");
			span.SetAttribute("className", "section");
			span.InnerHtml = string.Format
				(CultureInfo.InvariantCulture,
				 "<img style=\"width:16px;height:16px;margin-right:5px\" id=\"image_{0}\" src=\"open\">" +
				 "<b style=\"cursor:auto;\" id=\"{0}\" onclick=\"ExpandCollapse({0})\">{1}</b><br>",
				 this.internalIndex, sectionName);

			linkNode = axWebBrowser.Document.CreateElement("span");
			linkNode.SetAttribute("className", "content");
			linkNode.Id = string.Format(CultureInfo.InvariantCulture, "content_{0}", this.internalIndex);
			span.AppendChild(linkNode);

			return span;
		}

		[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
		private HtmlElement CreateNewLink(string topicUrl, string topicName)
		{
			HtmlElement span = axWebBrowser.Document.CreateElement("a");
			span.InnerText   = topicName;
			span.SetAttribute("src", topicUrl);
			span.SetAttribute("className", "link");
			span.SetAttribute("title", topicName);
			span.Click      += new HtmlElementEventHandler(OnLinkClick);
			span.MouseOver  += new HtmlElementEventHandler(OnMouseOver);
			span.MouseLeave += new HtmlElementEventHandler(OnMouseOut);

			return span;
		}

		[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
		private HtmlElement CreateABreak()
		{
			HtmlElement br = axWebBrowser.Document.CreateElement("br");
			return br;
		}

		[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
		private static bool DoesLinkExist(HtmlElement parentNode, string topicName, string topicUrl)
		{
			HtmlElementCollection links = parentNode.GetElementsByTagName("a");
			foreach (HtmlElement link in links)
			{
				if (string.Compare(topicName, link.InnerText) == 0 &&
				    string.Compare(topicUrl, link.GetAttribute("src")) == 0)
				{
					return true;
				}
			}
			return false;
		}

		private void OnMouseOver(object sender, HtmlElementEventArgs e)
		{
			HtmlElement link = sender as HtmlElement;
			if (link != null)
			{
				StatusBarService.SetMessage(link.GetAttribute("src"));
			}
		}

		private void OnMouseOut(object sender, HtmlElementEventArgs e)
		{
			StatusBarService.SetMessage(string.Empty);
		}

		private void OnLinkClick(object sender, HtmlElementEventArgs e)
		{
			HtmlElement link = sender as HtmlElement;
			if (link != null)
			{
				string url = link.GetAttribute("src");
				if (!string.IsNullOrEmpty(url)) ShowHelpBrowser.OpenHelpView(url);
			}
		}

		[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
		public void RemoveAllChildren()
		{
			try
			{
				this.internalIndex = 0;
				var document = axWebBrowser != null ? axWebBrowser.Document : null;
				var body = document != null ? document.Body : null;
				if (body != null)
					body.InnerHtml = string.Empty;
			}
			catch (System.NotSupportedException ex)
			{
				LoggingService.Error("Help 2.0: Clean-up Call Exception; " + ex.ToString());
			}
		}
		#endregion

		#region DebugInfo
		[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
		public void CreateDebugPre(string debugInformation)
		{
			if (!string.IsNullOrEmpty(debugInformation))
			{
				axWebBrowser.Document.Body.InsertAdjacentElement
					(HtmlElementInsertionOrientation.BeforeEnd, CreateABreak());
				axWebBrowser.Document.Body.InsertAdjacentElement
					(HtmlElementInsertionOrientation.BeforeEnd, CreateABreak());

				HtmlElement pre = axWebBrowser.Document.CreateElement("pre");
				pre.InnerHtml = "--- Dynamic Help Debug ---<br>" + debugInformation;

				axWebBrowser.Document.Body.InsertAdjacentElement
					(HtmlElementInsertionOrientation.BeforeEnd, pre);
			}
		}
		#endregion
	}

	public static class TypeHandling
	{
		public static IEnumerable<Type> FindDeclaringType(Type type, string memberName)
		{
			MemberInfo[] memberInfos  = type.GetMember(memberName);
			List<Type> declaringTypes = new List<Type>();

			foreach (MemberInfo memberInfo in memberInfos)
			{
				if (!declaringTypes.Contains(memberInfo.DeclaringType))
					declaringTypes.Add(memberInfo.DeclaringType);
			}

			foreach (Type declaringType in declaringTypes)
			{
				yield return declaringType;
			}

			#region TypeHandling Class by Robert_G
			// QUOTE: "Aber das ist ja wohl eher ein no-Brainer... ;-)
			#endregion
		}
	}
}
