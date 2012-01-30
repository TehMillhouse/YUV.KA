using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Caliburn.Micro
{
	public interface IWindowManagerEx : IWindowManager
	{
		void SetOwner(IViewAware owningWindow, IViewAware ownedWindow);
	}
}
