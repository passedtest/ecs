public interface IComponent 
{
    void Register(int world, int entity);
}

/// TODO: Binary serialization;
public interface IBinarySerializedCompoent
{
    void Serialize(System.IO.BinaryWriter writer);
    void Deserialize(System.IO.BinaryReader writer);
}