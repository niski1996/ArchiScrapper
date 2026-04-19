using System.Reflection;
using ArchiScrapper.Messaging.Abstractions;
using Xunit;

namespace ArchiScrapper.CommonMessaging.CompatibilityTests;

public class PublicApiCompatibilityTests
{
    [Fact]
    public void RawEventProcessingFlowBuilderContractContainsExpectedMethods()
    {
        var methods = typeof(IRawEventProcessingFlowBuilder<>).GetMethods(BindingFlags.Public | BindingFlags.Instance);

        Assert.Contains(methods, method => method.Name == "UseMaterializer");
        Assert.Contains(methods, method => method.Name == "UseInfrastructureStep");
        Assert.Contains(methods, method => method.Name == "UseBusinessStep");
        Assert.Contains(methods, method => method.Name == "UseHandler");
        Assert.Contains(methods, method => method.Name == "Build");
    }

    [Fact]
    public void HandlingPipelineBuilderContractContainsExpectedMethods()
    {
        var methods = typeof(IHandlingPipelineBuilder<>).GetMethods(BindingFlags.Public | BindingFlags.Instance);

        Assert.Contains(methods, method => method.Name == "UseInfrastructureStep");
        Assert.Contains(methods, method => method.Name == "UseBusinessStep");
        Assert.Contains(methods, method => method.Name == "UseHandler");
        Assert.Contains(methods, method => method.Name == "Build");
    }
}
