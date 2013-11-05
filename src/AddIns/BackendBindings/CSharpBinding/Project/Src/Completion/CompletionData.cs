// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace CSharpBinding.Completion
{
	class CompletionData : ICompletionData, ICompletionItem, IFancyCompletionItem
	{
		public CompletionData(string text = "")
		{
			this.DisplayText = text;
			this.CompletionText = text;
		}
		
		public CompletionCategory CompletionCategory { get; set; }
		public string DisplayText { get; set; }
		public string Description { get; set; }
		public string CompletionText { get; set; }
		
		DisplayFlags displayFlags;
		DisplayFlags ICompletionData.DisplayFlags {
			get { return displayFlags; }
			set { displayFlags = value; }
		}
		
//		List<ICompletionData> overloads;
//
//		public virtual void AddOverload(ICompletionData data)
//		{
//			if (overloads == null)
//				overloads = new List<ICompletionData>();
//			overloads.Add(data);
//		}
//
//		public virtual bool HasOverloads {
//			get { return overloads != null && overloads.Count > 0; }
//		}
//
//		public virtual IEnumerable<ICompletionData> OverloadedData {
//			get { return overloads ?? EmptyList<ICompletionData>.Instance; }
//		}
		
		public virtual bool HasOverloads { get { return false; } }
		
		public virtual IEnumerable<ICompletionData> OverloadedData {
			get { return EmptyList<ICompletionData>.Instance; }
		}
		
		public virtual void AddOverload(ICompletionData data)
		{
		}
		
		string ICompletionItem.Text {
			get { return this.CompletionText; }
		}
		
		public IImage Image { get; set; }
		
		public virtual double Priority {
			get { return 0; }
		}
		
		public virtual void Complete(CompletionContext context)
		{
			context.Editor.Document.Replace(context.StartOffset, context.Length, this.CompletionText);
			context.EndOffset = context.StartOffset + this.CompletionText.Length;
		}
		
		object IFancyCompletionItem.Content {
			get { return this.DisplayText; }
		}
		
		object IFancyCompletionItem.Description {
			get { return this.Description; }
		}
	}
}
