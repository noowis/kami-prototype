using System;
using System.Collections.Generic;

namespace kamiprototype.Scripts.GameplayClasses;

public class Sequence
{
    private List<float> _timing;
    private List<char> _sequence;

    public Sequence(List<float> timing, List<char> sequence)
    {
        if(timing.Count != sequence.Count)
            throw new InvalidOperationException("Timing and sequence lengths do not match");
        _timing = timing;
        _sequence = sequence;
    }
}