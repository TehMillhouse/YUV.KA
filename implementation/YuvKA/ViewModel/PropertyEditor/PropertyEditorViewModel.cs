using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace YuvKA.ViewModel.PropertyEditor
{
	public class PropertyEditorViewModel
	{
		public PropertyEditorViewModel()
		{
		}

		public object Source {
			get { return Source; }
			set {
				this.Source = value;
				/* Get all available PropertyViewModelTypes */
				ICollection<System.Type> viewModels = new List<System.Type>();
				foreach (PropertyViewModel pvm in IoC.GetAllInstances(typeof(PropertyViewModel))) {
					viewModels.Add(pvm.GetType());
				}
				/* Get all propertydescriptors of source */
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value.GetType());
				/* Create a list of PropertyViewModels bound to the properties of source */
				List<PropertyViewModel>pvmList = new List<PropertyViewModel>();
				foreach (PropertyDescriptor pd in properties) {
					if (pd.IsBrowsable) {
						System.Type fittingPVM = viewModels.Single(pvm => pvm.BaseType.GetGenericArguments()[0] == pd.PropertyType);
						object[] parameters = { Source, pd };
						pvmList.Add((PropertyViewModel)(fittingPVM.GetConstructors()[0].Invoke(parameters)));
					}
				}
				Properties = pvmList;
			}
		}
		public IEnumerable<PropertyViewModel> Properties { get; private set; }
	}
}