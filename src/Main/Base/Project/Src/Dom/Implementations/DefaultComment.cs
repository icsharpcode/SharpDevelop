// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom {
	
	[Serializable]
	public class DefaultComment : System.MarshalByRefObject, IComment
	{
		bool    isBlockComment;
		string  commentTag;
		string  commentText;
		IRegion region;
		
		public DefaultComment(bool isBlockComment, string commentTag, string commentText, IRegion region)
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
		
		public IRegion Region {
			get {
				return region;
			}
		}
	}
}
