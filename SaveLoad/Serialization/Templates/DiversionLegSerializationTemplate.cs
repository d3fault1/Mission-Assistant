using System;
using System.Collections.Generic;

namespace MissionAssistant
{
    [Serializable]
    class DiversionLegSerializationTemplate : MapLineSerializationTemplate
    {
        //View Fields
        public double ContentOffset;
        public DiversionContentOrientation ContentOrientation;
        public bool ContentFlip;
        public int LinkingLegIndex;
        public int LinkingNode;

        //Data Fields
        public string Name;
        public double Altitude;
        public double Speed;

        //Member Fields
        public List<CheckpointSerializationTemplate> Checkpoints;

        public DiversionLegSerializationTemplate()
        {
            Checkpoints = new List<CheckpointSerializationTemplate>();
        }
    }
}
