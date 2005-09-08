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
	using System.Drawing;
	using System.Windows.Forms;
	using System.Reflection;
	using System.IO;
	using ICSharpCode.Core;
	using ICSharpCode.SharpDevelop;
	using ICSharpCode.SharpDevelop.Gui;
	using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
	using ICSharpCode.SharpDevelop.Dom;
	using ICSharpCode.TextEditor;
	using HtmlHelp2.Environment;
	using HtmlHelp2.ResourcesHelperClass;
	using MSHelpServices;


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
		private int internalIndex = 0;
		private string lastDynamicHelpWord = String.Empty;

		public override Control Control
		{
			get { return dynamicHelpBrowser; }
		}

		public override void Dispose()
		{
			try
			{
				dynamicHelpBrowser.Dispose();
			}
			catch {}
		}

		public override void RedrawContent()
		{
			dynamicHelpBrowser.RedrawContent();
		}

		public HtmlHelp2DynamicHelpPad()
		{
			dynamicHelpBrowser = new HtmlHelp2DynamicHelpBrowserControl();
			ParserService.ParserUpdateStepFinished += UpdateTick;
		}

		#region WebBrowser Scripting
		public void BuildDynamicHelpList(string dynamicHelpString, string expectedLanguage)
		{
			if(!HtmlHelp2Environment.IsReady || HtmlHelp2Environment.DynamicHelpIsBusy) {
				return;
			}

			if(String.Compare(dynamicHelpString, this.lastDynamicHelpWord) == 0) return;

			try
			{
				this.RemoveAllChildren();

				Cursor.Current = Cursors.WaitCursor;
				IHxTopicList topics = HtmlHelp2Environment.GetMatchingTopicsForDynamicHelp(dynamicHelpString);
				Cursor.Current = Cursors.Default;

				if(topics.Count > 0)
				{
					for(int i = 1; i <= topics.Count; i++)
					{
						IHxTopic topic = topics.ItemAt(i);

						if(expectedLanguage == null || expectedLanguage == "" || topic.HasAttribute("DevLang", expectedLanguage))
						{
							this.BuildNewChild(topic.Location,
							                   topic.get_Title(HxTopicGetTitleType.HxTopicGetRLTitle,HxTopicGetTitleDefVal.HxTopicGetTitleFileName),
							                   topic.URL);
						}
					}
				}

				this.lastDynamicHelpWord = dynamicHelpString;
			}
			catch {}
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
							try
							{
								HtmlElement sectionBlock = elem.FirstChild.NextSibling;
								HtmlElement contentSpan  = sectionBlock.NextSibling.NextSibling;

								if(sectionBlock.TagName == "B" && sectionBlock.InnerText == sectionName &&
								   contentSpan.TagName == "SPAN" && contentSpan.GetAttribute("className") == "content")
								{
									contentSpan.AppendChild(this.CreateNewLink(topicUrl, topicName));
									contentSpan.AppendChild(this.CreateABreak());

									return;
								}
							}
							catch {}
						}
					}
				
					dynamicHelpBrowser.Document.Body.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeEnd,
					                                                       this.CreateABreak());
				}

				HtmlElement linkContent = null;
				HtmlElement htmlSection = this.CreateNewSection(sectionName, out linkContent);
				dynamicHelpBrowser.Document.Body.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeEnd, htmlSection);
				linkContent.AppendChild(this.CreateNewLink(topicUrl, topicName));
				linkContent.AppendChild(this.CreateABreak());

				this.internalIndex++;
			}
			catch {}
		}

		private HtmlElement CreateNewSection(string sectionName, out HtmlElement linkNode)
		{
			HtmlElement span = null;
			linkNode         = null;

			try
			{
				span                = dynamicHelpBrowser.CreateHtmlElement("span");
//				span                = dynamicHelpBrowser.Document.CreateElement("span");
				span.SetAttribute("className", "section");

				HtmlElement img     = dynamicHelpBrowser.CreateHtmlElement("img");
//				HtmlElement img     = dynamicHelpBrowser.Document.CreateElement("img");
				img.Style           = "width:16px;height:16px;margin-right:5px";
				img.Id              = String.Format("image_{0}", this.internalIndex.ToString());
				img.SetAttribute("src", "OpenBook.png");
				span.AppendChild(img);

				HtmlElement b       = dynamicHelpBrowser.CreateHtmlElement("b");
//				HtmlElement b       = dynamicHelpBrowser.Document.CreateElement("b");
				b.InnerText         = sectionName;
				b.Style             = "cursor:pointer";
				b.SetAttribute("title", this.internalIndex.ToString());
				b.Click            += new HtmlElementEventHandler(this.OnSectionClick);
				span.AppendChild(b);

				span.AppendChild(this.CreateABreak());

				HtmlElement content = dynamicHelpBrowser.CreateHtmlElement("span");
//				HtmlElement content = dynamicHelpBrowser.Document.CreateElement("span");
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
			HtmlElement span = null;

			try
			{
				span             = dynamicHelpBrowser.CreateHtmlElement("span");
//				span             = dynamicHelpBrowser.Document.CreateElement("span");
				span.InnerText   = topicName;
				span.SetAttribute("className", "link");
				span.SetAttribute("title", topicUrl);
				span.MouseOver  += new HtmlElementEventHandler(OnMouseOver);
				span.MouseLeave += new HtmlElementEventHandler(OnMouseOut);
				span.Click      += new HtmlElementEventHandler(OnLinkClick);
			}
			catch {}

			return span;
		}

		private HtmlElement CreateABreak()
		{
			HtmlElement br = null;

			try
			{
				br = dynamicHelpBrowser.CreateHtmlElement("br");
//				br = dynamicHelpBrowser.Document.CreateElement("br");
			}
			catch {}

			return br;
		}
		#endregion

		private void OnMouseOver(object sender, HtmlElementEventArgs e)
		{
			try
			{
				StatusBarService.SetMessage(((HtmlElement)sender).GetAttribute("title"));
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
				string sectionId  = ((HtmlElement)sender).GetAttribute("title");
				object[] objArray = new object[1];
				objArray[0]       = (object)sectionId;
				dynamicHelpBrowser.Document.InvokeScript("ExpandCollapse", objArray);
			}
			catch {}
		}

		private void OnLinkClick(object sender, HtmlElementEventArgs e)
		{
			try
			{
				string url = ((HtmlElement)sender).GetAttribute("title");
				if(url != null && url != String.Empty) ShowHelpBrowser.OpenHelpView(url);
			}
			catch {}
		}

		#region Taken from DefinitionView.cs
		private void UpdateTick(object sender, ParserUpdateStepEventArgs e)
		{
//			if (!this.IsVisible) return;

			try
			{
				ResolveResult res = ResolveAtCaret(e);
				if (res == null || res.ResolvedType == null) return;
				WorkbenchSingleton.SafeThreadAsyncCall(this,
				                                       "BuildDynamicHelpList",
				                                       res.ResolvedType.FullyQualifiedName,
				                                       "");
				// thanks again to Daniel and Robert
			}
			catch {}
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
			try
			{
				axWebBrowser.Document.Body.InnerHtml = "";
			}
			catch {}
		}

		public HtmlElement CreateHtmlElement(string elementName)
		{
			try
			{
				HtmlElement newElement = axWebBrowser.Document.CreateElement(elementName);
				return newElement;
			}
			catch
			{
				return null;
			}
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
			axWebBrowser.Dock = DockStyle.Fill;
			axWebBrowser.WebBrowserShortcutsEnabled = false;
			axWebBrowser.IsWebBrowserContextMenuEnabled  = false;
			axWebBrowser.AllowWebBrowserDrop = false;

			Controls.Add(dynamicHelpToolbar);
			dynamicHelpToolbar.Dock             = DockStyle.Top;
			dynamicHelpToolbar.AllowItemReorder = false;
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
			try
			{
				string url = String.Format("{0}\\context.html",
				                           Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

				if(!File.Exists(url)) url = "about:blank";
				axWebBrowser.Navigate(url);
			}
			catch {}
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
}
