﻿using System;
using Unity.Entities;

namespace DOTS_MLAgents.Example.SpaceMagic.Scripts
{
    /// <summary>
    /// This IShareComponentData will be used to assign each sphere in a different group that will
    /// use a different IAgentSystem for its decision making.
    /// </summary>
    [Serializable]
    public struct SphereGroup : ISharedComponentData
    {
        public int Group;
    }
    
}