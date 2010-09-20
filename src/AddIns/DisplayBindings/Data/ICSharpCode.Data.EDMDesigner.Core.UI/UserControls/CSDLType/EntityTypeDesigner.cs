// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.UI.Converters;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.CSDLType
{
    public class EntityTypeDesigner : TypeBaseDesigner
    {
        public EntityTypeDesigner(UIEntityType entityType)
            : base(entityType)
        {
            EntityType = entityType;
            entityType.BaseTypeChanged +=
                () =>
                {
                    if (EntityType.BusinessInstance.BaseType == null)
                        DeleteBaseInheritanceRelation();
                    else if (_baseRelationContener == null)
                        DrawBaseInheritanceRelation();
                };
            entityTypeExpander.SetBinding(OpacityProperty, new Binding("IsCompletlyMapped") { Source = ((EntityType)UIType.BusinessInstance).Mapping, Converter = new BoolToOpacityConverter(), ConverterParameter = 0.75 });
        }

        public UIEntityType EntityType { get; private set; }

        private RelationContener _baseRelationContener;

        protected internal override void DrawRelations()
        {
            base.DrawRelations();
            DrawBaseInheritanceRelation();
            foreach (var entityTypeDesignerChild in Designer.Children.OfType<EntityTypeDesigner>().Where(etd => etd.EntityType.BusinessInstance.BaseType == EntityType.BusinessInstance).ToList())
                DrawInheritanceRelation(this, entityTypeDesignerChild);
        }

        private void DrawBaseInheritanceRelation()
        {
            var baseEntityTypeDesigner = Designer.Children.OfType<EntityTypeDesigner>().FirstOrDefault(etd => etd.EntityType.BusinessInstance == EntityType.BusinessInstance.BaseType);
            if (baseEntityTypeDesigner == null)
                _baseRelationContener = null;
            else
                _baseRelationContener = DrawInheritanceRelation(baseEntityTypeDesigner, this);
        }

        private RelationContener DrawInheritanceRelation(EntityTypeDesigner baseTypeDesigner, EntityTypeDesigner childTypeDesigner)
        {
            var relationContener = (from rc in Designer.Children.OfType<RelationContener>()
                                   let ir = rc.Content as InheritanceRelation
                                   where ir != null && ir.FromTypeDesigner == childTypeDesigner && ir.ToTypeDesigner == baseTypeDesigner
                                   select rc).FirstOrDefault();
            if (relationContener != null)
                return relationContener;
            var inheritanceRelation = new InheritanceRelation(Designer, childTypeDesigner, baseTypeDesigner, () => Designer.Children.OfType<EntityTypeDesigner>().FirstOrDefault(etd => etd.EntityType.BusinessInstance == EntityType.BusinessInstance.BaseType));
            relationContener = new RelationContener(inheritanceRelation);
            Designer.Children.Add(relationContener);
            relationContener.SetBinding(Canvas.LeftProperty, new Binding { Source = inheritanceRelation, Path = new PropertyPath("CanvasLeft") });
            relationContener.SetBinding(Canvas.TopProperty, new Binding { Source = inheritanceRelation, Path = new PropertyPath("CanvasTop") });
            baseTypeDesigner.AddRelationContenerWithoutRelatedProperty(relationContener);
            childTypeDesigner.AddRelationContenerWithoutRelatedProperty(relationContener);
            return relationContener;
        }

        public void DeleteBaseInheritanceRelation()
        {
            if (_baseRelationContener != null)
            {
                Designer.Children.Remove(_baseRelationContener);
                _baseRelationContener = null;
            }
        }
    }
}
