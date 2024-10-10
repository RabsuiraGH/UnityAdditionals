using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.Utility
{
    [Serializable]
    public class SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        [JsonIgnore]
        public Vector3 UnityVector
        {
            get
            {
                return new Vector3(x, y, z);
            }
        }

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public static List<SerializableVector3> GetSerializableList(List<Vector3> vList)
        {
            List<SerializableVector3> list = new List<SerializableVector3>(vList.Count);
            for (int i = 0; i < vList.Count; i++)
            {
                list.Add(new SerializableVector3(vList[i]));
            }
            return list;
        }

        public static List<Vector3> GetSerializableList(List<SerializableVector3> vList)
        {
            List<Vector3> list = new List<Vector3>(vList.Count);
            for (int i = 0; i < vList.Count; i++)
            {
                list.Add(vList[i].UnityVector);
            }
            return list;
        }
    }
}