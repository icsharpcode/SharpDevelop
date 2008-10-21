using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows;

namespace ICSharpCode.Xaml
{
	class InstanceTracker : Tracker
	{
		public override void Add(MemberNode memberNode)
		{
			foreach (var valueNode in memberNode.Values) {
				Add(valueNode);
			}
		}

		public override void Add(ObjectNode objectNode)
		{
			if (objectNode.Instance == null) {
				Instantiate(objectNode);
			}
			var parentMember = objectNode.ParentMember;
			if (parentMember != null) {
				if (parentMember.Member == IntristicMember.Items) {
					var collectionObjectNode = parentMember.ParentObject;
					if (collectionObjectNode.Type.IsDictionary) {
						//var keyValue = objectNode.Key.Value;
						//if (keyValue != null) {
						//    Runtime.Add(collectionObjectNode.Instance,
						//        keyValue.Instance, objectNode.Instance);
						//}
					}
					else {
						Runtime.Add(collectionObjectNode.Instance, objectNode.Instance);
					}
				}
				else {
					if (!objectNode.IsRetrieved) {
						object valueInstance;
						if (objectNode.Type.IsMarkupExtension) {
							valueInstance = ProvideValue(objectNode);
						}
						else {
							valueInstance = objectNode.Instance;
						}
						Runtime.SetValue(parentMember, valueInstance);
					}
				}
			}
		}

		public override void Add(TextNode textNode)
		{
			var parentMember = textNode.ParentMember;
			if (parentMember != null) {
				textNode.Instance = Runtime.ConvertFromText(
					parentMember.Property.XamlContext,
					textNode.Text);

				if (parentMember.Member == IntristicMember.Items) {
					// IAddChild support.
					// Instance could differ without it (e.g. TextBlock.Inlines vs TextBlock.Text)
					var grandParentMember = parentMember.ParentObject.ParentMember;
					if (grandParentMember != null) {
						var addChild = grandParentMember.ParentObject.Instance as IAddChild;
						if (addChild != null) {
							addChild.AddText(textNode.Text);
							return;
						}
					}

					Runtime.Add(parentMember.ParentObject.Instance, textNode.Instance);
				}
				else {
					Runtime.SetValue(parentMember, textNode.Instance);
					if (parentMember.Member == Directive.Key) {
						Runtime.Add(parentMember.ParentObject.ParentObject.Instance,
							textNode.Instance, parentMember.ParentObject.Instance);
					}
				}
			}
		}

		public override void Remove(MemberNode node, ObjectNode parent)
		{
			Runtime.ResetValue(parent.Instance, node.Member);
		}

		public override void Remove(ObjectNode node, MemberNode parent)
		{
			if (parent != null && parent.Member == IntristicMember.Items) {
				Runtime.Remove(parent.ParentObject.Instance, node.Instance);
			}
		}

		void Instantiate(ObjectNode objectNode)
		{
			var parentMember = objectNode.ParentMember;
			ISupportInitialize supportInitialize = null;

			if (objectNode.IsRetrieved) {
				objectNode.Instance = Runtime.GetValue(parentMember);
			}
			else {
				if (objectNode.InitializationText.IsSet) {
					objectNode.Instance = Runtime.ConvertFromText(
						objectNode.InitializationText.XamlContext,
						objectNode.InitializationText.ValueText);
				}
				else {
					var ctorNode = objectNode.FindMemberNode(IntristicMember.ConsructorArgs);
					if (ctorNode != null) {
						objectNode.Instance = Construct(ctorNode);
					}
					else {
						objectNode.Instance = Runtime.CreateInstance(objectNode.Type, null);
					}
				}

				supportInitialize = objectNode.Instance as ISupportInitialize;
				if (supportInitialize != null) {
					supportInitialize.BeginInit();
				}
			}

			foreach (var memberNode in objectNode.MemberNodes) {
				if (memberNode.Member == IntristicMember.ConsructorArgs) continue;
				Add(memberNode);
			}

			if (supportInitialize != null) {
				supportInitialize.EndInit();
			}
		}

		object Construct(MemberNode ctorNode)
		{
			var objectNode = ctorNode.ParentObject;
			var args = new List<object>();
			var ctor = objectNode.Type.Constructors.First(
				c => c.Arguments.Count() == ctorNode.Values.Count);

			for (int i = 0; i < ctorNode.Values.Count; i++) {
				var ctorArgValue = ctorNode.Values[i];
				Add(ctorArgValue);

				var value = ctorArgValue.Instance;
				var targetType = ctor.Arguments[i];
				if (!targetType.SystemType.IsAssignableFrom(value.GetType())) {
					var text = value as string;
					if (text != null) {
						value = Runtime.ConvertFromText(ctorNode.Property.XamlContext, targetType, text);
					}
					else {
						throw new XamlException("Cannot convert");
					}
				}
				args.Add(value);
			}
			return Runtime.CreateInstance(objectNode.Type, args.ToArray());
		}

		object ProvideValue(ObjectNode node)
		{
			var me = node.Instance as MarkupExtension;

			Instantiate(node);

			if (me is StaticResourceExtension) {
				return FindResource(node, (me as StaticResourceExtension).ResourceKey);
			}
			else if (me is DynamicResourceExtension) {
				return FindResource(node, (me as DynamicResourceExtension).ResourceKey);
			}

			return me.ProvideValue(node.ParentMember.Property.XamlContext);
		}

		object FindResource(ObjectNode node, object key)
		{
			if (key == null) return null;

			var current = node;
			while (current != null) {
				var result = GetResource(current.Instance, key);
				if (result != Runtime.UnsetValue) {
					return result;
				}
				current = current.ParentObject;
			}

			var appDefinition = node.Document.Project.ApplicationDefinition;
			if (appDefinition != null) {
				var app = appDefinition.Root.Instance as Application;
				if (app != null) {
					var result = GetResource(app, key);
					if (result != Runtime.UnsetValue) {
						return result;
					}
				}
			}
			//if (key is ComponentResourceKey)
			//{
			//    fore
			//}
			return null;
		}

		public static object GetResource(object container, object key)
		{
			var resources = GetResources(container);
			if (resources != null && resources.Contains(key)) {
				return resources[key];
			}
			return Runtime.UnsetValue;
		}

		public static ResourceDictionary GetResources(object container)
		{
			if (container is FrameworkElement) {
				return (container as FrameworkElement).Resources;
			}
			if (container is FrameworkTemplate) {
				return (container as FrameworkTemplate).Resources;
			}
			if (container is Style) {
				return (container as Style).Resources;
			}
			if (container is Application) {
				return (container as Application).Resources;
			}
			return null;
		}
	}
}
