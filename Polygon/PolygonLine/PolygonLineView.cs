using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MissionAssistant
{
    partial class PolygonLine : MapLine
    {
        #region Member Functions
        //Constructor
        public PolygonLine(Polygon parent) : base()
        {
            CreateObject(parent);
        }

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);
        }
        public override void Undraw(Canvas canvas)
        {
            base.Undraw(canvas);
        }
        public override bool isDrawn(Canvas canvas)
        {
            return base.isDrawn(canvas);
        }

        //Protected Methods
        protected override void ConstructGeometry()
        {
            base.ConstructGeometry();
            startnode.Tag = new object[] { this, 1 };
            line.Tag = new object[] { this, 0 };
            endnode.Tag = new object[] { this, 2 };
        }

        private void CreateObject(Polygon parent)
        {
            Parent = parent;
        }
        #endregion
    }
}
