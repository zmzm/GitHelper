using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GitSharp.Demo.CommitView
{
	public class WorkingTree
	{
		public WorkingTree(string path, RepositoryStatus status)
		{
			Path = path;
			RelativePath = "";
			RepositoryStatus = status;
		}

		public RepositoryStatus RepositoryStatus
		{
			get; set;
		}

		public string Path
		{
			get;
			private set;
		}

		protected virtual DirectoryInfo DirectoryInfo
		{
			get
			{
				return new DirectoryInfo(Path);
			}
		}

		public string Name
		{
			get
			{
				return DirectoryInfo.Name;
			}
		}

		public string RelativePath
		{
			get; private set;
		}

		public IEnumerable<WorkingTree> Children
		{
			get {
				return DirectoryInfo.GetDirectories().Select(d => new WorkingTree(d.FullName, RepositoryStatus) {RelativePath = System.IO.Path.Combine(RelativePath, d.Name)}).Concat
					(
						DirectoryInfo.GetFiles().Select(f => new WorkingFile(f.FullName, RepositoryStatus) { RelativePath = System.IO.Path.Combine(RelativePath, f.Name) } as WorkingTree)
					);
			}
		}

		public virtual string Status
		{
			get { return "";  }
		}
	}

	public class WorkingFile : WorkingTree
	{
		public WorkingFile(string path, RepositoryStatus status)
			: base(path, status)
		{
		}


		protected override DirectoryInfo DirectoryInfo
		{
			get
			{
				return new FileInfo(Path).Directory;
			}
		}

		FileInfo FileInfo
		{
			get
			{
				return new FileInfo(Path);
			}
		}

		public string Name
		{
			get
			{
				return FileInfo.Name;
			}
		}

		public IEnumerable<WorkingTree> Children
		{
			get { return new WorkingTree[0]; }
		}

		public override string Status
		{
			get
			{
				if (RepositoryStatus.Added.Contains(RelativePath))
					return "Added";
				if (RepositoryStatus.MergeConflict.Contains(RelativePath))
					return "MergeConflict";
				if (RepositoryStatus.Missing.Contains(RelativePath))
					return "Missing";
				if (RepositoryStatus.Modified.Contains(RelativePath))
					return "Modified";
				if (RepositoryStatus.Removed.Contains(RelativePath))
					return "Removed";
				if (RepositoryStatus.Staged.Contains(RelativePath))
					return "Staged";
				if (RepositoryStatus.Untracked.Contains(RelativePath))
					return "Untracked";
				return "";
			}
		}
	}
}
