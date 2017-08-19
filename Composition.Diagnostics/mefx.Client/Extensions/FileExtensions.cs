namespace System.IO
{
	using System.Reflection;

	public static class FileExtensions
	{
		public static Assembly ToAssembly(this FileInfo file)
		{
			return Assembly.LoadFrom(file.FullName);
		}
	}
}