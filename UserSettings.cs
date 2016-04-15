using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;

namespace GitSharp.Demo
{
	public static class UserSettings
	{
		public static string UserSettingsDirectory
		{
			get
			{
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GitSharp.Demo");
			}
		}

		public static string GetString(string setting)
		{
			try
			{
				var filename = Path.Combine(UserSettingsDirectory, setting);
				if (!new DirectoryInfo(UserSettingsDirectory).Exists || !new FileInfo(filename).Exists)
					return null;
				return File.ReadAllText(filename);
			}
			catch (IOException) { }
			catch (UnauthorizedAccessException) { }
			catch (SecurityException) { }
			return null;
		}

		public static void SetValue(string setting, string value)
		{
			try
			{
				var filename = Path.Combine(UserSettingsDirectory, setting);
				if (!new DirectoryInfo(UserSettingsDirectory).Exists)
					Directory.CreateDirectory(UserSettingsDirectory);
				if (value == null)
					File.Delete(filename);
				else
					File.WriteAllText(filename, value);
			}
			catch (IOException) { }
			catch (UnauthorizedAccessException) { }
			catch (SecurityException) { }
		}
	}
}

#if DEBUG

namespace Test
{
	using GitSharp.Demo;
	using NUnit.Framework;

	[TestFixture]
	public class UserSettingsTest
	{
		[Test]
		public void ReadWrite()
		{
			Assert.IsNull(UserSettings.GetString("not a valid setting"));
			UserSettings.SetValue("test", "hello world!");
			Assert.AreEqual("hello world!", UserSettings.GetString("test"));
			UserSettings.SetValue("test", null);
			Assert.IsNull(UserSettings.GetString("test"));
		}
	}
}
#endif