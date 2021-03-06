﻿using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonVirtualSetupAnalyzerTests
{
    public class ReturnsNullAsOrdinaryMethodWithGenericTypeSpecifiedTests : NonVirtualSetupDiagnosticVerifier
    {
        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod()
        {
            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public object Bar()
        {
            return new object();
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar());
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(19, 51)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForLiteral(string literal, string type)
        {
            await Task.CompletedTask;
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForStaticMethod()
        {
            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public static object Bar()
        {
            return new object();
        }
    }

    public class FooTests
    {
        public void Test()
        {
            ReturnsExtensions.ReturnsNull<object>(Foo.Bar());
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(18, 51)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod()
        {
            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public virtual object Bar()
        {
            return new object();
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar());
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod()
        {
            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public virtual object Bar()
        {
            return new object();
        }
    }

    public class Foo2 : Foo
    {
        public override object Bar() => null;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo2>();
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar());
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired()
        {
            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public virtual object Bar()
        {
            return new object();
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            var returnValue = substitute.Bar();
            ReturnsExtensions.ReturnsNull<object>(returnValue);
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForDelegate()
        {
            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System;

namespace MyNamespace
{
    public class FooTests
    {
        public void Test()
        {
            var substitute = Substitute.For<Func<object>>();
            ReturnsExtensions.ReturnsNull<object>(substitute());
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod()
        {
            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public virtual object Bar()
        {
            return new object();
        }
    }

    public class Foo2 : Foo
    {
        public sealed override object Bar() => new object();
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo2>();
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar());
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(24, 51)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod()
        {
            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public abstract class Foo
    {
        public abstract object Bar();
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar());
        }
    }
}";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod()
        {
            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public interface IFoo
    {
        object Bar();
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar());
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty()
        {
            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public interface IFoo
    {
        object Bar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar);
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod()
        {
            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
   public interface IFoo<T>
    {
        object Bar<T>();
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo<int>>();
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar<int>());
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty()
        {
            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public abstract class Foo
    {
        public abstract object Bar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar);
        }
    }
}";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer()
        {
            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public interface IFoo
    {
        object this[int i] { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            ReturnsExtensions.ReturnsNull<object>(substitute[1]);
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty()
        {
            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public virtual object Bar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar);
        }
    }
}";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty()
        {
            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public object Bar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar);
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(16, 51)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualIndexer()
        {
            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public virtual object this[int x] => new object();
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            ReturnsExtensions.ReturnsNull<object>(substitute[1]);
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer()
        {
            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public object this[int x] => new object();
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            ReturnsExtensions.ReturnsNull<object>(substitute[1]);
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(16, 51)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod()
        {
            var source = @"

namespace NSubstitute
{
    public class Foo
    {
        public object Bar()
        {
            return new object();
        }
    }

    public static class ReturnsExtensions
    {
        public static T ReturnsNull<T>(this T returnValue, T returnThis)
        {
            return default(T);
        }
    }

    public class FooTests
    {
        public void Test()
        {
            Foo substitute = null;
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar(), null);
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualProperty()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Bar", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public object Bar { get; }

        public object FooBar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar);
            ReturnsExtensions.ReturnsNull<object>(substitute.FooBar);
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(19, 51)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericProperty()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Bar", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public class Foo<T> where T : class
    {
        public T Bar { get; }

        public object FooBar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo<object>>();
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar);
            ReturnsExtensions.ReturnsNull<object>(substitute.FooBar);
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(19, 51)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar(System.Int32,System.Int32)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public object Bar(int x)
        {
            return new object();
        }

        public object Bar(int x, int y)
        {
            return new object();
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar(1, 2));
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar(1));
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(25, 51)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericMethod()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar``1(``0,``0)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public object Bar(int x)
        {
            return new object();
        }

        public object Bar<T>(T x, T y)
        {
            return new object();
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar<int>(1, 2));
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar(1));
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(25, 51)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualIndexer()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Item(System.Int32,System.Int32)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public object this[int x] => new object();
        public object this[int x, int y] => new object();
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            ReturnsExtensions.ReturnsNull<object>(substitute[1, 2]);
            ReturnsExtensions.ReturnsNull<object>(substitute[1]);
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(18, 51)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericIndexer()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Item(`0,`0)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public class Foo<T>
    {
        public object this[T x] => new object();
        public object this[T x, T y] => new object();
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            ReturnsExtensions.ReturnsNull<object>(substitute[1, 2]);
            ReturnsExtensions.ReturnsNull<object>(substitute[1]);
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(18, 51)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireType()
        {
             Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo", DiagnosticIdentifiers.NonVirtualSetupSpecification);

             var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public object Bar { get; set; }
        public object this[int x] => new object();
        public object FooBar()
        {
            return new object();
        }
    }

    public class FooBarBar
    {
        public object Bar { get;set; }
        public object this[int x] => new object();
        public object FooBar()
        {
            return new object();
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            ReturnsExtensions.ReturnsNull<object>(substitute[1]);
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar);
            ReturnsExtensions.ReturnsNull<object>(substitute.FooBar());

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar>();
            ReturnsExtensions.ReturnsNull<object>(substituteFooBarBar[1]);
            ReturnsExtensions.ReturnsNull<object>(substituteFooBarBar.Bar);
            ReturnsExtensions.ReturnsNull<object>(substituteFooBarBar.FooBar());
        }
    }
}";

             var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(36, 51)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(37, 51)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(38, 51)
                    }
                }
            };

             await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireGenericType()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo`1", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public class Foo<T>
    {
        public object Bar { get; set; }
        public object this[int x] => new object();
        public object FooBar()
        {
            return new object();
        }
    }

    public class FooBarBar<T>
    {
        public object Bar { get;set; }
        public object this[int x] => new object();
        public object FooBar()
        {
            return new object();
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            ReturnsExtensions.ReturnsNull<object>(substitute[1]);
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar);
            ReturnsExtensions.ReturnsNull<object>(substitute.FooBar());

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar<int>>();
            ReturnsExtensions.ReturnsNull<object>(substituteFooBarBar[1]);
            ReturnsExtensions.ReturnsNull<object>(substituteFooBarBar.Bar);
            ReturnsExtensions.ReturnsNull<object>(substituteFooBarBar.FooBar());
        }
    }
}";

            var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(36, 51)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(37, 51)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(38, 51)
                    }
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireNamespace()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("N:MyNamespace", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyOtherNamespace
{
    public class FooBarBar
    {
        public object Bar { get; set; }
        public object this[int x] => new object();
        public object FooBar()
        {
            return new object();
        }
    }
}

namespace MyNamespace
{
    using MyOtherNamespace;
    public class Foo
    {
        public object Bar { get; set; }
        public object this[int x] => new object();
        public object FooBar()
        {
            return new object();
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            ReturnsExtensions.ReturnsNull<object>(substitute[1]);
            ReturnsExtensions.ReturnsNull<object>(substitute.Bar);
            ReturnsExtensions.ReturnsNull<object>(substitute.FooBar());

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar>();
            ReturnsExtensions.ReturnsNull<object>(substituteFooBarBar[1]);
            ReturnsExtensions.ReturnsNull<object>(substituteFooBarBar.Bar);
            ReturnsExtensions.ReturnsNull<object>(substituteFooBarBar.FooBar());
        }
    }
}";

            var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(40, 51)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(41, 51)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(42, 51)
                    }
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingExtensionMethod()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.MyExtensions.GetBar(System.Object)~System.Object", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{
    public class FooTests
    {
        public void Test()
        {
            MyExtensions.Bar = Substitute.For<IBar>();
            var substitute = Substitute.For<object>();
            ReturnsExtensions.ReturnsNull<object>(substitute.GetBar());
            ReturnsExtensions.ReturnsNull<object>(substitute.GetFooBar());
        }
    }

    public static class MyExtensions
    {
        public static IBar Bar { get; set; }

        public static object GetBar(this object @object)
        {
            return Bar.Foo(@object);
        }

        public static object GetFooBar(this object @object)
        {
            return new object();
        }
    }

    public interface IBar
    {
        object Foo(object @obj);
    }
}";
            var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member GetFooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(13, 51)
                    }
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }
    }
}