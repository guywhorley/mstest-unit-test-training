using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MyClassesTest
{
	/// <summary>
	/// Assembly Initialize and Cleanup methods
	/// </summary>
	[TestClass]
	public class MyClassesTestInitialization
	{
		/// <summary>
		/// Unittesting framework will pass in a TestContext object automatically.
		/// </summary>
		/// <param name="tc"></param>
		[AssemblyInitialize]
		public static void AssemblyInitialize(TestContext tc)
		{
			//
			// TODO: Create resources needed for your tests
			//
			tc.WriteLine("In the AssemblyIntializer");
		}

		[AssemblyCleanup]
		public static void AssemblyCleanup()
		{
			// TODO: Clean up any resources
		}
	}
}
