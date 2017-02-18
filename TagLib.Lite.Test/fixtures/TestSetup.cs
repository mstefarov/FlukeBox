using System;
using System.IO;
using NUnit.Framework;

namespace TagLib.Tests
{
	[SetUpFixture]
	public class MySetUpClass
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}
	}
}
