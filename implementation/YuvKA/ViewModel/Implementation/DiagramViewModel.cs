using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace YuvKA
{
    public class DiagramViewModel : OutputWindowViewModel
    {
        public ObservableCollection<Input> Inputs
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public new DiagramNode NodeModel
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Input ReferenceVideo
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public IList<IGraphType> Types
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public void DeleteGraph(DiagramGraph graph)
        {
            throw new System.NotImplementedException();
        }

        public void AddGraph(Input video)
        {
            throw new System.NotImplementedException();
        }
    
        public class Input
        {
            public int Index { get; set; }
            public string Name { get { return "Video " + (Index + 1); } }
        }
    }
}
