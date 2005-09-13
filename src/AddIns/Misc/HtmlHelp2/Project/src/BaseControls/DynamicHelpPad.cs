/* ***********************************************************
 *
 * Help 2.0 Environment for SharpDevelop
 * Dynamic Help Pad
 * Copyright (c) 2005, Mathias Simmack. All rights reserved.
 *
 * ********************************************************* */
namespace HtmlHelp2
{
	using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
	using System.Drawing;
	using System.Drawing.Design;
	using System.Windows.Forms;
	using System.Reflection;
	using System.IO;
	using System.Xml;
	using ICSharpCode.Core;
	using ICSharpCode.SharpDevelop;
	using ICSharpCode.SharpDevelop.Gui;
	using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
	using ICSharpCode.SharpDevelop.Dom;
	using ICSharpCode.SharpDevelop.Project;
	using ICSharpCode.TextEditor;
	using HtmlHelp2.Environment;
	using HtmlHelp2.ResourcesHelperClass;
	using MSHelpServices;
	
	
	// TODO: detect the active language (CSharp, VB#, ...) to limit the results
	// TODO: if there are no DH results, implement Keyword search (index)
	// TODO: insert some default entries (Homepage, Wiki, Community, ...)


	public class ShowDynamicHelpMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PadDescriptor dynamicHelp = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2DynamicHelpPad));
			if(dynamicHelp != null) dynamicHelp.BringPadToFront();
		}
	}

	public class HtmlHelp2DynamicHelpPad : AbstractPadContent
	{
		protected HtmlHelp2DynamicHelpBrowserControl dynamicHelpBrowser;
		private int internalIndex                 = 0;
		private StringCollection dynamicHelpTerms = new StringCollection();
		private string debugPreElement            = String.Empty;
		private bool enableDebugInfo              = false;

		public override Control Control
		{
			get { return dynamicHelpBrowser; }
		}

//		public override void Dispose()
//		{
//			dynamicHelpBrowser.Dispose();
//		}

		public override void RedrawContent()
		{
			dynamicHelpBrowser.RedrawContent();
		}

		public HtmlHelp2DynamicHelpPad()
		{
			dynamicHelpBrowser                      = new HtmlHelp2DynamicHelpBrowserControl();
			ParserService.ParserUpdateStepFinished += UpdateTick;
			PropertyPad.SelectedObjectChanged      += new EventHandler(this.FormsDesignerSelectedObjectChanged);
			PropertyPad.SelectedGridItemChanged    += new SelectedGridItemChangedEventHandler(this.FormsDesignerSelectedGridItemChanged);
			ProjectService.SolutionClosed          += new EventHandler(this.SolutionClosed);
		}

		#region WebBrowser Scripting
		private void BuildDynamicHelpList(string expectedLanguage)
		{
			if(this.dynamicHelpTerms.Count == 0) return;
			this.RemoveAllChildren();
			this.debugPreElement     = String.Empty;

			Cursor.Current           = Cursors.WaitCursor;
			foreach(string currentHelpTerm in dynamicHelpTerms)
			{
				this.CallDynamicHelp(currentHelpTerm, expectedLanguage, false);
			}
			Cursor.Current           = Cursors.Default;
			if(this.enableDebugInfo) this.CreateDebugPre();
		}

		private bool CallDynamicHelp(string searchTerm, string expectedLanguage, bool keywordSearch)
		{
			if(!HtmlHelp2Environment.IsReady || HtmlHelp2Environment.DynamicHelpIsBusy) return false;
			bool result          = false;
			IHxTopicList topics  = null;

			try
			{
				if(keywordSearch) topics = HtmlHelp2Environment.GetMatchingTopicsForKeywordSearch(searchTerm);
				else topics              = HtmlHelp2Environment.GetMatchingTopicsForDynamicHelp(searchTerm);
				result                   = (topics != null && topics.Count > 0);

				this.debugPreElement    += String.Format("{0} ({1}): {2} {3}<br>",
				                                         searchTerm, (keywordSearch)?"Kwd":"DH",
				                                         topics.Count.ToString(), (topics.Count == 1)?"topic":"topics");
			}
			catch {}

			if(result)
			{
				List<IHxTopic> newTopics = this.SortTopics(topics);
				foreach(IHxTopic topic in newTopics)
				{
					this.BuildNewChild(topic.Location,
					                   topic.get_Title(HxTopicGetTitleType.HxTopicGetTOCTitle,
					                                   HxTopicGetTitleDefVal.HxTopicGetTitleFileName),
					                   topic.URL);
				}
			}
			return result;
		}


		private void RemoveAllChildren()
		{
			this.internalIndex = 0;
			dynamicHelpBrowser.RemoveAllChildren();
		}

		private void BuildNewChild(string sectionName, string topicName, string topicUrl)
		{
			try
			{
				HtmlElementCollection children = dynamicHelpBrowser.Document.Body.GetElementsByTagName("span");

				if(children.Count > 0)
				{
					foreach(HtmlElement elem in children)
					{
						if(elem.GetAttribute("className") == "section")
						{
							HtmlElement sectionBlock = elem.FirstChild.NextSibling;
							HtmlElement contentSpan  = sectionBlock.NextSibling.NextSibling;

							if(sectionBlock.TagName == "B" && sectionBlock.InnerText == sectionName &&
							   contentSpan.TagName == "SPAN" && contentSpan.GetAttribute("className") == "content")
							{
								if(!this.DoesLinkExist(contentSpan, topicName, topicUrl))
								{
									contentSpan.AppendChild(this.CreateNewLink(topicUrl, topicName));
									contentSpan.AppendChild(this.CreateABreak());
								}

								return;
							}
						}
					}

					dynamicHelpBrowser.InsertHtmlElement(HtmlElementInsertionOrientation.BeforeEnd,
					                                     this.CreateABreak());
				}

				HtmlElement linkContent = null;
				HtmlElement htmlSection = this.CreateNewSection(sectionName, out linkContent);
				dynamicHelpBrowser.InsertHtmlElement(HtmlElementInsertionOrientation.BeforeEnd, htmlSection);
				linkContent.AppendChild(this.CreateNewLink(topicUrl, topicName));
				linkContent.AppendChild(this.CreateABreak());

				this.internalIndex++;
			}
			catch(Exception ex)
			{
				LoggingService.Error("Help 2.0: cannot build node for dynamic help; " + ex.ToString());
			}
		}

		private HtmlElement CreateNewSection(string sectionName, out HtmlElement linkNode)
		{
			HtmlElement span        = null;
			linkNode                = null;

			try
			{
				span                = dynamicHelpBrowser.CreateHtmlElement("span");
				span.SetAttribute("className", "section");

				HtmlElement img     = dynamicHelpBrowser.CreateHtmlElement("img");
				img.Style           = "width:16px;height:16px;margin-right:5px";
				img.Id              = String.Format("image_{0}", this.internalIndex.ToString());
				img.SetAttribute("src", "OpenBook.png");
				span.AppendChild(img);

				HtmlElement b       = dynamicHelpBrowser.CreateHtmlElement("b");
				b.InnerText         = sectionName;
				b.Style             = "cursor:pointer";
				b.Id                = this.internalIndex.ToString();
				b.Click            += new HtmlElementEventHandler(this.OnSectionClick);
				span.AppendChild(b);

				span.AppendChild(this.CreateABreak());

				HtmlElement content = dynamicHelpBrowser.CreateHtmlElement("span");
				content.Id          = String.Format("content_{0}", this.internalIndex.ToString());
				content.SetAttribute("className", "content");
				span.AppendChild(content);

				linkNode            = content;
			}
			catch {}

			return span;
		}

		private HtmlElement CreateNewLink(string topicUrl, string topicName)
		{
			HtmlElement span     = null;

			try
			{
				span             = dynamicHelpBrowser.CreateHtmlElement("a");
				span.InnerText   = topicName;
				span.SetAttribute("src", topicUrl);
				span.SetAttribute("className", "link");
				span.SetAttribute("title", topicName);
				span.Click      += new HtmlElementEventHandler(OnLinkClick);
				span.MouseOver  += new HtmlElementEventHandler(OnMouseOver);
				span.MouseLeave += new HtmlElementEventHandler(OnMouseOut);
			}
			catch {}

			return span;
		}

		private HtmlElement CreateABreak()
		{
			HtmlElement br = null;

			try
			{
				br         = dynamicHelpBrowser.CreateHtmlElement("br");
			}
			catch {}

			return br;
		}

		private bool DoesLinkExist(HtmlElement parentNode, string topicName, string topicUrl)
		{
			try
			{
				HtmlElementCollection allLinks = parentNode.GetElementsByTagName("a");
				if(allLinks.Count > 0)
				{
					foreach(HtmlElement link in allLinks)
					{
						if(String.Compare(topicName, link.InnerText) == 0 &&
						   String.Compare(topicUrl, link.GetAttribute("src")) == 0)
						{
							return true;
						}
					}
				}
			}
			catch { }

			return false;
		}
		#endregion

		private void OnMouseOver(object sender, HtmlElementEventArgs e)
		{
			try
			{
				StatusBarService.SetMessage(((HtmlElement)sender).GetAttribute("src"));
			}
			catch {}
		}

		private void OnMouseOut(object sender, HtmlElementEventArgs e)
		{
			StatusBarService.SetMessage("");
		}

		private void OnSectionClick(object sender, HtmlElementEventArgs e)
		{
			try
			{
				string sectionId  = ((HtmlElement)sender).Id;
				object[] objArray = new object[1];
				objArray[0]       = (object)sectionId;
				dynamicHelpBrowser.Document.InvokeScript("ExpandCollapse", objArray);
			}
			catch(Exception ex)
			{
				LoggingService.Error("Help 2.0: cannot run script; " + ex.ToString());
			}
		}

		private void OnLinkClick(object sender, HtmlElementEventArgs e)
		{
			try
			{
				string url = ((HtmlElement)sender).GetAttribute("src");
				if(url != null && url != String.Empty) ShowHelpBrowser.OpenHelpView(url);
			}
			catch {}
		}

		#region Taken from DefinitionView.cs
		private void UpdateTick(object sender, ParserUpdateStepEventArgs e)
		{
			this.dynamicHelpTerms.Clear();

			ResolveResult res = ResolveAtCaret(e);
			if(res == null) return;

			if(res != null && res.ResolvedType != null)
			{
				this.AddToStringCollection(res.ResolvedType.FullyQualifiedName);
			}

			MemberResolveResult member    = res as MemberResolveResult;
			NamespaceResolveResult nspace = res as NamespaceResolveResult;
			MethodResolveResult method    = res as MethodResolveResult;
			TypeResolveResult types       = res as TypeResolveResult;

			if(member != null && member.ResolvedMember != null)
			{
				this.AddToStringCollection(0, member.ResolvedMember.FullyQualifiedName);
			}
			if(nspace != null)
			{
				this.AddToStringCollection(0, nspace.Name);
			}
			if(method != null && method.ContainingType != null)
			{
				this.AddToStringCollection(0, method.ContainingType.FullyQualifiedName);
			}
			if(types != null && types.ResolvedClass != null)
			{
				this.AddToStringCollection(0, types.ResolvedClass.FullyQualifiedName);
			}

			WorkbenchSingleton.SafeThreadAsyncCall(this, "BuildDynamicHelpList", "");
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

			return ParserService.Resolve(expr, caret.Line, caret.Column, fileName, content);
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
			if(selectedObject == null) return;
			this.dynamicHelpTerms.Clear();

			Type myObject = selectedObject.GetType();
			if(selectedItem != null)
			{
				foreach(Type type in TypeHandling.FindDeclaringType(myObject, selectedItem.Label))
				{
					this.AddToStringCollection(String.Format("{0}.{1}", type.FullName, selectedItem.Label));
				}
			}
			this.AddToStringCollection(myObject.FullName);

			WorkbenchSingleton.SafeThreadAsyncCall(this, "BuildDynamicHelpList", "");
		}
		#endregion

		#region DebugInfo
		private void CreateDebugPre()
		{
			if(this.debugPreElement == String.Empty) return;

			try
			{
				dynamicHelpBrowser.Document.Body.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeEnd,
				                                                       this.CreateABreak());
				dynamicHelpBrowser.Document.Body.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeEnd,
				                                                       this.CreateABreak());

				HtmlElement pre = dynamicHelpBrowser.CreateHtmlElement("pre");
				pre.InnerHtml   = "--- Dynamic Help Debug ---<br>" + this.debugPreElement;

				dynamicHelpBrowser.InsertHtmlElement(HtmlElementInsertionOrientation.BeforeEnd, pre);
			}
			catch {}
		}
		#endregion

		private void SolutionClosed(object sender, EventArgs e)
		{
			this.RemoveAllChildren();
		}

		#region StringCollection & Sorting
		private void AddToStringCollection(string searchTerm)
		{
			this.AddToStringCollection(-1, searchTerm);
		}

		private void AddToStringCollection(int insertWhere, string searchTerm)
		{
			if(this.dynamicHelpTerms.IndexOf(searchTerm) == -1)
			{
				if(insertWhere == -1) this.dynamicHelpTerms.Add(searchTerm);
				else this.dynamicHelpTerms.Insert(insertWhere, searchTerm);
			}
		}

		private List<IHxTopic> SortTopics(IHxTopicList topics)
		{
			List<IHxTopic> result = new List<IHxTopic>();

			try
			{
				if(topics != null && topics.Count > 0)
				{
					foreach(IHxTopic topic in topics)
					{
						if(!result.Contains(topic)) result.Add(topic);
					}
					
					TopicComparer topicComparer = new TopicComparer();
					result.Sort(topicComparer);
				}
			}
			catch(Exception ex)
			{
				LoggingService.Error("Help 2.0: error while rebuild topics; " + ex.ToString());
			}

			return result;
		}

		class TopicComparer : IComparer<IHxTopic>
		{
			public int Compare(IHxTopic x, IHxTopic y)
			{
				int result             = CompareType("kbSyntax", x, y);
				if(result == 0) result = CompareType("kbHowTo", x, y);
				if(result == 0) result = CompareType("kbArticle", x, y);

				return result;
			}

			private int CompareType(string topicType, IHxTopic x, IHxTopic y)
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
	}

	public class HtmlHelp2DynamicHelpBrowserControl : UserControl
	{
		WebBrowser axWebBrowser      = new WebBrowser();
		ToolStrip dynamicHelpToolbar = new ToolStrip();
		string[] toolbarButtons      = new string[] {
			"${res:AddIns.HtmlHelp2.Contents}",
			"${res:AddIns.HtmlHelp2.Index}",
			"${res:AddIns.HtmlHelp2.Search}"
		};

		public HtmlDocument Document
		{
			get { return axWebBrowser.Document;	}
		}

		public void RemoveAllChildren()
		{
			axWebBrowser.Document.Body.InnerHtml = "";
		}

		public HtmlElement CreateHtmlElement(string elementName)
		{
			HtmlElement newElement = axWebBrowser.Document.CreateElement(elementName);
			return newElement;
		}

		public void InsertHtmlElement(HtmlElementInsertionOrientation insertWhere, HtmlElement insertWhat)
		{
			axWebBrowser.Document.Body.InsertAdjacentElement(insertWhere, insertWhat);
		}


		public void RedrawContent()
		{
			for(int i = 0; i < toolbarButtons.Length; i++)
			{
				dynamicHelpToolbar.Items[i].ToolTipText = StringParser.Parse(toolbarButtons[i]);
			}
		}

		public HtmlHelp2DynamicHelpBrowserControl()
		{
			this.InitializeComponents();
			this.LoadDynamicHelpPage();
		}

		private void InitializeComponents()
		{
			Dock = DockStyle.Fill;
			Size = new Size(500, 500);

			Controls.Add(axWebBrowser);
			axWebBrowser.Dock                            = DockStyle.Fill;
			axWebBrowser.WebBrowserShortcutsEnabled      = false;
			axWebBrowser.IsWebBrowserContextMenuEnabled  = false;
			axWebBrowser.AllowWebBrowserDrop             = false;

			Controls.Add(dynamicHelpToolbar);
			dynamicHelpToolbar.Dock                      = DockStyle.Top;
			dynamicHelpToolbar.AllowItemReorder          = false;
			dynamicHelpToolbar.Enabled                   = HtmlHelp2Environment.IsReady;
			for(int i = 0; i < toolbarButtons.Length; i++)
			{
				ToolStripButton button = new ToolStripButton();
				button.ToolTipText     = StringParser.Parse(toolbarButtons[i]);
				button.ImageIndex      = i;
				button.Click          += new EventHandler(this.ToolStripButtonClicked);

				dynamicHelpToolbar.Items.Add(button);
			}

			dynamicHelpToolbar.ImageList            = new ImageList();
			dynamicHelpToolbar.ImageList.ColorDepth = ColorDepth.Depth32Bit;
			dynamicHelpToolbar.ImageList.Images.Add(ResourcesHelper.GetBitmap("HtmlHelp2.16x16.Toc.png"));
			dynamicHelpToolbar.ImageList.Images.Add(ResourcesHelper.GetBitmap("HtmlHelp2.16x16.Index.png"));
			dynamicHelpToolbar.ImageList.Images.Add(ResourcesHelper.GetBitmap("HtmlHelp2.16x16.Search.png"));

			if(HtmlHelp2Environment.IsReady)
			{
				HtmlHelp2Environment.NamespaceReloaded   += new EventHandler(this.NamespaceReloaded);
			}
		}

		private void LoadDynamicHelpPage()
		{
			if(!HtmlHelp2Environment.IsReady) return;

			string url = String.Format("{0}\\context.html",
			                           Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

			if(!File.Exists(url)) url = "about:blank";
			axWebBrowser.Navigate(url);
		}

		private void ToolStripButtonClicked(object sender, EventArgs e)
		{
			ToolStripItem item = (ToolStripItem)sender;
			PadDescriptor pad  = null;

			switch(item.ImageIndex)
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
	}

	#region TypeHandling by Robert_G
	public static class TypeHandling
	{
		public static IEnumerable<Type> FindDeclaringType(Type type, string memberName)
		{
			MemberInfo[] memberInfos  = type.GetMember(memberName);
			List<Type> declaringTypes = new List<Type>();

			foreach(MemberInfo memberInfo in memberInfos)
			{
				if(!declaringTypes.Contains(memberInfo.DeclaringType))
					declaringTypes.Add(memberInfo.DeclaringType);
			}

			foreach(Type declaringType in declaringTypes)
			{
				yield return declaringType;
			}

			// QUOTE:
			// "Aber das ist wohl eher ein no-Brainer... ;-)"
			// (Robert)
		}
	}
	#endregion
}
