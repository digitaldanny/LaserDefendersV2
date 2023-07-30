using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DFramework 
{
    public static class Util
    {
        /*
         * Return a Random enum value in the "T" enum type.
         * 
         * @return  A random value of the T enum type.
         */
        public static T RandomEnumValue<T>()
        {
            var values = Enum.GetValues(typeof(T));
            int random = UnityEngine.Random.Range(0, values.Length);
            return (T)values.GetValue(random);
        }
    }
}
