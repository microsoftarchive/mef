namespace System.IO
{
	using System.Reflection;
	using System.Windows;

	public static class FileExtensions
	{
		public static Assembly ToAssembly(this FileInfo file)
		{
			AssemblyPart part = new AssemblyPart();

			using (var stream = file.OpenRead())
			{
				return part.Load(stream);
			}
		}
	}
}