﻿using System.Threading.Tasks;
using Xunit;

namespace NSubstitute.Analyzers.Test
{
    public abstract class SubstituteAnalyzerTestBase : AnalyzerTest
    {
        [Fact]
        public abstract Task ReturnsDiagnostic_WhenUsedForClassWithoutPublicOrProtectedConstructor();

        [Fact]
        public abstract Task ReturnsDiagnostic_WhenPassedParametersCount_GreaterThanCtorParametersCount();

        [Fact]
        public abstract Task ReturnsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount();

        // even though parameters are optional, NSubstitute requires to pass all of them
        [Fact]
        public abstract Task ReturnsDiagnostic_WhenUsedWithWithoutProvidingOptionalParameters();

        [Fact]
        public abstract Task ReturnsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToNotApplied();

        [Fact]
        public abstract Task ReturnsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToDynamicProxyGenAssembly2();

        [Fact]
        public abstract Task ReturnsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToWrongAssembly();

        [Fact]
        public abstract Task ReturnsDiagnostic_WhenCorrespondingConstructorArgumentsNotCompatible();

        [Theory]
        public abstract Task ReturnsDiagnostic_WhenConstructorArgumentsRequireExplicitConversion(string ctorValues, string invocationValues);

        [Theory]
        public abstract Task ReturnsNoDiagnostic_WhenConstructorArgumentsAreImplicitlyConvertible(string ctorValues, string invocationValues);
    }
}