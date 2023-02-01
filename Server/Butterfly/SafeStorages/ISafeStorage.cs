namespace Butterfly.SafeStorage
{
    public interface ISafeStorage<ValueType>
    {
        public void Add(ValueType pValue);
        public void Clear();
        public bool ExtractAll(out ValueType[] oResult);
    }
}
