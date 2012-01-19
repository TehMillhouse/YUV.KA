using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace YuvKA.ViewModel.PropertyEditor
{
	public class PropertyEditorViewModel : PropertyChangedBase
	{
		private object source;

		public PropertyEditorViewModel()
		{
		}

		public object Source
		{
			get { return source; }
			set
			{
				this.source = value;
				/* Get all available PropertyViewModelTypes */
				ICollection<System.Type> viewModels = new List<System.Type>();
				foreach (PropertyViewModel pvm in IoC.GetAllInstances(typeof(PropertyViewModel))) {
					viewModels.Add(pvm.GetType());
				}
				/* Get all propertydescriptors of source */
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value.GetType());
				/* Create a list of PropertyViewModels bound to the properties of source */
				List<PropertyViewModel> pvmList = new List<PropertyViewModel>();
				foreach (PropertyDescriptor pd in properties) {
					var browsable = pd.Attributes.OfType<BrowsableAttribute>().SingleOrDefault();
					if (browsable != null && browsable.Browsable) {
						System.Type fittingPVM = viewModels.Single(pvm => (pvm.BaseType.GetGenericArguments()[0].IsAssignableFrom(pd.PropertyType)));
						PropertyViewModel vm = (PropertyViewModel) Activator.CreateInstance(fittingPVM);
						vm.Source = source;
						vm.Property = pd;
						pvmList.Add(vm);
					}
				}
				Properties = pvmList;
				NotifyOfPropertyChange(() => Properties);
			}
		}
		public IEnumerable<PropertyViewModel> Properties { get; private set; }
	}
}