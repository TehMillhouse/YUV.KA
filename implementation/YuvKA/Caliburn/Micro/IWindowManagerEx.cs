namespace Caliburn.Micro
{
	public interface IWindowManagerEx : IWindowManager
	{
		void ShowWindow(IViewAware rootModel, IViewAware owningModel);
	}
}