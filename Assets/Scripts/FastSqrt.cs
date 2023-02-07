using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastSqrt
{
    private Dictionary<float, float> dict;

    public FastSqrt()
    {
        dict = new Dictionary<float, float>();
    }

    public void Clear()
    {
        dict.Clear();
    }

    public float Sqrt(float val)
    {
        if (dict.ContainsKey(val))
            return dict[val];
        else
        {
            var ret = Mathf.Sqrt(val);
            dict[val] = ret;
            return ret;
        }
    }
}
