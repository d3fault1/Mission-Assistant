using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionAssistant
{
    class ImageData
    {
        public int imgID;
        public PointLatLng position;
        public bool isDraggable;

        public ImageData(int id, PointLatLng pos, bool drag)
        {
            imgID = id;
            position = pos;
            isDraggable = drag;
        }
    }
}
