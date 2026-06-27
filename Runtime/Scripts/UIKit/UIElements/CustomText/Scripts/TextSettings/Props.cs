using System;
using UnityEngine;

namespace CustomText
{
    [Serializable]
    public class Props
    {
        [SerializeField] public Custom_TextStyle _type = Custom_TextStyle.Default;
        [SerializeField] public Custom_ColorStyle _color = Custom_ColorStyle.Default;
        [SerializeField] public Custom_MaterialStyle _material = Custom_MaterialStyle.Default;
    }
}
