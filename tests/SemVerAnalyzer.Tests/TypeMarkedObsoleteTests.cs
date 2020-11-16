using System;
using dnlib.DotNet;
using PowerAssert;
using Pushpay.SemVerAnalyzer.Engine.Rules;
using Xunit;

namespace Pushpay.SemVerAnalyzer.Tests
{
	public class TypeMarkedObsoleteTests
	{
		[Obsolete]
		class Local { }

		class Obsolete { }

		static readonly TypeDef _localDef;
		static readonly TypeDef _obsoleteDef;
		readonly TypeMarkedObsoleteRule _rule;

		static TypeMarkedObsoleteTests()
		{
#pragma warning disable 612
			_localDef = typeof(Local).GetTypeDef();
#pragma warning restore 612
			_obsoleteDef = typeof(Obsolete).GetTypeDef();
		}

		public TypeMarkedObsoleteTests()
		{
			_rule = new TypeMarkedObsoleteRule();
		}

		[Fact]
		public void AddsObsoleteAttributeApplies()
		{
			PAssert.IsTrue(() => _rule.Applies(_obsoleteDef, _localDef));
		}

		[Fact]
		public void StaysObsoleteAttributeDoesNotApply()
		{
			PAssert.IsTrue(() => !_rule.Applies(_localDef, _localDef));
		}

		[Fact]
		public void StaysNonObsoleteAttributeDoesNotApply()
		{
			PAssert.IsTrue(() => !_rule.Applies(_obsoleteDef, _obsoleteDef));
		}

		[Fact]
		public void RemovesObsoleteAttributeDoesNotApply()
		{
			PAssert.IsTrue(() => !_rule.Applies(_localDef, _obsoleteDef));
		}
	}
}
