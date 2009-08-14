// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.XmlParser
{
	class TagMatchingHeuristics
	{
		const int maxConfigurationCount = 10;
		
		XmlParser parser;
		Cache cache;
		string input;
		List<RawObject> tags;
		
		public TagMatchingHeuristics(XmlParser parser, string input, List<RawObject> tags)
		{
			this.parser = parser;
			this.cache = parser.Cache;
			this.input = input;
			this.tags = tags;
		}
		
		public RawDocument ReadDocument()
		{
			RawDocument doc = new RawDocument() { Parser = parser };
			
			XmlParser.Log("Flat stream: {0}", PrintObjects(tags));
			List<RawObject> valid = MatchTags(tags);
			XmlParser.Log("Fixed stream: {0}", PrintObjects(valid));
			IEnumerator<RawObject> validStream = valid.GetEnumerator();
			validStream.MoveNext(); // Move to first
			while(true) {
				// End of stream?
				try {
					if (validStream.Current == null) break;
				} catch (InvalidCastException) {
					break;
				}
				doc.AddChild(ReadTextOrElement(validStream));
			}
			
			if (doc.Children.Count > 0) {
				doc.StartOffset = doc.FirstChild.StartOffset;
				doc.EndOffset = doc.LastChild.EndOffset;
			}
			
			XmlParser.Log("Constructed {0}", doc);
			cache.Add(doc, null);
			return doc;
		}
		
		RawObject ReadSingleObject(IEnumerator<RawObject> objStream)
		{
			RawObject obj = objStream.Current;
			objStream.MoveNext();
			return obj;
		}
		
		RawObject ReadTextOrElement(IEnumerator<RawObject> objStream)
		{
			RawObject curr = objStream.Current;
			if (curr is RawText || curr is RawElement) {
				return ReadSingleObject(objStream);
			} else {
				RawTag currTag = (RawTag)curr;
				if (currTag == StartTagPlaceholder) {
					return ReadElement(objStream);
				} else if (currTag.IsStartOrEmptyTag) {
					return ReadElement(objStream);
				} else {
					return ReadSingleObject(objStream);
				}
			}
		}
		
		RawElement ReadElement(IEnumerator<RawObject> objStream)
		{
			RawElement element = new RawElement();
			element.IsProperlyNested = true;
			
			// Read start tag
			RawTag startTag = ReadSingleObject(objStream) as RawTag;
			XmlParser.DebugAssert(startTag != null, "Start tag expected");
			XmlParser.DebugAssert(startTag.IsStartOrEmptyTag || startTag == StartTagPlaceholder, "Start tag expected");
			if (startTag == StartTagPlaceholder) {
				element.HasStartOrEmptyTag = false;
				element.IsProperlyNested = false;
				TagReader.OnSyntaxError(element, objStream.Current.StartOffset, objStream.Current.EndOffset,
				                        "Matching openning tag was not found");
			} else {
				element.HasStartOrEmptyTag = true;
				element.AddChild(startTag);
			}
			
			// Read content and end tag
			if (element.StartTag.IsStartTag || startTag == StartTagPlaceholder) {
				while(true) {
					RawTag currTag = objStream.Current as RawTag; // Peek
					if (currTag == EndTagPlaceholder) {
						TagReader.OnSyntaxError(element, element.LastChild.EndOffset, element.LastChild.EndOffset,
						                        "Expected '</{0}>'", element.StartTag.Name);
						ReadSingleObject(objStream);
						element.HasEndTag = false;
						element.IsProperlyNested = false;
						break;
					} else if (currTag != null && currTag.IsEndTag) {
						if (currTag.Name != element.StartTag.Name) {
							TagReader.OnSyntaxError(element, currTag.StartOffset + 2, currTag.StartOffset + 2 + currTag.Name.Length,
							                        "Expected '{0}'.  End tag must have same name as start tag.", element.StartTag.Name);
						}
						element.AddChild(ReadSingleObject(objStream));
						element.HasEndTag = true;
						break;
					}
					RawObject nested = ReadTextOrElement(objStream);
					if (nested is RawElement) {
						if (!((RawElement)nested).IsProperlyNested)
							element.IsProperlyNested = false;
						element.AddChildren(Split((RawElement)nested).ToList());
					} else {
						element.AddChild(nested);
					}
				}
			} else {
				element.HasEndTag = false;
			}
			
			element.StartOffset = element.FirstChild.StartOffset;
			element.EndOffset = element.LastChild.EndOffset;
			
			XmlParser.Log("Constructed {0}", element);
			cache.Add(element, null); // Need all elements in cache for offset tracking
			return element;
		}
		
		IEnumerable<RawObject> Split(RawElement elem)
		{
			int myIndention = GetIndentLevel(elem);
			// If has virtual end and is indented
			if (!elem.HasEndTag && myIndention != -1) {
				int lastAccepted = 0; // Accept start tag
				while (lastAccepted + 1 < elem.Children.Count - 1 /* no end tag */) {
					RawObject nextItem = elem.Children[lastAccepted + 1];
					if (nextItem is RawText) {
						lastAccepted++; continue;  // Accept
					} else {
						// Include all more indented items
						if (GetIndentLevel(nextItem) > myIndention) {
							lastAccepted++; continue;  // Accept
						} else {
							break;  // Reject
						}
					}
				}
				// Accepted everything?
				if (lastAccepted + 1 == elem.Children.Count - 1) {
					yield return elem;
					yield break;
				}
				XmlParser.Log("Splitting {0} - take {1} of {2} nested", elem, lastAccepted, elem.Children.Count - 2);
				RawElement topHalf = new RawElement();
				topHalf.HasStartOrEmptyTag = elem.HasStartOrEmptyTag;
				topHalf.HasEndTag = elem.HasEndTag;
				topHalf.AddChildren(elem.Children.Take(lastAccepted + 1));    // Start tag + nested
				topHalf.StartOffset = topHalf.FirstChild.StartOffset;
				topHalf.EndOffset = topHalf.LastChild.EndOffset;
				TagReader.OnSyntaxError(topHalf, topHalf.LastChild.EndOffset, topHalf.LastChild.EndOffset,
						                 "Expected '</{0}>'", topHalf.StartTag.Name);
				
				XmlParser.Log("Constructed {0}", topHalf);
				cache.Add(topHalf, null);
				yield return topHalf;
				for(int i = lastAccepted + 1; i < elem.Children.Count - 1; i++) {
					yield return elem.Children[i];
				}
			} else {
				yield return elem;
			}
		}
		
		int GetIndentLevel(RawObject obj)
		{
			int offset = obj.StartOffset - 1;
			int level = 0;
			while(true) {
				if (offset < 0) break;
				char c = input[offset];
				if (c == ' ') {
					level++;
				} else if (c == '\t') {
					level += 4;
				} else if (c == '\r' || c == '\n') {
					break;
				} else {
					return -1;
				}
				offset--;
			}
			return level;
		}
		
		/// <summary>
		/// Stack of still unmatched start tags.
		/// It includes the cost and backtack information.
		/// </summary>
		class Configuration
		{
			/// <summary> Unmatched start tags </summary>
			public ImmutableStack<RawTag> StartTags { get; set; }
			/// <summary> Properly nested tags </summary>
			public ImmutableStack<RawObject> Document { get; set; }
			/// <summary> Number of needed modificaitons to the document </summary>
			public int Cost { get; set; }
		}
		
		/// <summary>
		/// Dictionary which stores the cheapest configuration
		/// </summary>
		class Configurations: Dictionary<ImmutableStack<RawTag>, Configuration>
		{
			public Configurations()
			{
			}
			
			public Configurations(IEnumerable<Configuration> configs)
			{
				foreach(Configuration config in configs) {
					this.Add(config);
				}
			}
			
			/// <summary> Overwrite only if cheaper </summary>
			public void Add(Configuration newConfig)
			{
				Configuration oldConfig;
				if (this.TryGetValue(newConfig.StartTags, out oldConfig)) {
					if (newConfig.Cost < oldConfig.Cost) {
						this[newConfig.StartTags] = newConfig;
					}
				} else {
					base.Add(newConfig.StartTags, newConfig);
				}
			}
			
			public override string ToString()
			{
				StringBuilder sb = new StringBuilder();
				foreach(var kvp in this) {
					sb.Append("\n - '");
					foreach(RawTag startTag in kvp.Value.StartTags.Reverse()) {
						sb.Append('<');
						sb.Append(startTag.Name);
						sb.Append('>');
					}
					sb.AppendFormat("' = {0}", kvp.Value.Cost);
				}
				return sb.ToString();
			}
		}
		
		// Tags used to guide the element creation
		readonly RawTag StartTagPlaceholder = new RawTag();
		readonly RawTag EndTagPlaceholder = new RawTag();
		
		/// <summary>
		/// Add start or end tag placeholders so that the documment is properly nested
		/// </summary>
		List<RawObject> MatchTags(IEnumerable<RawObject> objs)
		{
			Configurations configurations = new Configurations();
			configurations.Add(new Configuration {
				StartTags = ImmutableStack<RawTag>.Empty,
				Document = ImmutableStack<RawObject>.Empty,
				Cost = 0,
			});
			foreach(RawObject obj in objs) {
				configurations = ProcessObject(configurations, obj);
			}
			// Close any remaining start tags
			foreach(Configuration conifg in configurations.Values) {
				while(!conifg.StartTags.IsEmpty) {
					conifg.StartTags = conifg.StartTags.Pop();
					conifg.Document = conifg.Document.Push(EndTagPlaceholder);
					conifg.Cost += 1;
				}
			}
			XmlParser.Log("Configurations after closing all remaining tags:" + configurations.ToString());
			Configuration bestConfig = configurations.Values.OrderBy(v => v.Cost).First();
			XmlParser.Log("Best configuration has cost {0}", bestConfig.Cost);
			
			return bestConfig.Document.Reverse().ToList();
		}
		
		/// <summary> Get posible configurations after considering fiven object </summary>
		Configurations ProcessObject(Configurations oldConfigs, RawObject obj)
		{
			XmlParser.Log("Processing {0}", obj);
			
			RawTag tag = obj as RawTag;
			XmlParser.Assert(obj is RawTag || obj is RawText || obj is RawElement, obj.GetType().Name + " not expected");
			if (obj is RawElement)
				XmlParser.Assert(((RawElement)obj).IsProperlyNested, "Element not proprly nested");
			
			Configurations newConfigs = new Configurations();
			
			foreach(var kvp in oldConfigs) {
				Configuration oldConfig = kvp.Value;
				var oldStartTags = oldConfig.StartTags;
				var oldDocument = oldConfig.Document;
				int oldCost = oldConfig.Cost;
				
				if (tag != null && tag.IsStartTag) {
					newConfigs.Add(new Configuration {                    // Push start-tag (cost 0)
						StartTags = oldStartTags.Push(tag),
						Document = oldDocument.Push(tag),
						Cost = oldCost,
					});
				} else if (tag != null && tag.IsEndTag) {
					newConfigs.Add(new Configuration {                    // Ignore (cost 1)
						StartTags = oldStartTags,
						Document = oldDocument.Push(StartTagPlaceholder).Push(tag),
						Cost = oldCost + 1,
	               });
					if (!oldStartTags.IsEmpty && oldStartTags.Peek().Name != tag.Name) {
						newConfigs.Add(new Configuration {                // Pop 1 item (cost 1) - not mathcing
							StartTags = oldStartTags.Pop(),
							Document = oldDocument.Push(tag),
							Cost = oldCost + 1,
		               });
					}
					int popedCount = 0;
					var startTags = oldStartTags;
					var doc = oldDocument;
					foreach(RawTag poped in oldStartTags) {
						popedCount++;
						if (poped.Name == tag.Name) {
							newConfigs.Add(new Configuration {             // Pop 'x' items (cost x-1) - last one is matching
								StartTags = startTags.Pop(),
								Document = doc.Push(tag),
								Cost = oldCost + popedCount - 1,
							});
						}
						startTags = startTags.Pop();
						doc = doc.Push(EndTagPlaceholder);
					}
				} else {
					// Empty tag  or  other tag type  or  text  or  properly nested element
					newConfigs.Add(new Configuration {                    // Ignore (cost 0)
						StartTags = oldStartTags,
						Document = oldDocument.Push(obj),
						Cost = oldCost,
	               });
				}
			}
			
			// Log("New configurations:" + newConfigs.ToString());
			
			Configurations bestNewConfigurations = new Configurations(
				newConfigs.Values.OrderBy(v => v.Cost).Take(maxConfigurationCount)
			);
			
			XmlParser.Log("Best new configurations:" + bestNewConfigurations.ToString());
			
			return bestNewConfigurations;
		}
		
		#region Helper methods
		
		string PrintObjects(IEnumerable<RawObject> objs)
		{
			StringBuilder sb = new StringBuilder();
			foreach(RawObject obj in objs) {
				if (obj is RawTag) {
					if (obj == StartTagPlaceholder) {
						sb.Append("#StartTag#");
					} else if (obj == EndTagPlaceholder) {
						sb.Append("#EndTag#");
					} else {
						sb.Append(((RawTag)obj).OpeningBracket);
						sb.Append(((RawTag)obj).Name);
						sb.Append(((RawTag)obj).ClosingBracket);
					}
				} else if (obj is RawElement) {
					sb.Append('[');
					sb.Append(PrintObjects(((RawElement)obj).Children));
					sb.Append(']');
				} else if (obj is RawText) {
					sb.Append('~');
				} else {
					throw new Exception("Should not be here: " + obj);
				}
			}
			return sb.ToString();
		}
		
		#endregion
	}
}
