using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitSharp.Demo
{
	public interface IRepositoryView
	{
		void Update(Repository repository);
	}
}
