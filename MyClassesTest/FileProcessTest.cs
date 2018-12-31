using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyClasses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace MyClassesTest
{
  [TestClass]
  public class FileProcessTest
	{
		#region fields and properties

		private const string BAD_FILE_NAME = @"C:\temp\nowhere";
		private string _GoodFileName;

		// Unittest framework expects the name "TestContext"
		// The tc has various props and methods available to it
		public TestContext TestContext { get; set; }

		#endregion

		#region Class Setup/Cleanup

		[ClassInitialize]
		public static void ClassInitialize(TestContext tc)
		{
			tc.WriteLine("In ClassInitialize");
		}

		[ClassCleanup]
		public static void ClassCleanup()
		{
		}

		#endregion

		#region Utils

		// can use non-test methods to support the testing
		public void SetGoodFileName()
		{
			// using config file to pull basic file name
			_GoodFileName = ConfigurationManager.AppSettings["GoodFileName"];
			if (_GoodFileName.Contains("[AppPath]"))
			{
				_GoodFileName = _GoodFileName.Replace("[AppPath]", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
			}
		}

		#endregion

		#region Test Setup/Cleanup
		
		// You don't need to pass in TC since it is already available
		[TestInitialize]
		public void BeforeTest()
		{
			TestContext.WriteLine($"In BeforeTest for {TestContext.TestName}");
			if (TestContext.TestName.StartsWith("FileNameDoesExist"))
			{
				SetGoodFileName();
				TestContext.WriteLine($"Creating the file: {_GoodFileName} ");
				File.AppendAllText(_GoodFileName, "This is my file content.");
			}
		}

		[TestCleanup]
		public void AfterTest()
		{
			TestContext.WriteLine("In AfterTest");

			if (TestContext.TestName.StartsWith("FileNameDoesExist"))
			{
				TestContext.WriteLine($"Deleting the file: {_GoodFileName} ");
				File.Delete(_GoodFileName);
			}
		}

		#endregion

		[TestMethod]
		[TestCategory("DataDriven")]
		[DataSource("System.Data.SqlClient",
			"Server=Localhost;Database=Sandbox;Integrated Security=Yes",
			"tests.FileProcessTest",
			DataAccessMethod.Sequential)]
		public void FileNameDoesExistFromDb()
		{
			TestContext.WriteLine("In the test");
			string fileName = TestContext.DataRow["FileName"].ToString();
			bool expectedValue = Convert.ToBoolean(TestContext.DataRow["ExpectedValue"]);
			bool causesException = Convert.ToBoolean(TestContext.DataRow["CausesException"]);

			try
			{
				FileProcess fp = new FileProcess();

				TestContext.WriteLine($"Testing the file: {fileName} ");
				bool fromCall = fp.FileExists(fileName);
				Assert.AreEqual(expectedValue, fromCall);

			}
			catch (AssertFailedException ex)
			{
				throw ex;
			}
			catch (ArgumentNullException)
			{
				Assert.IsTrue(causesException);
			}
		}

		[TestMethod]
		[Description("This is where my description goes.")]
		[Owner("Guy Whorley")]
		[Priority(1)]
		[TestCategory("Suite-Smoke")]
		public void FileNameDoesExist()
		{
			TestContext.WriteLine("In the test");
			FileProcess fp = new FileProcess();
			bool fromCall;

			TestContext.WriteLine($"Testing the file: {_GoodFileName} ");
			fromCall = fp.FileExists(_GoodFileName);

			Assert.IsTrue(fromCall);
		}


		[TestMethod]
		[Description("This is where my description goes.")]
		[Owner("Guy Whorley")]
		[Priority(1)]
		[TestCategory("Suite-Smoke")]
		public void FileNameDoesExistMessageWithFormatting()
		{
			FileProcess fp = new FileProcess();
			bool fromCall;

			TestContext.WriteLine($"Testing the file: {_GoodFileName} ");
			fromCall = fp.FileExists(_GoodFileName);
			
			Assert.IsFalse(fromCall, $"Checking {_GoodFileName}. Does NOT exist?");
		}

		[TestMethod]
		[Owner("Guy Whorley")]
		[TestCategory("Suite-Smoke")]
		public void FileNameDoesExistCustomMessage()
		{
			FileProcess fp = new FileProcess();
			bool fromCall;

			TestContext.WriteLine($"Testing the file: {_GoodFileName} ");
			fromCall = fp.FileExists(_GoodFileName);

			Assert.IsFalse(fromCall, "File does NOT exist.");
		}
		
		// TIMEOUT EXAMPLE
		[TestMethod]
		[Timeout(3000)]
		[TestCategory("Timeout")]
		public void SimulateTimeout()
		{
			System.Threading.Thread.Sleep(3100);
		}

		[TestMethod]
		[Owner("Bill")]
		[Priority(2)]
		[TestCategory("Suite-Smoke")]
		public void FileNameDoesNotExist()
		{
			FileProcess fp = new FileProcess();
			bool fromCall;

			fromCall = fp.FileExists(BAD_FILE_NAME);

			Assert.IsFalse(fromCall);
		}

		private const string FILE_NAME = @"FileToDeploy.txt";

		[TestMethod]
		[Owner("Guy Whorley")]
		[DeploymentItem(FILE_NAME)]
		public void FileNameDoesExistUsingDeploymentItem()
		{
			FileProcess fp = new FileProcess();
			string fileName;
			bool fromCall;

			fileName = TestContext.DeploymentDirectory + @"\" + FILE_NAME;
			TestContext.WriteLine($"Checking file: {FILE_NAME}");

			fromCall = fp.FileExists(fileName);

			Assert.IsTrue(fromCall);
		}

		[TestMethod]
		[Ignore]
		[Owner("Guy Whorley")]
		[TestCategory("Suite-Deep")]
		public void FileNameNullOrEmpty_ThrowsArgumentNullException()
		{
			FileProcess fp = new FileProcess();

			try
			{
				fp.FileExists("");
			}
			catch (ArgumentNullException)
			{
				// Test was a success
				return;
			}

			// Fail the test
			Assert.Fail("Call to FileExists() did NOT throw an ArgumentNullException.");
		}
		
		[TestMethod]
		[Owner("Mike")]
		[ExpectedException(typeof(ArgumentNullException))]
		[TestCategory("Suite-Full")]
		public void FileNameNullOrEmpty_ThrowsArgumentNullException_UsingAttribute()
		{
			FileProcess fp = new FileProcess();

			fp.FileExists("");
		}

		[TestMethod]
		[TestCategory("Assert.AreSameEqual")]
		public void Assert_StringTypesAreEqual()
		{
			Assert.AreEqual("Paul", "paul"); // case-sensitive
											 //Assert.AreEqual("Paul", "paul", false); // case-INsensitive
		}

		[TestMethod]
		[TestCategory("Assert.Instance")]
		public void Assert_ObjectsInstanceIsTypeOfFail()
		{
			FileNotFoundException fnfEx = new FileNotFoundException();
			
			Assert.IsNotInstanceOfType(fnfEx, typeof(ArgumentNullException));
		}

		[TestMethod]
		[TestCategory("Assert.Instance")]
		public void Assert_ObjectsInstanceIsTypeOfPass()
		{
			FileNotFoundException fnfEx = new FileNotFoundException();
			Assert.IsInstanceOfType(fnfEx, typeof(IOException));
		}


		[TestMethod]
		[TestCategory("Assert.Instance")]
		public void Assert_ObjectIsNull()
		{
			FileNotFoundException fnfEx = null;

			Assert.IsNull(fnfEx);
		}
		
		[TestMethod]
		[TestCategory("Assert.StringAssert")]
		public void ContainsTest()
		{
			StringAssert.Contains("Guy Whorley", "Guy");
		}

		[TestMethod]
		[TestCategory("Assert.StringAssert")]
		public void RegExIsAllLowerCase()
		{
			Regex r = new Regex(@"^([^A-Z])+$");
			StringAssert.Matches("all lower case", r);
		}

		[TestMethod]
		[TestCategory("Assert.StringAssert")]
		public void RegExIsNotAllLowerCase()
		{
			Regex r = new Regex(@"^([^A-Z])+$");
			StringAssert.Matches("All lower case", r);
		}

		[TestMethod]
		[TestCategory("Assert.Collections")]
		public void CollectionContains()
		{
			List<int> c1 = new List<int> { 1, 2, 3, 5, 6 };
			CollectionAssert.Contains(c1, 3);
		}

		[TestMethod]
		[TestCategory("Assert.Collections")]
		public void CollectionDoesNotContain()
		{
			List<int> c1 = new List<int> { 1, 2, 3, 5, 6 };
			CollectionAssert.DoesNotContain(c1, 3);
		}

		[TestMethod]
		[TestCategory("Assert.Collections")]
		public void CollectionContainsSubset()
		{
			List<int> superset = new List<int> { 1, 2, 3, 4, 5, 6 };
			List<int> subset = new List<int> { 2, 3, 4 };

			CollectionAssert.IsSubsetOf(subset, superset);		
		}

		[TestMethod]
		[TestCategory("Assert.Collections")]
		public void CollectionsEqualCaseDifferent()
		{
			//Must be in same order
			List<string> names1 = new List<string> { "Chris", "Whorley" };
			List<string> names2 = new List<string> { "chris", "whorley" };

			CollectionAssert.AreEqual(names1, names2);	
		}

		[TestMethod]
		[TestCategory("Assert.Collections")]
		public void CollectionsEqualCaseSame()
		{
			//Must be in same order
			List<string> names1 = new List<string> { "Chris", "Whorley" };
			List<string> names2 = new List<string> { "Chris", "Whorley" };

			CollectionAssert.AreEqual(names1, names2);
		}

		[TestMethod]
		[TestCategory("Assert.Collections")]
		public void CollectionsAreEquivalentCaseSame()
		{
			// Can be out-of-order
			List<string> names1 = new List<string> { "Chris", "Whorley" };
			List<string> names2 = new List<string> { "Whorley", "Chris" };

			CollectionAssert.AreEquivalent(names1, names2);
		}

		[TestMethod]
		[TestCategory("Assert.Collections")]
		public void CollectionsAreEquivalentDifferentSame()
		{
			// can be out-of-order
			List<string> names1 = new List<string> { "Chris", "Whorley" };
			List<string> names2 = new List<string> { "whorley", "chris" };

			CollectionAssert.AreEquivalent(names1, names2);
		}


		[TestMethod]
		[TestCategory("Assert.AreSameEqual")]
		
		public void Assert_RefOjectsAreSameObjectTest()
		{
			FileProcess fp1 = new FileProcess();
			FileProcess fp2 = new FileProcess();
			
			Assert.AreSame(fp1, fp2, $"{fp1.GetHashCode()} {fp2.GetHashCode()}"); // objects point to SAME object
		}

		[TestMethod]
		[TestCategory("Assert.AreSameEqual")]
		public void Assert_ValueOjectsAreEqualTest()
		{
			DateTime d1 = DateTime.Now;
			System.Threading.Thread.Sleep(1000);
			DateTime d2 = d1;
			Assert.AreNotEqual<DateTime>(d1, d2, $"{d1.GetHashCode()} {d2.GetHashCode()}"); // objects point to SAME object
		}
		
		[TestMethod]
		[TestCategory("Assert.AreSameEqual")]
		public void Assert_ObjectsAreNotSameObjectTypeTest()
		{
			DateTime d1 = DateTime.Now;
			System.Threading.Thread.Sleep(1000);
			DateTime d2 = d1;

			Assert.AreNotSame(d1, d2); // objects point to same object
		}
	}
}
