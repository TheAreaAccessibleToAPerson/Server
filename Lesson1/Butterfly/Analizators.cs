namespace Butterfly
{
    public abstract class Analizator<InputBufferType, OutputBufferType> : system.AnalizatorObject<InputBufferType, OutputBufferType>
        where InputBufferType : struct where OutputBufferType : struct
    {}

    public abstract class AnalizatorLocalBuffer<LocalBufferType, InputBufferType, OutputBufferType> : system.LocalValue.LocalBuffer<LocalBufferType>.Analizator<InputBufferType, OutputBufferType>
        where LocalBufferType : struct where InputBufferType : struct where OutputBufferType : struct
    { }
    public abstract class AnalizatorLocalBuffers<LocalBufferType, InputBufferType, OutputBufferType> : system.LocalValue.LocalBuffers<LocalBufferType>.Analizator<InputBufferType, OutputBufferType>
        where LocalBufferType : struct where InputBufferType : struct where OutputBufferType : struct
    { }
}
