using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.CSharp
{
	public enum GeneratedCodeMember
	{
		Unknown,

		StaticFields,
		InstanceFields,
		StaticProperties,
		InstanceProperties,
		Constructors,
		StaticMethods,
		InstanceMethods,
		StaticEvents,
		InstanceEvents,
		NestedTypes
	}

	public class GeneratedCodeSettings
	{
		List<GeneratedCodeMember> codeMemberOrder;

		public List<GeneratedCodeMember> CodeMemberOrder {
			get {
				return codeMemberOrder;
			}
			set {
				codeMemberOrder = value;
			}
		}

		public bool GenerateCategoryComments {
			get;
			set;
		}

		public bool SubOrderAlphabetical {
			get;
			set;
		}

		public void Apply (AstNode rootNode)
		{
			if (rootNode == null)
				throw new ArgumentNullException ("rootNode");
			rootNode.AcceptVisitor (new GenerateCodeVisitior (this));
		}

		public virtual string GetCategoryLabel(GeneratedCodeMember memberCategory)
		{
			switch (memberCategory) {
				case GeneratedCodeMember.StaticFields:
					return "Static Fields";
				case GeneratedCodeMember.InstanceFields:
					return "Fields";
				case GeneratedCodeMember.StaticProperties:
					return "Static Properties";
				case GeneratedCodeMember.InstanceProperties:
					return "Properties";
				case GeneratedCodeMember.Constructors:
					return "Constructors";
				case GeneratedCodeMember.StaticMethods:
					return "Static Methods";
				case GeneratedCodeMember.InstanceMethods:
					return "Methods";
				case GeneratedCodeMember.StaticEvents:
					return "Static Events";
				case GeneratedCodeMember.InstanceEvents:
					return "Events";
				case GeneratedCodeMember.NestedTypes:
					return "Nested Types";
			}
			return null;
		}

		class GenerateCodeVisitior : DepthFirstAstVisitor
		{
			GeneratedCodeSettings settings;

			public GenerateCodeVisitior(GeneratedCodeSettings settings)
			{
				if (settings == null)
					throw new ArgumentNullException("settings");
				this.settings = settings;
			}

			GeneratedCodeMember GetCodeMemberCategory(EntityDeclaration x)
			{
				bool isStatic = x.HasModifier(Modifiers.Static) || x.HasModifier(Modifiers.Const);
				if (x is FieldDeclaration)
					return isStatic ? GeneratedCodeMember.StaticFields : GeneratedCodeMember.InstanceFields;
				if (x is PropertyDeclaration)
					return isStatic ? GeneratedCodeMember.StaticProperties : GeneratedCodeMember.InstanceProperties;
				if (x is ConstructorDeclaration || x is DestructorDeclaration)
					return GeneratedCodeMember.Constructors;
				if (x is MethodDeclaration)
					return isStatic ? GeneratedCodeMember.StaticMethods : GeneratedCodeMember.InstanceMethods;
				if (x is EventDeclaration)
					return isStatic ? GeneratedCodeMember.StaticEvents : GeneratedCodeMember.InstanceEvents;

				if (x is TypeDeclaration)
					return GeneratedCodeMember.NestedTypes;

				return GeneratedCodeMember.Unknown;
			}

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				if (typeDeclaration.ClassType == ClassType.Enum)
					return;
				var entities = new List<EntityDeclaration>(typeDeclaration.Members);
				entities.Sort((x, y) => {
					int i1 = settings.CodeMemberOrder.IndexOf(GetCodeMemberCategory(x));
					int i2 = settings.CodeMemberOrder.IndexOf(GetCodeMemberCategory(y));
					if (i1 != i2)
						return i1.CompareTo(i2);
					if (settings.SubOrderAlphabetical)
						return (x.Name ?? "").CompareTo((y.Name ?? ""));
					return entities.IndexOf(x).CompareTo(entities.IndexOf(y));
				});
				typeDeclaration.Members.Clear();
				typeDeclaration.Members.AddRange(entities);

				if (settings.GenerateCategoryComments) {
					var curCat = GeneratedCodeMember.Unknown;
					foreach (var mem in entities) {
						var cat = GetCodeMemberCategory(mem);
						if (cat == curCat)
							continue;
						curCat = cat;
						var label = settings.GetCategoryLabel(curCat);
						if (string.IsNullOrEmpty(label))
							continue;

						var cmt = new Comment("", CommentType.SingleLine);
						var cmt2 = new Comment(" " + label, CommentType.SingleLine);
						var cmt3 = new Comment("", CommentType.SingleLine);
						mem.Parent.InsertChildsBefore(mem, Roles.Comment, cmt, cmt2, cmt3);
						mem.Parent.InsertChildBefore(cmt, new UnixNewLine(), Roles.NewLine);
						mem.Parent.InsertChildAfter(cmt3, new UnixNewLine(), Roles.NewLine);
					}
				}
			}
		}

		static Lazy<GeneratedCodeSettings> defaultSettings = new Lazy<GeneratedCodeSettings>(
			() => new GeneratedCodeSettings() {
				CodeMemberOrder = new List<GeneratedCodeMember>() {
					GeneratedCodeMember.StaticFields,
					GeneratedCodeMember.InstanceFields,
					GeneratedCodeMember.StaticProperties,
					GeneratedCodeMember.InstanceProperties,
					GeneratedCodeMember.Constructors,
					GeneratedCodeMember.StaticMethods,
					GeneratedCodeMember.InstanceMethods,
					GeneratedCodeMember.StaticEvents,
					GeneratedCodeMember.InstanceEvents,
					GeneratedCodeMember.NestedTypes
				},
				GenerateCategoryComments = true,
				SubOrderAlphabetical = true
		});

		public static GeneratedCodeSettings Default {
			get {
				return defaultSettings.Value;
			}
		}
	}
}