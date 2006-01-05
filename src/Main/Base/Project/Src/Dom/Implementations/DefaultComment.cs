// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom {
	
	[Serializable]
	public class DefaultComment : IComment
	{
		bool    isBlockComment;
		string  commentTag;
		string  commentText;
		DomRegion region;
		
		public DefaultComment(bool isBlockComment, string commentTag, string commentText, DomRegion region)
		{
			this.isBlockComment = isBlockComment;
			this.commentTag = commentTag;
			this.commentText = commentText;
			this.region = region;
		}
		
		public bool IsBlockComment {
			get {
				return isBlockComment;
			}
		}
		
		public string CommentTag {
			get {
				return commentTag;
			}
		}
		
		public string CommentText {
			get {
				return commentText;
			}
		}
		
		public DomRegion Region {
			get {
				return region;
			}
		}
	}
}
