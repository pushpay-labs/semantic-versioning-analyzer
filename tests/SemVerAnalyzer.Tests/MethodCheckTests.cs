// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit;

namespace Pushpay.SemVerAnalyzer.Tests
{
	public class MethodCheckTests : StandardRuleCheckBase
	{
		class Local : Base
		{
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
		public void Test()
		{
			PerformFieldCheck<Reference, Local>();
		}
	}
}
