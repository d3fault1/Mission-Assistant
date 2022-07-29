using System;
using System.Collections.Generic;

namespace MissionAssistant
{
    [Serializable]
    class RouteLegSerializationTemplate : MapLineSerializationTemplate
    {
        //View Fields
        public double PanelOffsetX;
        public double PanelOffsetY;
        public bool IsFlipped;

        //Data Fields
        public string Name;
        public RouteLegType Type;
        public double Altitude;
        public double Speed;
        public double FuelAdjustment;

        //Member Fields
        public List<CheckpointSerializationTemplate> Checkpoints;

        public RouteLegSerializationTemplate()
        {
            Checkpoints = new List<CheckpointSerializationTemplate>();
        }
    }
}
