namespace LibreLancer.GameData.Items;

public class TradelaneEquipment : Equipment
{
    public ResolvedFx RingActive; 
    
    static TradelaneEquipment() => EquipmentObjectManager.RegisterType<TradelaneEquipment>(AddEquipment);

    static GameObject AddEquipment(GameObject parent, ResourceManager res, EquipmentType type, string hardpoint,
        Equipment equip)
    {
        if (type != EquipmentType.Server)
            parent.Components.Add(new CTradelaneComponent(parent, (TradelaneEquipment) equip));
        return null;
    }
}