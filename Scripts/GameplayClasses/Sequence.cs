using System;
using System.Collections.Generic;

namespace kamiprototype.Scripts.GameplayClasses;

public class Sequence
{
    public List<double> _timing;
    public List<char> Button;

    public Sequence(List<double> timing, List<char> sequence)
    {
        if(timing.Count != sequence.Count)
            throw new InvalidOperationException("Timing and sequence lengths do not match");
        _timing = timing;
        Button = sequence;
    }
}