namespace YuvKA
{
    public class ArtifactsOverlay : IOverlayType
    {
        public bool DependsOnReference { get { return true; } }

        public Frame Process(Frame frame, Frame reference)
        {
            throw new System.NotImplementedException();
        }
    }

    public class MoveVectorsOverlay : IOverlayType
    {
        public bool DependsOnReference { get { return false; } }

        public Frame Process(Frame frame, Frame reference)
        {
            throw new System.NotImplementedException();
        }
    }

    public class BlocksOverlay : IOverlayType
    {
        public bool DependsOnReference { get { return false; } }

        public Frame Process(Frame frame, Frame reference)
        {
            throw new System.NotImplementedException();
        }
    }
}