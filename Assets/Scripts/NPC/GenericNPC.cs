using UnityEngine;

public class GenericNPC : NPC
{
    private bool isGeneric;

    private void Awake(){ isGeneric = true; }

    void Start(){ if (isGeneric) return; }
    void Update(){ if (isGeneric) return; }
}
