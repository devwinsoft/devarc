using System;

[AttributeUsage(System.AttributeTargets.Field)]
class PropertyAttribute : System.Attribute
{
    public int Length = 0;
}
