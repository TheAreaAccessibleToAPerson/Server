namespace Butterfly
{
    public abstract class ProgramController : system.ProgramObject {}

    public abstract class IndependentController : system.ThreadObject 
    { 
        public IndependentController() : base(TypeObjectData.INDEPENDENT_CONTROLLER){} 
    }
    public abstract class IndependentControllerLocalBuffer<LocalBufferType> : system.LocalValue.LocalBuffer<LocalBufferType>.Controller where LocalBufferType : struct
    {
        public IndependentControllerLocalBuffer() : base(TypeObjectData.INDEPENDENT_CONTROLLER) { }
    }
    public abstract class IndependentControllerLocalBuffers<LocalBufferType> : system.LocalValue.LocalBuffer<LocalBufferType>.Controller where LocalBufferType : struct
    {
        public IndependentControllerLocalBuffers() : base(TypeObjectData.INDEPENDENT_CONTROLLER) { }
    }
    public abstract class Controller : system.ThreadObject 
    { 
        public Controller() : base(TypeObjectData.CONTROLLER) { } 
    }
    public abstract class ControllerLocalBuffer<LocalBufferType> : system.LocalValue.LocalBuffer<LocalBufferType>.Controller where LocalBufferType : struct
    { 
        public ControllerLocalBuffer() : base(TypeObjectData.CONTROLLER) { } 
    }
    public abstract class ControllerLocalBuffers<LocalBufferType> : system.LocalValue.LocalBuffer<LocalBufferType>.Controller where LocalBufferType : struct
    {
        public ControllerLocalBuffers() : base(TypeObjectData.CONTROLLER) { }
    }
}
