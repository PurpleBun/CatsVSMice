using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatAI
{
    public struct FuzzyValue
    {
        public float value;
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
        public static FuzzyResult CompareRules(FuzzyRule[] rules, float randomRange = 1)
        {
            if(rules.Length == 0)
            {
                return FuzzyResult.VeryUndesirable;
            }
            int value = 0;
            for(int i = 0; i < rules.Length; i++)
            {
                value += (int)FuzzyCompare(rules[i].value1, rules[i].value2, rules[i].comparison);
            }

            value = Mathf.RoundToInt(value / rules.Length);

            return (FuzzyResult)value;
        }

        static FuzzyResult FuzzyCompare(FuzzyValue value1, FuzzyValue value2, Compare comparison)
        {
            FuzzyResult result = FuzzyResult.VeryUndesirable;

            switch (comparison)
            {
                case Compare.Greater:
                    result = (value1.value > value2.value) ? value1.result : value2.result;
                    break;
                case Compare.GreaterorEqual:
                    result = (value1.value >= value2.value) ? value1.result : value2.result;
                    break;
                case Compare.Equal:
                    result = (value1.value == value2.value) ? value1.result : value2.result;
                    break;
                case Compare.LessorEqual:
                    result = (value1.value <= value2.value) ? value1.result : value2.result;
                    break;
                case Compare.Less:
                    result = (value1.value < value2.value) ? value1.result : value2.result;
                    break;
                default:
                    result = FuzzyResult.VeryUndesirable;
                    break;
            }
            Debug.Log(result);
            return result;
        }
    }
}
