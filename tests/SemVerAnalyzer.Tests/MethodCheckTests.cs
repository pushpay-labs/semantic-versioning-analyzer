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
			public new void VirtualMethod(){ }
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
		public void Test()
		{
			PerformFieldCheck<Reference, Local>(
				"`Local.VirtualMethod(Local)` has been overridden.",
				"`Local.NewMethod(Local)` is new or has been made accessible."
			);
		}
	}
}
