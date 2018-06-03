﻿using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzerTests.NonVirtualSetupAnalyzerTests
{
    public abstract class NonVirtualSetupDiagnosticVerifier : VisualBasicDiagnosticVerifier, INonVirtualSetupDiagnosticVerifier
    {
        [Fact]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod();

        [Theory]
        [InlineData("1", "Integer")]
        [InlineData(@"""c""C", "Char")]
        [InlineData("true", "Boolean")]
        [InlineData("false", "Boolean")]
        [InlineData(@"""1""", "String")]
        public abstract Task ReportsDiagnostics_WhenSettingValueForLiteral(string literal, string type);

        [Fact]
        public abstract Task ReportsDiagnostics_WhenSettingValueForStaticMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForDelegate();

        [Fact]
        public abstract Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty();

        [Fact]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualIndexer();

        [Fact]
        public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod();

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new NonVirtualSetupAnalyzer();
        }
    }
}