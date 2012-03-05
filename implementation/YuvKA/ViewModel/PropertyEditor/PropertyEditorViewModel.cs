using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;

namespace YuvKA.ViewModel.PropertyEditor
{
	/// <summary>
	/// This class is responsible for managing the
	/// viewmodels of the properties of one object
	/// </summary>
	public class PropertyEditorViewModel : PropertyChangedBase
	{
		private object source;

		/// <summary>
		/// Constructs a propertyeditorviewmodel without any manged object
		/// </summary>
		public PropertyEditorViewModel()
		{
		}

		/// <summary>
		/// The object, of which this object manages the propertyviewmodels
		/// </summary>
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
						System.Type fittingPVM = viewModels.SingleOrDefault(pvm => (pvm.BaseType.GetGenericArguments()[0].IsAssignableFrom(pd.PropertyType)));
						if (fittingPVM == null)
							return;
						PropertyViewModel vm = (PropertyViewModel)Activator.CreateInstance(fittingPVM);
						vm.Initialize(source, pd);
						pvmList.Add(vm);
					}
				}
				Properties = pvmList;
				NotifyOfPropertyChange(() => Properties);
			}
		}

		/// <summary>
		/// The list of viewmodels assigned to the properties of the source object
		/// </summary>
		public IEnumerable<PropertyViewModel> Properties { get; private set; }
	}
}