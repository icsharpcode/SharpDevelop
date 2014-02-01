// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		
		object fancyDescription;
		
		object IFancyCompletionItem.Description {
			get {
				if (fancyDescription == null) {
					fancyDescription = CreateFancyDescription();
				}
				return fancyDescription;
			}
		}
		
		protected virtual object CreateFancyDescription()
		{
			return Description;
		}
	}
}
