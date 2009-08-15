// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Xml
{
	/// <summary>
	/// Holds all valid parsed items.
	/// Also tracks their offsets as document changes.
	/// </summary>
	class TrackedSegmentCollection
	{
		/// <summary> Previously parsed items as long as they are valid  </summary>
		TextSegmentCollection<AXmlObject> parsedItems = new TextSegmentCollection<AXmlObject>();
		
		/// <summary>
		/// Is used to identify what memory range was touched by object
		/// The default is (StartOffset, EndOffset + 1) which is not stored
		/// </summary>
		TextSegmentCollection<TouchedRange> touchedRanges = new TextSegmentCollection<TouchedRange>();
		
		/// <summary>
		/// Track offsets of syntax errors as well.
		/// Objects can not change in the cache, so not need to update it.
		/// </summary>
		TextSegmentCollection<SyntaxError> syntaxErrors = new TextSegmentCollection<SyntaxError>();
		
		class TouchedRange: TextSegment
		{
			public AXmlObject TouchedByObject { get; set; }
		}
		
		public void UpdateOffsetsAndInvalidate(IEnumerable<DocumentChangeEventArgs> changes)
		{
			foreach(DocumentChangeEventArgs change in changes) {
				// Update offsets of all items
				parsedItems.UpdateOffsets(change);
				touchedRanges.UpdateOffsets(change);
				syntaxErrors.UpdateOffsets(change);
				
				// Remove any items affected by the change
				AXmlParser.Log("Changed offset {0}", change.Offset);
				// Removing will cause one of the ends to be set to change.Offset
				// FindSegmentsContaining includes any segments touching
				// so that conviniently takes care of the +1 byte
				foreach(AXmlObject obj in parsedItems.FindSegmentsContaining(change.Offset)) {
					InvalidateCache(obj, false);
				}
				foreach(TouchedRange range in touchedRanges.FindSegmentsContaining(change.Offset)) {
					AXmlParser.Log("Found that {0} dependeds on ({1}-{2})", range.TouchedByObject, range.StartOffset, range.EndOffset);
					InvalidateCache(range.TouchedByObject, true);
					touchedRanges.Remove(range);
				}
			}
		}
		
		/// <summary> Add object to cache, optionally adding extra memory tracking </summary>
		public void AddParsedObject(AXmlObject obj, int? maxTouchedLocation)
		{
			AXmlParser.Assert(obj.Length > 0 || obj is AXmlDocument, string.Format("Invalid object {0}.  It has zero length.", obj));
			if (obj is AXmlContainer) {
				int objStartOffset = obj.StartOffset;
				int objEndOffset = obj.EndOffset;
				foreach(AXmlObject child in ((AXmlContainer)obj).Children) {
					AXmlParser.Assert(objStartOffset <= child.StartOffset && child.EndOffset <= objEndOffset, "Wrong nesting");
				}
			}
			parsedItems.Add(obj);
			foreach(SyntaxError syntaxError in obj.MySyntaxErrors) {
				syntaxErrors.Add(syntaxError);
			}
			obj.IsCached = true;
			if (maxTouchedLocation != null) {
				// location is assumed to be read so the range ends at (location + 1)
				// For example eg for "a_" it is (0-2)
				TouchedRange range = new TouchedRange() {
					StartOffset = obj.StartOffset,
					EndOffset = maxTouchedLocation.Value + 1,
					TouchedByObject = obj
				};
				touchedRanges.Add(range);
				AXmlParser.Log("{0} touched range ({1}-{2})", obj, range.StartOffset, range.EndOffset);
			}
		}
		
		List<AXmlObject> FindParents(AXmlObject child)
		{
			List<AXmlObject> parents = new List<AXmlObject>();
			foreach(AXmlObject parent in parsedItems.FindSegmentsContaining(child.StartOffset)) {
				// Parent is anyone wholy containg the child
				if (parent.StartOffset <= child.StartOffset && child.EndOffset <= parent.EndOffset && parent != child) {
					parents.Add(parent);
				}
			}
			return parents;
		}
		
		void InvalidateCache(AXmlObject obj, bool includeParents)
		{
			if (includeParents) {
				List<AXmlObject> parents = FindParents(obj);
				
				foreach(AXmlObject r in parents) {
					if (parsedItems.Remove(r)) {
						foreach(SyntaxError syntaxError in r.MySyntaxErrors) {
							syntaxErrors.Remove(syntaxError);
						}
						r.IsCached = false;
						AXmlParser.Log("Removing cached item {0} (it is parent)", r);
					}
				}
			}
			
			if (parsedItems.Remove(obj)) {
				foreach(SyntaxError syntaxError in obj.MySyntaxErrors) {
					syntaxErrors.Remove(syntaxError);
				}
				obj.IsCached = false;
				AXmlParser.Log("Removed cached item {0}", obj);
			}
		}
		
		public T GetCachedObject<T>(int offset, int lookaheadCount, Predicate<T> conditon) where T: AXmlObject, new()
		{
			AXmlObject obj = parsedItems.FindFirstSegmentWithStartAfter(offset);
			while(obj != null && offset <= obj.StartOffset && obj.StartOffset <= offset + lookaheadCount) {
				if (obj is T && conditon((T)obj)) {
					return (T)obj;
				}
				obj = parsedItems.GetNextSegment(obj);
			}
			return null;
		}
	}
}
