using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatAI
{
    public struct FuzzyValue
    {
        public float Value;
        public FuzzyResult result;
    }

    public struct FuzzyRule
    {
        public FuzzyValue value1;
        public FuzzyValue value2;
        public Compare comparison;
    }

    public enum Compare
    {
        Greater,
        GreaterorEqual,
        Equal,
        LessorEqual,
        Less,
    }

    public class FuzzyClasses
    {
        //public static FuzzyResult CompareRules(FuzzyRule[] rules, float randomRange = 10)
        //{
        //    if (rules == null)
        //    {
        //        return FuzzyResult.VeryUndesirable;
        //    }

        //}
    }
}
