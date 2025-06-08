using System;
using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace PastryTycoon.ServiceDefaults;

public class IgnoreGrainTimerSampler : Sampler
{
    public override SamplingResult ShouldSample(in SamplingParameters samplingParameters)
    {
        if (samplingParameters.Name == "IGrainTimerInvoker/InvokeCallbackAsync")
        {
            return new SamplingResult(SamplingDecision.Drop);
        }
        return new SamplingResult(SamplingDecision.RecordAndSample);
    }

    //public override string Description => "Ignores Orleans Grain Timer Invoker spans";
}
