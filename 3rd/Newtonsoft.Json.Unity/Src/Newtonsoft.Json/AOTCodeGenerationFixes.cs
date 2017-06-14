using System;
using System.Collections.Generic;

class AOTCodeGenerationFixes
{
    public void UsedOnlyForAOTCodeGeneration()
    {
        // Fix: System.ExecutionEngineException: Attempting to call method 
        // 'System.Collections.Generic.List`1[[System.Single, mscorlib, 
        // Version=2.0.0.0, Culture=, PublicKeyToken=b77a5c561934e089]]::.cctor' 
        // for which no ahead of time (AOT) code was generated.
        new List<Boolean>();
        new List<SByte>();
        new List<Byte>();
        new List<Int16>();
        new List<UInt16>();
        new List<Int32>();
        new List<UInt32>();
        new List<Int64>();
        new List<UInt64>();
        new List<Decimal>();
        new List<Single>();
        new List<Double>();
        new List<DateTime>();
        new List<TimeSpan>();
        new List<DateTimeOffset>();

        throw new InvalidOperationException();
    }

}