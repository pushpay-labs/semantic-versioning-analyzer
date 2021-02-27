// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit;

namespace Pushpay.SemVerAnalyzer.Tests
{
	public class MethodCheckTests : StandardRuleCheckBase
	{
		class Local : Base
		{
			public void NewMethod(){ }
			public override void VirtualMethod() { }
		}

		class Reference : Base
		{
		}

		abstract class Base
		{
			public virtual void VirtualMethod() { }
		}

		public MethodCheckTests(IntegrationFixture integrationFixture)
			: base(integrationFixture)
		{
		}

		[Fact]
		public void NewOverride()
		{
			PerformFieldCheck<Reference, Local>(
				"`Local.VirtualMethod()` is a new override.",
				"`Local.NewMethod()` is new or has been made accessible."
			);
		}

		[Fact]
		public void LostOverride()
		{
			PerformFieldCheck<Local, Reference>(
				"`Local.VirtualMethod()` is no longer overridden.",
				"`Local.NewMethod()` is no longer present or accessible."
			);
		}
	}
}
