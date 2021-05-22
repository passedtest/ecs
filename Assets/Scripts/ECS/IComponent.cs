public interface IComponent 
{
    void Register(in int world, in int entity);
}

/// TODO: Binary serialization;
public interface IBinarySerializedCompoent
{
    void Serialize(System.IO.BinaryWriter writer);
    void Deserialize(System.IO.BinaryReader writer);
}